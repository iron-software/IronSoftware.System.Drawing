namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegLinetoHorizontalAbs interface corresponds to an "absolute horizontal lineto" (H) path data command.
    /// </summary>
    public sealed class SvgPathSegLinetoHorizontalAbs : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.LinetoHorizontalAbs;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "H";

        /// <summary>
        /// Gets or sets the absolute X coordinate for the end point of this path segment.
        /// </summary>
        public float X { get; set; }

        public SvgPathSegLinetoHorizontalAbs(float x)
        {
            X = x;
        }

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitLinetoHorizontalAbs(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitLinetoHorizontalAbs(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegLinetoHorizontalAbs(X);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X}";
    }
}