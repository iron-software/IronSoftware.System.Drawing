using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public sealed class IgnoreOnMacFactAttribute : FactWithAutomaticDisplayNameAttribute
    {
        public IgnoreOnMacFactAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName] string testMethodName = "")
        {
            if (charsToReplace != null)
            {
                DisplayName = testMethodName?.Replace(charsToReplace, replacementChars);
            }

            if (!IsRunningOnMacOSX())
            {
                return;
            }

            Skip = "Ignored on Azure DevOps";
        }

        /// <summary>Determine if runtime is MacOSX.</summary>
        /// <returns>True if being executed in MacOSX, false otherwise.</returns>
        public static bool IsRunningOnMacOSX()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}
