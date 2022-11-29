namespace SVGSharpie
{
    public interface ISvgPathSegCurve2
    {
        /// <summary>
        /// Gets the x-axis coordinate of the control point at the end of the curve
        /// </summary>
        float X2 { get; }

        /// <summary>
        /// Gets the y-axis coordinate of the control point at the end of the curve
        /// </summary>
        float Y2 { get; }
    }
}