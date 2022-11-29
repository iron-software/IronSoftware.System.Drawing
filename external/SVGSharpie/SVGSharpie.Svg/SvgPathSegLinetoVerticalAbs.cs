namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegLinetoVerticalAbs interface corresponds to an "absolute vertical lineto" (V) path data command.
    /// </summary>
    public sealed class SvgPathSegLinetoVerticalAbs : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.LinetoVerticalAbs;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "V";

        /// <summary>
        /// Gets or sets the absolute Y coordinate for the end point of this path segment.
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegLinetoVerticalAbs(float y)
        {
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitLinetoVerticalAbs(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitLinetoVerticalAbs(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegLinetoVerticalAbs(Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter} {Y}";
    }
}