namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegCurvetoQuadraticSmoothRel interface corresponds to a "relative smooth cubic curveto" (t) path data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticSmoothRel : SvgPathSeg
    {
        /// <summary>
        /// Gets the x-axis coordinate of the point to draw the curve to
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets the y-axis coordinate of the point to draw the curve to
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegCurvetoQuadraticSmoothRel(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.CurvetoQuadraticSmoothRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "t";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitCurvetoQuadraticSmoothRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitCurvetoQuadraticSmoothRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegCurvetoQuadraticSmoothRel(X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X} {Y}";
    }
}