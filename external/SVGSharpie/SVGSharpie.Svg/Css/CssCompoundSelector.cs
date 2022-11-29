using System;
using System.Linq;

namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a compound selector, which is a sequence of simple selectors that are not separated by a combinator. 
    /// It represents an element that matches all of the simple selectors it contains. If it contains a type selector 
    /// or universal selector, that selector must come first in the sequence. Only one type selector or universal 
    /// selector is allowed in the sequence.
    /// </summary>
    public sealed class CssCompoundSelector : CssSelector
    {
        /// <inheritdoc cref="CssSelector.SelectorType"/>
        public override CssSelectorType SelectorType => CssSelectorType.Compound;

        /// <inheritdoc cref="CssSelector.Specificity"/>
        public override CssSpecificity Specificity
            => Selectors.Aggregate(new CssSpecificity(), (p, selector) => p + selector.Specificity);

        /// <summary>
        /// Gets the collection of <see cref="CssSimpleSelector"/> not separated by a combinator
        /// </summary>
        public CssSimpleSelector[] Selectors { get; }

        public CssCompoundSelector(CssSimpleSelector[] selectors)
        {
            Selectors = selectors ?? throw new ArgumentNullException(nameof(selectors));
            var typeOrUniversalCount = Selectors.Count(i =>
                i.SimpleSelectorType == CssSimpleSelectorType.TypeSelector ||
                i.SimpleSelectorType == CssSimpleSelectorType.UniversalSelector);
            if (typeOrUniversalCount == 1)
            {
                if (selectors[0].SimpleSelectorType != CssSimpleSelectorType.TypeSelector &&
                    selectors[0].SimpleSelectorType != CssSimpleSelectorType.UniversalSelector)
                {
                    throw new ArgumentException("Type or universal selectors must come first in the sequence");
                }
            }
            if (typeOrUniversalCount > 1)
            {
                throw new ArgumentException("Only one type selector or universal selector is allowed and must come first");
            }
        }

        /// <inheritdoc cref="CssSelector.Accept"/>
        public override void Accept(CssSelectorVisitor visitor)
            => visitor.VisitCompoundSelector(this);

        /// <inheritdoc cref="CssSelector.Accept"/>
        public override TResult Accept<TResult>(CssSelectorVisitor<TResult> visitor)
            => visitor.VisitCompoundSelector(this);

        public override string ToString() => string.Join(string.Empty, Selectors.Select(i => i.ToString()));
    }
}