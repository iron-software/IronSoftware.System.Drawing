using System;

namespace SVGSharpie.Css
{
    /// <summary>
    /// Describes a single item of a <see cref="CssComplexSelector"/>
    /// </summary>
    public sealed class CssComplexSelectorItem
    {
        /// <summary>
        /// Gets the selector preceeding the <see cref="Combinator"/>
        /// </summary>
        public CssCompoundSelector Selector { get; }

        /// <summary>
        /// Gets the combinator following the <see cref="Selector"/>
        /// </summary>
        public CssCombinatorType? Combinator { get; }

        public CssComplexSelectorItem(CssCompoundSelector selector, CssCombinatorType? combinator)
        {
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            Combinator = combinator;
        }

        public override string ToString() => $"{Selector} {Combinator}";
    }
}