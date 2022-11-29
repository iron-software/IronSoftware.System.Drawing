namespace SVGSharpie
{
    public interface ISvgPathSegCurve1
    {
        /// <summary>
        /// Gets the x-axis coordinate of the control point at the beginning of the curve
        /// </summary>
        float X1 { get; }

        /// <summary>
        /// Gets the y-axis coordinate of the control point at the beginning of the curve
        /// </summary>
        float Y1 { get; }
    }
}