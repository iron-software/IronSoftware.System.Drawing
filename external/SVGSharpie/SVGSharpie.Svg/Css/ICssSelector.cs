namespace SVGSharpie.Css
{
    /// <summary>
    /// Represents a structure that can be used as a condition (e.g. in a CSS rule) that determines which elements a 
    /// selector matches in the document tree, or as a flat description of the HTML or XML fragment corresponding 
    /// to that structure.
    /// </summary>
    public interface ICssSelector
    {
        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="CssSelectorVisitor">visitor</see> specified.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        void Accept(CssSelectorVisitor visitor);

        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="CssSelectorVisitor">visitor</see> specified
        /// and returns the result.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        /// <returns>result of the visitor visit method call</returns>
        TResult Accept<TResult>(CssSelectorVisitor<TResult> visitor);
    }
}