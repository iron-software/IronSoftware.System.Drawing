using System;

namespace SVGSharpie
{
    /// <summary>
    /// Represents a gradient color stop, a color value at an offset between 0..1
    /// </summary>
    public struct SvgGradientPaintServerColorStop
    {
        /// <summary>
        /// Gets the offset, clamped to 0..1, of the current stop
        /// </summary>
        public float Offset { get; }

        /// <summary>
        /// Gets the color of the current stop at the <see cref="Offset"/>
        /// </summary>
        public SvgColor Color { get; }

        /// <summary>
        /// Gets the opacity of the current stop at the <see cref="Offset"/>
        /// </summary>
        public float Opacity { get; }

        public SvgGradientPaintServerColorStop(float offset, SvgColor color, float opacity = 1)
        {
            Offset = Math.Max(0, Math.Min(1, offset));
            Color = color;
            Opacity = opacity;
        }
    }
}