using System.Collections.Generic;

namespace SVGSharpie
{
    /// <summary>
    /// Represents a paint server which linearly interpolates a color through two or more color stops
    /// </summary>
    public sealed class SvgLinearGradientPaintServer : SvgGradientPaintServer
    {
        /// <summary>
        /// Gets the starting x component of gradient vector for the current linear gradient
        /// </summary>
        public float X1 { get; }

        /// <summary>
        /// Gets the starting y component of gradient vector for the current linear gradient
        /// </summary>
        public float Y1 { get; }

        /// <summary>
        /// Gets the ending x component of gradient vector for the current linear gradient
        /// </summary>
        public float X2 { get; }

        /// <summary>
        /// Gets the ending y component of gradient vector for the current linear gradient
        /// </summary>
        public float Y2 { get; }

        /// <summary>
        /// Gets the coordinate system units for the current paint server
        /// </summary>
        public SvgUnitTypes Units { get; }

        /// <inheritdoc cref="SvgPaintServer.Accept"/>
        public override void Accept(SvgPaintServerVisitor visitor)
            => visitor.VisitLinearGradientPaintServer(this);

        /// <inheritdoc cref="SvgPaintServer.Accept"/>
        public override TResult Accept<TResult>(SvgPaintServerVisitor<TResult> visitor)
            => visitor.VisitLinearGradientPaintServer(this);

        internal SvgLinearGradientPaintServer(float x1, float y1, float x2, float y2, SvgUnitTypes units, IReadOnlyList<SvgGradientPaintServerColorStop> stops) : base(stops)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Units = units;
        }
    }
}