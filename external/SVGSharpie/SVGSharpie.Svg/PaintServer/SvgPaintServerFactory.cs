using System;
using System.Collections.Generic;
using System.Linq;

namespace SVGSharpie
{
    internal static class SvgPaintServerFactory
    {
        /// <summary>
        /// Creates the paint server to use when painting the specified element
        /// </summary>
        /// <param name="elementToPaint">the element being painted</param>
        /// <param name="paint">the paint from which to create the paint server from</param>
        /// <param name="currentColor">the color value to use when <see cref="SvgPaintType"/> is CurrentColor</param>
        /// <returns>paint server or null if the <see cref="SvgPaintType"/> is <see cref="SvgPaintType.None"/></returns>
        public static SvgPaintServer CreatePaintServer(SvgElement elementToPaint, SvgPaint paint, SvgColor currentColor, float opacity)
        {
            if (paint == null) throw new ArgumentNullException(nameof(paint));
            if (elementToPaint == null) throw new ArgumentNullException(nameof(elementToPaint));
            switch (paint.PaintType)
            {
                case SvgPaintType.None:
                    return null;
                case SvgPaintType.CurrentColor:
                    return new SvgSolidColorPaintServer(currentColor, opacity);
                case SvgPaintType.ExplicitColor:
                    return new SvgSolidColorPaintServer(paint.Color, opacity);
                case SvgPaintType.IRIReference:
                    return CreatePaintServerFromReference(elementToPaint, paint.Reference);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static SvgPaintServer CreatePaintServerFromReference(SvgElement elementToPaint, string reference)
        {
            if (!reference.StartsWith("#"))
            {
                throw new NotSupportedException($"Expected reference to start with '#' but got '{reference}'");
            }
            var referenced = elementToPaint.ParentSvg.GetElementById(reference.Substring(1));
            if (referenced == null)
            {
                throw new Exception($"Referenced element '{reference}' not found");
            }
            if (referenced is SvgLinearGradientElement linear)
            {
                return CreateLinearGradientPaintServer(elementToPaint, linear);
            }
            if (referenced is SvgRadialGradientElement radial)
            {
                return CreateRadialGradientPaintServer(elementToPaint, radial);
            }
            throw new NotSupportedException($"Unsupported paint server element '{reference}'");
        }

        private static SvgPaintServer CreateRadialGradientPaintServer(SvgElement elementToPaint, SvgRadialGradientElement radial)
        {
            var stops = ConvertStops(radial.Stops);
            if (radial.GradientTransform.Count > 0)
            {
                throw new NotImplementedException($"gradient '{radial.GradientTransform}' transformations");
            }

            var bbox = elementToPaint.GetBBox();
            var width = bbox?.Width ?? 0;
            var height = bbox?.Height ?? 0;

            var cx = radial.CircleX?.GetAbsoluteValue(width) ?? (width * 0.5f);
            var cy = radial.CircleY?.GetAbsoluteValue(height) ?? (height * 0.5f);
            var cr = radial.CircleRadius?.GetAbsoluteValue(width) ?? (width * 0.5f);
            var fx = radial.FocalX?.GetAbsoluteValue(width) ?? cx;
            var fy = radial.FocalY?.GetAbsoluteValue(height) ?? cy;

            return new SvgRadialGradientPaintServer(cx, cy, cr, fx, fy, radial.GradientUnits, stops);
        }

        private static SvgPaintServer CreateLinearGradientPaintServer(SvgElement elementToPaint, SvgLinearGradientElement linear)
        {
            var stops = ConvertStops(linear.Stops);
            if (linear.GradientTransform.Count > 0)
            {
                throw new NotImplementedException($"gradient '{linear.GradientTransform}' transformations");
            }

            var bbox = elementToPaint.GetBBox();
            var width = bbox?.Width ?? 0;
            var height = bbox?.Height ?? 0;

            float? GetRelativeValue(SvgLength? length)
            {
                if (length == null) return null;
                var lengthType = length.Value.LengthType;
                if (lengthType != SvgLengthType.Number && lengthType != SvgLengthType.Percentage)
                {
                    throw new Exception($"Expected linear gradient value to be number or percent but got '{length}'");
                }
                return length.Value.GetAbsoluteValue(1);
            }

            var x1 = (GetRelativeValue(linear.X1) ?? 0) * width;
            var y1 = (GetRelativeValue(linear.Y1) ?? 0) * height;
            var x2 = (GetRelativeValue(linear.X2) ?? 1) * width;
            var y2 = (GetRelativeValue(linear.Y2) ?? 0) * height;

            if (Math.Abs(x1 - x2) < float.Epsilon && Math.Abs(y1 - y2) < float.Epsilon)
            {
                if (stops.Count < 1)
                {
                    throw new Exception($"Linear gradient has no stops");
                }
                var lastStop = stops[stops.Count - 1];
                return new SvgSolidColorPaintServer(lastStop.Color, lastStop.Opacity);
            }

            return new SvgLinearGradientPaintServer(x1, y1, x2, y2, linear.GradientUnits, stops);
        }

        private static IReadOnlyList<SvgGradientPaintServerColorStop> ConvertStops(SvgStopElementList stopElements)
        {
            float? lastOffset = 0;

            SvgGradientPaintServerColorStop ConvertStop(SvgStopElement stopElement)
            {
                var clamped = Math.Max(0, Math.Min(1, stopElement.Offset ?? 0));
                if (lastOffset != null)
                {
                    clamped = Math.Max(clamped, lastOffset.Value);
                }
                lastOffset = clamped;
                return new SvgGradientPaintServerColorStop(clamped, stopElement.StopColor ?? SvgColor.Black, stopElement.StopOpacity);
            }

            return stopElements.Select(ConvertStop).ToArray();
        }
    }
}
