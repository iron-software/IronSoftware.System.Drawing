using System.Runtime.CompilerServices;

namespace IronSoftware.Drawing.Common.Tests
{
    public class FactWithAutomaticDisplayNameAttribute : Xunit.FactAttribute
    {
        public FactWithAutomaticDisplayNameAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName] string testMethodName = "")
        {
            if(charsToReplace != null)
            {
                base.DisplayName = testMethodName?.Replace(charsToReplace, replacementChars);
            }
        }
    }
}