using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public sealed class IgnoreOnUnixFactAttribute : FactWithAutomaticDisplayNameAttribute
    {
        public IgnoreOnUnixFactAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName] string testMethodName = "")
        {
            if (charsToReplace != null)
            {
                DisplayName = testMethodName?.Replace(charsToReplace, replacementChars);
            }

            if (!IsRunningOnUnix())
            {
                return;
            }

            Skip = "Ignored on Azure Linux or OSX";
        }

        /// <summary>Determine if runtime is Linux or OSX.</summary>
        /// <returns>True if being executed in Linux or OSX, false otherwise.</returns>
        public static bool IsRunningOnUnix()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}
