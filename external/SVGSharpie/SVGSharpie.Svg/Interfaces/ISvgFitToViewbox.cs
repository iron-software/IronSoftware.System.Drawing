namespace SVGSharpie
{
    /// <summary>
    /// Defines attributes that apply to elements which have XML attributes ‘viewBox’ and ‘preserveAspectRatio’.
    /// </summary>
    public interface ISvgFitToViewbox
    {
        /// <summary>
        /// Gets the viewbox container to which graphics are stretched to
        /// </summary>
        SvgRect? ViewBox { get; }

        /// <summary>
        /// Gets the description of how to stretch graphics to fit a viewport
        /// </summary>
        SvgPreserveAspectRatio PreserveAspectRatio { get; }
    }
}