namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegCurvetoQuadraticRel interface corresponds to a "relative quadratic Bézier curveto" (q) path data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticRel : SvgPathSeg
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

        public SvgPathSegCurvetoQuadraticRel(float x1, float y1, float x, float y)
        {
            X1 = x1;
            Y1 = y1;
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.CurvetoQuadraticRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "q";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitCurvetoQuadraticRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitCurvetoQuadraticRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegCurvetoQuadraticRel(X1, Y1, X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X1} {Y1} {X} {Y}";
    }
}