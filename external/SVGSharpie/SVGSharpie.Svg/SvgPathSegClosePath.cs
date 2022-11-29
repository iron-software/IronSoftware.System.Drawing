namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegClosePath interface corresponds to a "closepath" (z) path data command.
    /// </summary>
    public sealed class SvgPathSegClosePath : SvgPathSeg
    {
        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.ClosePath;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "z";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitClosePath(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitClosePath(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegClosePath();

        public override string ToString() => PathSegTypeAsLetter;
    }
}