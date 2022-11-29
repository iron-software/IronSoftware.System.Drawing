namespace SVGSharpie.Css
{
    public enum CssPseudoClassType
    {
        Functional,
        Linguistic,
        Location,
        UserAction,
        TimeDimensional,
        Input,
        Structural,

        Unknown
    }

    public enum CssUserActionPseudoClassType
    {
        Hover,
        Active,
        Focus,
        FocusRing,
        FocusWithin,
        Drop
    }

    public enum CssStructuralPseudoClassType
    {
        Root,
        Empty,
        Blank,
        NThChild,
        NThLastChild,
        FirstChild,
        LastChild,
        OnlyChild,
        NThOfType,
        NthLastOfType,
        FirstOfType,
        LastOfType,
        OnlyOfType,

        // Grid structural

        NThColumn,
        NThLastColumn
    }
    
    public enum CssInputPseudoClassType
    {
        Enabled,
        Disabled,
        ReadOnly,
        ReadWrite,
        PlaceholderShown,
        Default,
        Checked,
        Indeterminate,
        Valid,
        Invalid,
        InRange,
        OutOfRange,
        Required,
        Optional,
        UserInvalid
    }

    public enum CssFunctionalPseudoClassType
    {
        /// <summary>
        /// The negation pseudo-class, :not(), is a functional pseudo-class taking a selector list as an argument. 
        /// It represents an element that is not represented by its argument.
        /// </summary>
        Not,
        /// <summary>
        /// The matches-any pseudo-class, :matches(), is a functional pseudo-class taking a selector list as its 
        /// argument. It represents an element that is represented by its argument.
        /// </summary>
        Matches,
        /// <summary>
        /// The relational pseudo-class, :has(), is a functional pseudo-class taking a relative selector list as an 
        /// argument. It represents an element if any of the relative selectors, when absolutized and evaluated with 
        /// the element as the :scope elements, would match at least one element.
        /// </summary>
        Has
    }
}