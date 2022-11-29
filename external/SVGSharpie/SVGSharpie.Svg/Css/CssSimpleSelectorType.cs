namespace SVGSharpie.Css
{
    public enum CssSimpleSelectorType
    {
        /// <summary>
        /// A type selector is the name of a document language element type, and represents an instance of that 
        /// element type in the document tree.
        /// </summary>
        TypeSelector,
        /// <summary>
        /// The universal selector is a special type selector, that represents an element of any element type.
        /// </summary>
        UniversalSelector,
        /// <summary>
        /// The class selector is given as a full stop (. U+002E) immediately followed by an identifier. 
        /// It represents an element belonging to the class identified by the identifier, as defined by the 
        /// document language.
        /// </summary>
        ClassSelector,
        /// <summary>
        /// The ID selector consists of a “number sign” (U+0023, #) immediately followed by the ID value, which 
        /// must be a CSS identifier. An ID selector represents an element instance that has an identifier that 
        /// matches the identifier in the ID selector.
        /// </summary>
        IdSelector
    }
}