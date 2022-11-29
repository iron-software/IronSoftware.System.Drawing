namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegLinetoVerticalRel interface corresponds to a "relative vertical lineto" (v) path data command.
    /// </summary>
    public sealed class SvgPathSegLinetoVerticalRel : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.LinetoVerticalRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "v";

        /// <summary>
        /// Gets or sets the relative Y coordinate for the end point of this path segment.
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegLinetoVerticalRel(float y)
        {
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitLinetoVerticalRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitLinetoVerticalRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegLinetoVerticalRel(Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter} {Y}";
    }
}