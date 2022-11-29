namespace SVGSharpie
{
    /// <summary>
    /// The SVGPathSeg interface is a base interface that corresponds to a single command within a path data specification.
    /// </summary>
    public abstract class SvgPathSeg
    {
        /// <summary>
        /// Gets the type of the path segment, as specified by one of the constants defined on this interface.
        /// </summary>
        public abstract SvgPathSegType PathSegType { get; }

        /// <summary>
        /// Gets the type of the path segment, specified by the corresponding one character command name.
        /// </summary>
        public abstract string PathSegTypeAsLetter { get; }

        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="SvgPathSegVisitor">visitor</see> specified.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        public abstract void Accept(SvgPathSegVisitor visitor);

        /// <summary>
        /// Calls the appropriate Visit method on the <see cref="SvgPathSegVisitor">visitor</see> specified
        /// and returns the result.
        /// </summary>
        /// <param name="visitor">visitor to visit</param>
        /// <returns>result of the visitor visit method call</returns>
        public abstract TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract SvgPathSeg DeepClone();

        internal SvgPathSeg()
        {
        }
    }
}