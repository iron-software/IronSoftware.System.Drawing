using System;
using System.Collections.Generic;

namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a functional pseudo-class
    /// </summary>
    public sealed class CssFunctionalPseudoClass : CssPseudoClass
    {
        /// <inheritdoc cref="CssPseudoClass.PseudoClassType"/>
        public override CssPseudoClassType PseudoClassType => CssPseudoClassType.Functional;

        /// <summary>
        /// Gets the functional pseudo-class type of the current pseudo-class
        /// </summary>
        public CssFunctionalPseudoClassType FunctionalPseudoClassType { get; }

        /// <summary>
        /// Gets the list of selectors of the current pseudo-class
        /// </summary>
        public IReadOnlyList<CssSelector> Selectors { get; }

        public CssFunctionalPseudoClass(CssSelectorList selectors, CssFunctionalPseudoClassType pseudoClassType)
        {
            Selectors = selectors ?? throw new ArgumentNullException(nameof(selectors));
            FunctionalPseudoClassType = pseudoClassType;
        }

        internal static bool TryConvertToFunctionalPseudoClassType(string name, out CssFunctionalPseudoClassType pseudoClassType)
        {
            switch (name)
            {
                case "not":
                    pseudoClassType = CssFunctionalPseudoClassType.Not;
                    return true;
                case "matches":
                    pseudoClassType = CssFunctionalPseudoClassType.Matches;
                    return true;
                case "has":
                    pseudoClassType = CssFunctionalPseudoClassType.Has;
                    return true;
            }
            pseudoClassType = CssFunctionalPseudoClassType.Not;
            return false;
        }
    }
}