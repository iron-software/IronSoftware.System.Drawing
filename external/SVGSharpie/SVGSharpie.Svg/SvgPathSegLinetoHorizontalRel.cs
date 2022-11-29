namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegLinetoHorizontalAbs interface corresponds to a "relative horizontal lineto" (h) path data command.
    /// </summary>
    public sealed class SvgPathSegLinetoHorizontalRel : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.LinetoHorizontalRel;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "h";

        /// <summary>
        /// Gets or sets the relative X coordinate for the end point of this path segment.
        /// </summary>
        public float X { get; set; }

        public SvgPathSegLinetoHorizontalRel(float x)
        {
            X = x;
        }

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitLinetoHorizontalRel(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitLinetoHorizontalRel(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegLinetoHorizontalRel(X);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X}";
    }
}