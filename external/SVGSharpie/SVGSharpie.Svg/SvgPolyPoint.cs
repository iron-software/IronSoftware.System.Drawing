namespace SVGSharpie
{
    /// <summary>
    /// Represents a point used by the <see cref="SvgPolylineElement"/> and <see cref="SvgPolygonElement"/>.
    /// Coordinate values are in the user coordinate system.
    /// </summary>
    public struct SvgPolyPoint
    {
        /// <summary>
        /// Gets or sets the x-axis coordinate of the point, in the user coordinate system.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the point, in the user coordinate system.
        /// </summary>
        public float Y { get; set; }

        public SvgPolyPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X},{Y}";
    }
}