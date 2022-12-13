using System.Runtime.CompilerServices;

namespace IronSoftware.Drawing.Common.Tests
{
    [Xunit.Sdk.XunitTestCaseDiscoverer("Xunit.Sdk.TheoryDiscoverer", "xunit.execution.{Platform}")]
    public class TheoryWithAutomaticDisplayNameAttribute : FactWithAutomaticDisplayNameAttribute
    {
        public TheoryWithAutomaticDisplayNameAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName] string testMethodName = "") : base(charsToReplace, replacementChars, testMethodName) { }
    }
}