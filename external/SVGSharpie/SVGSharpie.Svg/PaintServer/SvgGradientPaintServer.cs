using System;
using System.Collections.Generic;

namespace SVGSharpie
{
    /// <summary>
    /// Represents a paint server which interpolates a color through two or more color stops
    /// </summary>
    public abstract class SvgGradientPaintServer : SvgPaintServer
    {
        public IReadOnlyList<SvgGradientPaintServerColorStop> Stops { get; }

        internal SvgGradientPaintServer(IReadOnlyList<SvgGradientPaintServerColorStop> stops)
        {
            Stops = stops ?? throw new ArgumentNullException(nameof(stops));
        }
    }
}