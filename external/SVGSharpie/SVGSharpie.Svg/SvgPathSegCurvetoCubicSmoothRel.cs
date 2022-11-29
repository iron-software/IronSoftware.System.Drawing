namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegCurvetoCubicSmoothRel interface corresponds to a "relative cubic Bézier curveto" (s) path data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoCubicSmoothRel : SvgPathSeg, ISvgPathSegCurve2
    {
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

        public SvgPathSegCurvetoCubicSmoothRel(float x2, float y2, float x, float y)
        {
            X2 = x2;
            Y2 = y2;
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.CurvetoCubicSmoothRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "s";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitCurvetoCubicSmoothRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitCurvetoCubicSmoothRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegCurvetoCubicSmoothRel(X2, Y2, X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X2} {Y2} {X} {Y}";
    }
}