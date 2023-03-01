using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public sealed class IgnoreOnUnixFactAttribute : FactWithAutomaticDisplayNameAttribute
    {
        public IgnoreOnUnixFactAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName] string testMethodName = "")
        {
            if (charsToReplace != null)
            {
                base.DisplayName = testMethodName?.Replace(charsToReplace, replacementChars);
            }

            if (!IsRunningOnUnix())
            {
                return;
            }

            Skip = "Ignored on Azure DevOps";
        }

        /// <summary>Determine if runtime is Azure DevOps.</summary>
        /// <returns>True if being executed in Azure DevOps, false otherwise.</returns>
        public static bool IsRunningOnUnix()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }
    }
}
