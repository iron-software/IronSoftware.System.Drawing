namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a user action pseudo-class
    /// </summary>
    public sealed class CssUserActionPseudoClass : CssPseudoClass
    {
        public CssUserActionPseudoClassType UserActionPseudoClassType { get; }

        /// <summary>
        /// Gets the user action pseudo-class type of the current pseudo-class
        /// </summary>
        public override CssPseudoClassType PseudoClassType => CssPseudoClassType.UserAction;

        public CssUserActionPseudoClass(CssUserActionPseudoClassType userActionPseudoClassType)
        {
            UserActionPseudoClassType = userActionPseudoClassType;
        }

        internal static bool TryConvertToUserActionPseudoClassType(string name, out CssUserActionPseudoClassType pseudoClassType)
        {
            switch (name)
            {
                case "hover":
                    pseudoClassType = CssUserActionPseudoClassType.Hover;
                    return true;
                case "active":
                    pseudoClassType = CssUserActionPseudoClassType.Active;
                    return true;
                case "focus":
                    pseudoClassType = CssUserActionPseudoClassType.Focus;
                    return true;
                case "focus-ring":
                    pseudoClassType = CssUserActionPseudoClassType.FocusRing;
                    return true;
                case "focus-within":
                    pseudoClassType = CssUserActionPseudoClassType.FocusWithin;
                    return true;
                case "drop":
                    pseudoClassType = CssUserActionPseudoClassType.Drop;
                    return true;
            }
            pseudoClassType = CssUserActionPseudoClassType.Hover;
            return false;
        }
    }
}