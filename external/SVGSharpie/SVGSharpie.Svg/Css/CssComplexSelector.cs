using System;
using System.Linq;

namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a sequence of one or more <see cref="CssCompoundSelector"/> separated by <see cref="CssCombinatorType"/>s
    /// </summary>
    public sealed class CssComplexSelector : CssSelector
    {
        /// <inheritdoc cref="CssSelector.SelectorType"/>
        public override CssSelectorType SelectorType => CssSelectorType.Complex;

        /// <inheritdoc cref="CssSelector.Specificity"/>
        public override CssSpecificity Specificity
            => Items.Aggregate(new CssSpecificity(), (p, item) => p + item.Selector.Specificity);

        /// <summary>
        /// Gets the collection of items (selector followed by a combinator) in the current selector
        /// </summary>
        public CssComplexSelectorItem[] Items { get; }

        public CssComplexSelector(CssComplexSelectorItem[] items)
        {
            if (items?.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(items));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <inheritdoc cref="CssSelector.Accept"/>
        public override void Accept(CssSelectorVisitor visitor)
            => visitor.VisitComplexSelector(this);

        /// <inheritdoc cref="CssSelector.Accept"/>
        public override TResult Accept<TResult>(CssSelectorVisitor<TResult> visitor)
            => visitor.VisitComplexSelector(this);

        public override string ToString() => string.Join(" ", Items.Select(i => i.ToString()));
    }
}