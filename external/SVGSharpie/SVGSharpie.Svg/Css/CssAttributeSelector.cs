using System;

namespace SVGSharpie.Css
{
    /// <inheritdoc cref="ICssSelector" />
    /// <summary>
    /// Attribute selectors match an element if that element has an attribute that matches the attribute represented by the attribute selector.
    /// </summary>
    public struct CssAttributeSelector : ICssSelector
    {
        /// <summary>
        /// Gets the name of the attribute to match
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// Gets the value of the attribute to match as per the <see cref="Operation"/>
        /// </summary>
        public string AttributeValue { get; }

        /// <summary>
        /// Gets the type of match to perform on the attribute
        /// </summary>
        public CssAttributeSelectorOperation Operation { get; }

        /// <summary>
        /// Gets the original value type of the <see cref="AttributeValue"/>
        /// </summary>
        public CssAttributeSelectorValueType AttributeValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the current selector explicitly specified case insensitivity
        /// </summary>
        public bool ExplicitCaseInsensitive { get; }

        public CssAttributeSelector(string attributeName, string attributeValue, CssAttributeSelectorOperation operation, CssAttributeSelectorValueType attributeValueType, bool explicitCaseInsensitive)
        {
            AttributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            AttributeValue = attributeValue ?? throw new ArgumentNullException(nameof(attributeValue));
            Operation = operation;
            AttributeValueType = attributeValueType;
            ExplicitCaseInsensitive = explicitCaseInsensitive;
        }
        
        /// <inheritdoc cref="CssSelector.Accept"/>
        public void Accept(CssSelectorVisitor visitor)
            => visitor.VisitAttributeSelector(this);

        /// <inheritdoc cref="CssSelector.Accept"/>
        public TResult Accept<TResult>(CssSelectorVisitor<TResult> visitor)
            => visitor.VisitAttributeSelector(this);
    }
}