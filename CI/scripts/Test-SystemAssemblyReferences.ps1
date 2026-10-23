<#
.SYNOPSIS
    Fails the build if a DLL we ship references a System.* compatibility assembly at a higher
    version than the NuGet dependency declared in the nuspec guarantees.

.DESCRIPTION
    On .NET Framework an assembly reference to, for example, System.Memory 4.0.2.0 cannot be
    satisfied by System.Memory 4.0.1.2 on disk; the app throws FileLoadException. This happened
    in OCR-372: code compiled against System.Memory 4.6.x shipped in a package that only
    guaranteed 4.5.5.

    For every netstandard2.0 / .NET Framework dependency group in the nuspec, this script reads
    the DLLs that group ships (from the nuspec <file> entries), lists their references to the
    watched assemblies and checks each one against the assembly version inside the package
    version declared for that group. A watched reference without a declared dependency also
    fails.

.EXAMPLE
    pwsh CI/scripts/Test-SystemAssemblyReferences.ps1 `
        -NuspecPath NuGet/IronSoftware.Drawing.nuspec `
        -BinRoot IronSoftware.Drawing/IronSoftware.Drawing.Common/bin/Release
#>
param(
    [Parameter(Mandatory = $true)] [string] $NuspecPath,
    # Folder that replaces "..\bin\$configuration$\" in the nuspec <file src> entries.
    [Parameter(Mandatory = $true)] [string] $BinRoot,
    [string] $PackagesRoot = $(if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path $HOME '.nuget/packages' }),
    [string[]] $Frameworks = @('netstandard2.0', 'net462', 'net472', 'net48'),
    [string[]] $WatchedAssemblies = @(
        'System.Memory',
        'System.Buffers',
        'System.Numerics.Vectors',
        'System.Runtime.CompilerServices.Unsafe',
        'System.Threading.Tasks.Extensions',
        'Microsoft.Bcl.AsyncInterfaces',
        'System.Text.Json',
        'System.Text.Encodings.Web'
    )
)

$ErrorActionPreference = 'Stop'

function Get-AssemblyReferences([string] $path) {
    $stream = [IO.File]::OpenRead($path)
    try {
        $pe = [System.Reflection.PortableExecutable.PEReader]::new($stream)
        $md = [System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe)
        foreach ($handle in $md.AssemblyReferences) {
            $ref = $md.GetAssemblyReference($handle)
            [pscustomobject]@{ Name = $md.GetString($ref.Name); Version = $ref.Version }
        }
        $pe.Dispose()
    }
    finally { $stream.Dispose() }
}

function Get-AssemblyVersion([string] $path) {
    $stream = [IO.File]::OpenRead($path)
    try {
        $pe = [System.Reflection.PortableExecutable.PEReader]::new($stream)
        $md = [System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe)
        $version = $md.GetAssemblyDefinition().Version
        $pe.Dispose()
        return $version
    }
    finally { $stream.Dispose() }
}

# Lowest assembly version the given package version provides to .NET Framework / netstandard2.0
# consumers. The lib folders can differ (System.Memory 4.6.3: net462 = 4.0.5.0, netstandard2.0 = 4.0.2.0),
# so take the minimum to be safe for every consumer.
function Get-PackageAssemblyVersion([string] $id, [string] $version, [string] $assemblyName) {
    $packageDir = Join-Path $PackagesRoot (Join-Path $id.ToLowerInvariant() $version.ToLowerInvariant())
    if (-not (Test-Path $packageDir)) {
        throw "Package $id $version is not in '$PackagesRoot'. Restore the solution before running this check."
    }
    $versions = foreach ($lib in 'net462', 'net461', 'net46', 'net45', 'netstandard2.0') {
        $dll = Join-Path $packageDir "lib/$lib/$assemblyName.dll"
        if (Test-Path $dll) { Get-AssemblyVersion $dll }
    }
    if (-not $versions) {
        throw "No .NET Framework / netstandard2.0 assembly found for $id $version in '$packageDir'."
    }
    return ($versions | Sort-Object | Select-Object -First 1)
}

function ConvertTo-ShortFramework([string] $tfm) {
    switch -Regex ($tfm) {
        '^\.NETStandard(\d)\.(\d)$' { return "netstandard$($Matches[1]).$($Matches[2])" }
        '^\.NETFramework(\d)\.(\d)(?:\.(\d))?$' { return "net$($Matches[1])$($Matches[2])$($Matches[3])" }
        default { return $tfm.ToLowerInvariant() }
    }
}

[xml] $nuspec = Get-Content -Raw -Path $NuspecPath
$failures = New-Object System.Collections.Generic.List[string]
$checked = 0

$files = $nuspec.SelectNodes("//*[local-name()='files']/*[local-name()='file']")
foreach ($group in $nuspec.SelectNodes("//*[local-name()='dependencies']/*[local-name()='group']")) {
    $tfm = ConvertTo-ShortFramework $group.GetAttribute('targetFramework')
    if ($Frameworks -notcontains $tfm) { continue }

    $declared = @{}
    foreach ($dep in $group.SelectNodes("*[local-name()='dependency']")) {
        # Minimum version of a range such as "[4.6.3, )" or "4.6.3".
        $declared[$dep.GetAttribute('id')] = ($dep.GetAttribute('version') -replace '^[\[\(]\s*', '' -split '[,\]\)]')[0].Trim()
    }

    $dlls = foreach ($file in $files) {
        $target = $file.GetAttribute('target') -replace '/', '\'
        if ($target.TrimEnd('\') -ne "lib\$tfm") { continue }
        $src = $file.GetAttribute('src') -replace '/', '\'
        $relative = $src -replace '^.*?\\bin\\\$configuration\$\\', ''
        Get-ChildItem -Path (Join-Path $BinRoot $relative) -ErrorAction SilentlyContinue |
            Where-Object { $_.Extension -eq '.dll' }
    }
    if (-not $dlls) {
        $failures.Add("[$tfm] No DLLs found under '$BinRoot' for the nuspec lib\$tfm entries. Build before running this check.")
        continue
    }

    foreach ($dll in $dlls) {
        foreach ($ref in Get-AssemblyReferences $dll.FullName) {
            if ($WatchedAssemblies -notcontains $ref.Name) { continue }
            $checked++
            if (-not $declared.ContainsKey($ref.Name)) {
                $failures.Add("[$tfm] $($dll.Name) references $($ref.Name) $($ref.Version), but the nuspec does not declare a $($ref.Name) dependency for $tfm.")
                continue
            }
            $packageVersion = $declared[$ref.Name]
            $provided = Get-PackageAssemblyVersion $ref.Name $packageVersion $ref.Name
            $status = if ($ref.Version -gt $provided) { 'FAIL' } else { 'ok' }
            Write-Host ("[{0}] {1}: {2} {3} -> declared package {4} provides {5}  {6}" -f $tfm, $dll.Name, $ref.Name, $ref.Version, $packageVersion, $provided, $status)
            if ($status -eq 'FAIL') {
                $failures.Add("[$tfm] $($dll.Name) references $($ref.Name) $($ref.Version), but the declared $($ref.Name) $packageVersion only provides $provided. Raise the nuspec dependency (and the compile reference) together.")
            }
        }
    }
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Host "##vso[task.logissue type=error]$_"; Write-Host "ERROR: $_" }
    exit 1
}
Write-Host "System.* assembly reference check passed ($checked references checked)."
