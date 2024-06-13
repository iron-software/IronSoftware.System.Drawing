using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests
{
    [Trait(TestingTraits.CATEGORY, TestingTraits.INTEGRATION_TEST)]
    public abstract class TestsBase : IDisposable
    {
        protected static readonly TargetFramework TargetFramework = new TargetFramework();
        protected readonly ITestOutputHelper Console;

        static TestsBase()
        {
        }

        protected TestsBase(ITestOutputHelper output)
        {
            Console = output;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static string GetDataPath()
        {
            string settingsValue = TargetFramework.GetAppSettingsValue("IronSoftware.Drawing.Common.Tests.DataFolder");
            settingsValue = settingsValue.Replace('\\', Path.DirectorySeparatorChar);

            string currentFolder = Environment.CurrentDirectory;

            string dataPath = Path.Combine(currentFolder, settingsValue);
            string fullPath = Path.GetFullPath(dataPath);
            return fullPath;
        }

        public static string GetRelativeFilePath(string fileName)
        {
            return GetRelativeFilePath(null, fileName);
        }

        public static string GetRelativeFilePath(string mainPath, string fileName)
        {
            string dataPath = GetDataPath();
            if (!string.IsNullOrEmpty(mainPath))
            {
                string fullPath = Path.Combine(dataPath, mainPath);
                if (mainPath.Contains("expected") && IsUnix())
                {
                    fullPath = Path.Combine(fullPath, "Unix");
                }

                fullPath = Path.Combine(fullPath, fileName);
                fullPath = fullPath.Replace('\\', Path.DirectorySeparatorChar);

                return fullPath;
            }
            else
            {
                return Path.Combine(dataPath, fileName);
            }
        }

        protected static bool IsUnix()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
        
        /// <summary>
        /// Get current processor architecture/bittiness string
        /// </summary>
        /// <returns>String representing architecture of the current process</returns>
        public static string GetArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                ? "arm64"
                : Environment.Is64BitProcess
                ? "x64"
                : "x86";
        }
    }
}
