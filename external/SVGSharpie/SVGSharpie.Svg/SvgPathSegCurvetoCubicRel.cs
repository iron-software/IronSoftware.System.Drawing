namespace SVGSharpie
{
    /// <inheritdoc cref="SvgPathSeg" />
    /// <summary>
    /// The SVGPathSegCurvetoCubicRel interface corresponds to a "relative cubic Bézier curveto" (c) path data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoCubicRel : SvgPathSeg, ISvgPathSegCurve1, ISvgPathSegCurve2
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

        public SvgPathSegCurvetoCubicRel(float x1, float y1, float x2, float y2, float x, float y)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.CurvetoCubicRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "c";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitCurvetoCubicRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitCurvetoCubicRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegCurvetoCubicRel(X1, Y1, X2, Y2, X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X1} {Y1} {X2} {Y2} {X} {Y}";
    }
}