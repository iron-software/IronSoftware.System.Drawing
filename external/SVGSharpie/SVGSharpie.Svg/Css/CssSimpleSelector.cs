using System;
using System.Collections.Generic;

namespace SVGSharpie.Css
{
    public sealed class CssSimpleSelector : CssSelector
    {
        /// <inheritdoc cref="CssSelector.SelectorType"/>
        public override CssSelectorType SelectorType => CssSelectorType.Simple;

        /// <summary>
        /// Gets the type of the simple selector
        /// </summary>
        public CssSimpleSelectorType SimpleSelectorType { get; }

        /// <summary>
        /// Gets the name of the selector which could represent the type or class name or the id, depending on the 
        /// <see cref="SimpleSelectorType"/> value.
        /// </summary>
        public CssQualifiedName Name { get; }

        /// <summary>
        /// Gets the collection of attribute selectors attached to the current selector
        /// </summary>
        public IReadOnlyList<CssAttributeSelector> AttributeSelectors { get; }

        /// <summary>
        /// Gets the collection of pseudo classes attached to the current selector
        /// </summary>
        public IReadOnlyList<CssPseudoClass> PseudoClasses { get; }

        public override CssSpecificity Specificity =>
            new CssSpecificity(
                SimpleSelectorType == CssSimpleSelectorType.IdSelector ? 1 : 0,
                (SimpleSelectorType == CssSimpleSelectorType.ClassSelector ? 1 : 0) + AttributeSelectors.Count + PseudoClasses.Count,
                SimpleSelectorType == CssSimpleSelectorType.TypeSelector ? 1 : 0 /* +PseudoElements.Length */
            );

        public CssSimpleSelector(CssSimpleSelectorType simpleSelectorType, CssQualifiedName name, CssAttributeSelector[] attributeSelectors, CssPseudoClass[] pseudoClasses)
        {
            SimpleSelectorType = simpleSelectorType;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AttributeSelectors = attributeSelectors ?? throw new ArgumentNullException(nameof(attributeSelectors));
            PseudoClasses = pseudoClasses ?? throw new ArgumentNullException(nameof(pseudoClasses));
        }

        /// <inheritdoc cref="CssSelector.Accept"/>
        public override void Accept(CssSelectorVisitor visitor)
            => visitor.VisitSimpleSelector(this);

        /// <inheritdoc cref="CssSelector.Accept"/>
        public override TResult Accept<TResult>(CssSelectorVisitor<TResult> visitor)
            => visitor.VisitSimpleSelector(this);

        public override string ToString() => $"{SimpleSelectorType} {Name}";
    }
}