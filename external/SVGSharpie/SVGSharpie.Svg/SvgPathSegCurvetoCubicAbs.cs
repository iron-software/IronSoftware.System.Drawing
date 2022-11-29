namespace SVGSharpie
{
    /// <inheritdoc cref="SvgPathSeg" />
    /// <summary>
    /// The SVGPathSegCurvetoCubicAbs interface corresponds to an "absolute cubic Bézier curveto" (C) path data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoCubicAbs : SvgPathSeg, ISvgPathSegCurve1, ISvgPathSegCurve2
    {
        /// <summary>
        /// Gets the x-axis coordinate of the control point at the beginning of the curve
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// Gets the y-axis coordinate of the control point at the beginning of the curve
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// Gets the x-axis coordinate of the control point at the end of the curve
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// Gets the y-axis coordinate of the control point at the end of the curve
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// Gets the x-axis coordinate of the point to draw the curve to
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets the y-axis coordinate of the point to draw the curve to
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegCurvetoCubicAbs(float x1, float y1, float x2, float y2, float x, float y)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.CurvetoCubicAbs;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "C";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitCurvetoCubicAbs(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitCurvetoCubicAbs(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegCurvetoCubicAbs(X1, Y1, X2, Y2, X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X1} {Y1} {X2} {Y2} {X} {Y}";
    }
}