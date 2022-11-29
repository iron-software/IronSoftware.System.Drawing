namespace SVGSharpie
{
    public enum SvgPaintType
    {
        /// <summary>
        /// Indicates that no paint is applied.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that painting is done using the current animated value of the color specified by the ‘color’ property.
        /// </summary>
        CurrentColor,
        /// <summary>
        /// Indicates that an explicit color is specified
        /// </summary>
        ExplicitColor,
        /// <summary>
        /// Indicates that an IRI Reference is specified
        /// </summary>
        IRIReference
    }
}