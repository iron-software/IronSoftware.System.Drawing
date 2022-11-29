using System;
using System.Collections.Generic;

namespace SVGSharpie.Css
{
    public sealed class CssStyleRule
    {
        /// <summary>
        /// Gets the collection of selectors of the current rule
        /// </summary>
        public IReadOnlyList<CssSelector> Selectors { get; }

        /// <summary>
        /// Gets the style properties of the current rule
        /// </summary>
        public IReadOnlyDictionary<string, CssStylePropertyValue> Properties { get; }

        public CssStyleRule(CssSelectorList selectors, IReadOnlyDictionary<string, CssStylePropertyValue> properties)
        {
            Selectors = selectors ?? throw new ArgumentNullException(nameof(selectors));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }
    }
}