namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegMovetoAbs interface corresponds to an "absolute lineto" (L) path data command.
    /// </summary>
    public sealed class SvgPathSegLinetoAbs : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.LineToAbs;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "L";

        /// <summary>
        /// Gets or sets the absolute X coordinate for the end point of this path segment.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the absolute Y coordinate for the end point of this path segment.
        /// </summary>
        public float Y { get; set; }

        public SvgPathSegLinetoAbs(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitLinetoAbs(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitLinetoAbs(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegLinetoAbs(X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{X} {Y}";
    }
}