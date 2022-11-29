using System;
using System.Collections.Generic;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a list of of <see cref="T:SVGSharpie.SvgPolyPoint">points</see>
    /// </summary>
    public sealed class SvgPolyPointList : List<SvgPolyPoint>
    {
        public SvgPolyPointList()
        {
        }

        public SvgPolyPointList(IEnumerable<SvgPolyPoint> collection)
            :base(collection)
        {
        }

        public override string ToString() => string.Join(" ", this);

        internal SvgPolyPointList DeepClone()
            => new SvgPolyPointList(this);

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        internal SvgRect? GetBBox()
        {
            if (Count == 0)
            {
                return null;
            }
            var firstPoint = this[0];
            var minX = firstPoint.X;
            var minY = firstPoint.Y;
            var maxX = firstPoint.X;
            var maxY = firstPoint.Y;

            for (var i = 1; i < Count; i++)
            {
                var p = this[i];
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }

            return new SvgRect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}