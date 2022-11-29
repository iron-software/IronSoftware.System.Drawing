namespace SVGSharpie
{
    /// <summary>
    /// Represents a paint server that provides a single color with opacity
    /// </summary>
    public sealed class SvgSolidColorPaintServer : SvgPaintServer
    {
        /// <summary>
        /// Gets the solid color to paint with
        /// </summary>
        public SvgColor Color { get; }

        /// <summary>
        /// Gets the opacity to paint with (0..1)
        /// </summary>
        public float Opacity { get; }

        public SvgSolidColorPaintServer(SvgColor color, float opacity)
        {
            Color = color;
            Opacity = opacity;
        }

        /// <inheritdoc cref="SvgPaintServer.Accept"/>
        public override void Accept(SvgPaintServerVisitor visitor)
            => visitor.VisitSolidColorPaintServer(this);

        /// <inheritdoc cref="SvgPaintServer.Accept"/>
        public override TResult Accept<TResult>(SvgPaintServerVisitor<TResult> visitor)
            => visitor.VisitSolidColorPaintServer(this);
    }
}