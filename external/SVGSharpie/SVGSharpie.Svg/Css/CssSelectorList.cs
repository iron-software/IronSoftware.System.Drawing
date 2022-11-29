using System.Collections.Generic;

namespace SVGSharpie.Css
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a comma separated list of <see cref="T:SVGSharpie.Css.CssSelector" />s
    /// </summary>
    public sealed class CssSelectorList : List<CssSelector>
    {
        public override string ToString() => string.Join(", ", this);
    }
}