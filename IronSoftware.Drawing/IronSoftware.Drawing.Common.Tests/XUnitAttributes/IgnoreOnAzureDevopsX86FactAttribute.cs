using System;
using IronSoftware.Drawing.Common.Tests;
using Xunit;

public sealed class IgnoreOnAzureDevopsX86FactAttribute : FactWithAutomaticDisplayNameAttribute
{
    /// <summary>
    /// This class using for Skip test on Azure Devops and x86 architect
    /// <para>Regarding to OcrWizardTests.RunningWizardShouldFindBetterResult always failed on Devops x86 tested</para>
    /// </summary>
    public IgnoreOnAzureDevopsX86FactAttribute()
    {
        if (!(IsRunningOnAzureDevOps() && IsRunningOnX86()))
        {
            return;
        }

        Skip = "Ignored on Azure DevOps";
    }

    /// <summary>Determine if runtime is Azure DevOps.</summary>
    /// <returns>True if being executed in Azure DevOps, false otherwise.</returns>
    public static bool IsRunningOnAzureDevOps()
    {
        return Environment.GetEnvironmentVariable("SYSTEM_DEFINITIONID") != null;
    }

    // <summary>Determine if runtime is x86 architect.</summary>
    /// <returns>True if being executed in x86 architect, false otherwise.</returns>
    public static bool IsRunningOnX86()
    {
        return TestsBase.GetArchitecture() == "x86";
    }
}