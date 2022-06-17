using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests
{
    [Trait(TestingTraits.Category, TestingTraits.IntegrationTest)]
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
        }

        public static string GetDataPath()
        {
            var settingsValue = TargetFramework.GetAppSettingsValue("IronSoftware.Drawing.Common.Tests.DataFolder");
            settingsValue = settingsValue.Replace('\\', Path.DirectorySeparatorChar);

            var currentFolder = Environment.CurrentDirectory;

            var dataPath = Path.Combine(currentFolder, settingsValue);
            var fullPath = Path.GetFullPath(dataPath);
            return fullPath;
        }

        public static string GetRelativeFilePath(string fileName)
        {
            return GetRelativeFilePath(null, fileName);
        }

        public static string GetRelativeFilePath(string mainPath, string fileName)
        {
            var dataPath = GetDataPath();
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
    }
}
