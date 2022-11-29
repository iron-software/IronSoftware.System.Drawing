using System;

namespace SVGSharpie.Utils
{
    /// <inheritdoc />
    /// <summary>
    /// Converts SVG shape elements (<see cref="SvgRectElement"/>, <see cref="SvgEllipseElement"/>, 
    /// <see cref="SvgCircleElement"/>, <see cref="SvgLineElement"/>) into their path segment equivalents.
    /// </summary>
    internal sealed class SvgShapeElementToPathSegListConverter : SvgElementVisitor<SvgPathSegList>
    {
        public override SvgPathSegList DefaultVisit(SvgElement element)
            => throw new NotSupportedException($"Element of type '{element?.GetType()}' can't be converted to a path segment");

        public override SvgPathSegList VisitPathElement(SvgPathElement element)
            => element.Segments;

        public override SvgPathSegList VisitRectElement(SvgRectElement element)
        {
            var result = new SvgPathSegList();

            var x = element.X?.Value ?? 0;
            var y = element.Y?.Value ?? 0;
            var rx = element.RadiusX;
            var ry = element.RadiusY;
            var w = element.Width?.Value ?? 0;
            var h = element.Height?.Value ?? 0;

            result.Add(new SvgPathSegMovetoAbs(x + rx, y));
            result.Add(new SvgPathSegLinetoHorizontalAbs(x + w - rx));
            result.Add(new SvgPathSegArcAbs(rx, ry, 0, false, true, x + w, y + ry));

            result.Add(new SvgPathSegLinetoAbs(x+ w, y + h - ry));
            result.Add(new SvgPathSegArcAbs(rx, ry, 0, false, true, x + w - rx, y + h));
            result.Add(new SvgPathSegLinetoAbs(x + rx, y + h));

            result.Add(new SvgPathSegArcAbs(rx, ry, 0, false, true, x, y + h - ry));
            result.Add(new SvgPathSegLinetoAbs(x, y + ry));
            result.Add(new SvgPathSegArcAbs(rx, ry, 0, false, true, x + rx, y));

            result.Add(new SvgPathSegClosePath());

            return result;
        }

        public override SvgPathSegList VisitEllipseElement(SvgEllipseElement element)
        {
            var cx = element.Cx?.Value ?? 0;
            var cy = element.Cy?.Value ?? 0;
            var rx = element.Rx?.Value ?? 0;
            var ry = element.Ry?.Value ?? 0;
            return CreateEllipsePaths(cx, cy, ry, rx);
        }

        public override SvgPathSegList VisitCircleElement(SvgCircleElement element)
        {
            var cx = element.Cx?.Value ?? 0;
            var cy = element.Cy?.Value ?? 0;
            var r = element.R?.Value ?? 0;
            return CreateEllipsePaths(cx, cy, r, r);
        }

        public override SvgPathSegList VisitLineElement(SvgLineElement element)
        {
            return new SvgPathSegList
            {
                new SvgPathSegMovetoAbs(element.X1?.Value ?? 0, element.Y1?.Value ?? 0),
                new SvgPathSegLinetoAbs(element.X2?.Value ?? 0, element.Y2?.Value ?? 0)
            };
        }

        public override SvgPathSegList VisitPolylineElement(SvgPolylineElement element)
            => CreatePointsPath(element.Points, false);

        public override SvgPathSegList VisitPolygonElement(SvgPolygonElement element)
            => CreatePointsPath(element.Points, true);

        private static SvgPathSegList CreatePointsPath(SvgPolyPointList points, bool closed)
        {
            var result = new SvgPathSegList(points.Count + (closed ? 1 : 0));
            if (points.Count == 0)
            {
                return result;
            }
            result.Add(new SvgPathSegMovetoAbs(points[0].X, points[0].Y));
            for (var i = 1; i < points.Count; i++)
            {
                var p = points[i];
                result.Add(new SvgPathSegLinetoAbs(p.X, p.Y));
            }
            if (closed)
            {
                result.Add(new SvgPathSegClosePath());
            }
            return result;
        }

        private static SvgPathSegList CreateEllipsePaths(float cx, float cy, float ry, float rx)
        {
            var result = new SvgPathSegList();
            const float kappa = 0.55228474983f; // (4 * ((Math.Sqrt(2) - 1) / 3));

            result.Add(new SvgPathSegMovetoAbs(cx, cy - ry));
            result.Add(new SvgPathSegCurvetoCubicAbs(cx + kappa * rx, cy - ry, cx + rx, cy - kappa * ry, cx + rx, cy));
            result.Add(new SvgPathSegCurvetoCubicAbs(cx + rx, cy + kappa * ry, cx + kappa * rx, cy + ry, cx, cy + ry));
            result.Add(new SvgPathSegCurvetoCubicAbs(cx - kappa * rx, cy + ry, cx - rx, cy + kappa * ry, cx - rx, cy));
            result.Add(new SvgPathSegCurvetoCubicAbs(cx - rx, cy - kappa * ry, cx - kappa * rx, cy - ry, cx, cy - ry));
            result.Add(new SvgPathSegClosePath());

            return result;
        }
    }
}
