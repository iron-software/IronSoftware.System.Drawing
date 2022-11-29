using System;
using System.Collections.Generic;

namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a structural pseudo-class
    /// </summary>
    public sealed class CssStructuralPseudoClass : CssPseudoClass
    {
        public override CssPseudoClassType PseudoClassType => CssPseudoClassType.Structural;

        /// <summary>
        /// Gets the structural pseudo-class type of the current pseudo-class
        /// </summary>
        public CssStructuralPseudoClassType StructuralPseudoClassType { get; }

        /// <summary>
        /// Gets the list of selectors of the current pseudo-class
        /// </summary>
        public IReadOnlyList<CssSelector> Selectors { get; }

        public CssStructuralPseudoClass(CssSelectorList selectors, CssStructuralPseudoClassType pseudoClassType)
        {
            Selectors = selectors ?? throw new ArgumentNullException(nameof(selectors));
            StructuralPseudoClassType = pseudoClassType;
        }

        internal static bool TryConvertToStructuralPseudoClassType(string name, out CssStructuralPseudoClassType pseudoClassType)
        {
            switch (name)
            {
                case "first-child":
                    pseudoClassType = CssStructuralPseudoClassType.FirstChild;
                    return true;
                case "last-child":
                    pseudoClassType = CssStructuralPseudoClassType.LastChild;
                    return true;
            }
            pseudoClassType = CssStructuralPseudoClassType.Empty;
            return false;
        }
    }
}