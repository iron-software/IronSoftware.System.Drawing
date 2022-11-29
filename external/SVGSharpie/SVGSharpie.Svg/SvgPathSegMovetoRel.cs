namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegMovetoAbs interface corresponds to a "relative moveto" (m) path data command.
    /// </summary>
    public sealed class SvgPathSegMovetoRel : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.MoveToRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "m";

        /// <summary>
        /// Gets or sets the relative X coordinate for the end point of this path segment.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the relative Y coordinate for the end point of this path segment.
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegMovetoRel(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitMovetoRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitMovetoRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegMovetoRel(X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X} {Y}";
    }
}