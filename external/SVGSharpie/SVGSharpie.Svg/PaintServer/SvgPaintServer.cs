namespace SVGSharpie
{
    /// <summary>
    /// SVG Paint servers allow the fill and stroke of an object to be defined elsewhere
    /// </summary>
    public abstract class SvgPaintServer
    {
        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="SvgPaintServerVisitor">visitor</see> specified.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        public abstract void Accept(SvgPaintServerVisitor visitor);

        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="SvgPaintServerVisitor">visitor</see> specified
        /// and returns the result.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        /// <returns>result of the visitor visit method call</returns>
        public abstract TResult Accept<TResult>(SvgPaintServerVisitor<TResult> visitor);

        internal SvgPaintServer()
        {
        }
    }
}