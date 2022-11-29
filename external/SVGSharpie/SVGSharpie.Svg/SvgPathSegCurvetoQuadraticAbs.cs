namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegCurvetoQuadraticAbs interface corresponds to an "absolute quadratic Bézier curveto" (Q) path data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticAbs : SvgPathSeg
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
        /// Gets the x-axis coordinate of the point to draw the curve to
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets the y-axis coordinate of the point to draw the curve to
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegCurvetoQuadraticAbs(float x1, float y1, float x, float y)
        {
            X1 = x1;
            Y1 = y1;
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.CurvetoQuadraticAbs;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "Q";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitCurvetoQuadraticAbs(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitCurvetoQuadraticAbs(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegCurvetoQuadraticAbs(X1, Y1, X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X1} {Y1} {X} {Y}";
    }
}