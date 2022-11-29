using System;
using System.Collections.Generic;

namespace SVGSharpie.Utils
{
    /// <summary>
    /// Transforms all non 'moveto' path segment into their <see cref="SvgPathSegCurvetoCubicAbs">cubic curve</see> counterparts
    /// </summary>
    internal sealed class SvgPathSegLinesAndCurvestoCubicCurvesConverter : SvgPathSegVisitor
    {
        public SvgPathSegList Result { get; } = new SvgPathSegList();

        public void Flush()
            => CloseSubPath();

        public override void VisitClosePath(SvgPathSegClosePath segment)
            => CloseSubPath(true);

        private void EmitMoveto(SvgPathSeg segment)
        {
            CloseSubPath();
            segment.Accept(_tracker);
            _builder.MoveTo(CurPenX, CurPenY);
        }

        private void EmitLineto(SvgPathSeg segment)
        {
            segment.Accept(_tracker);
            _builder.LineTo(CurPenX, CurPenY);
        }

        private void EmitCubicCurveto<T>(T segment)
            where T : SvgPathSeg, ISvgPathSegCurve1
        {
            segment.Accept(_tracker);
            _builder.CubicCurveTo(segment.X1, segment.Y1, LastX2, LastY2, CurPenX, CurPenY);
        }

        private void EmitSmoothCubicCurveto(SvgPathSeg segment)
        {
            // 8.3.6 The cubic Bézier curve commands
            // The first control point is assumed to be the reflection of the second control point on the previous command relative to the current point.

            var cx1 = 2 * CurPenX - LastX2;
            var cy1 = 2 * CurPenY - LastY2;
            segment.Accept(_tracker);
            _builder.CubicCurveTo(cx1, cy1, LastX2, LastY2, CurPenX, CurPenY);
        }

        private void EmitSmoothQuadraticCurveto(SvgPathSeg segment)
        {
            // 8.3.7 The quadratic Bézier curve commands
            // The control point is assumed to be the reflection of the control point on the previous command relative to the current point. 

            var cx = 2 * CurPenX - LastX2;
            var cy = 2 * CurPenY - LastY2;

            // quadratic to cubic
            var cx1 = CurPenX + 2.0f / 3.0f * (cx - CurPenX);
            var cy1 = CurPenY + 2.0f / 3.0f * (cy - CurPenY);

            segment.Accept(_tracker);
            var x = CurPenX;
            var y = CurPenY;

            var cx2 = x + 2.0f / 3.0f * (cx - x);
            var cy2 = y + 2.0f / 3.0f * (cy - y);

            _builder.CubicCurveTo(cx1, cy1, cx2, cy2, x, y);
        }

        private void EmitQuadraticCurveTo(SvgPathSeg segment)
        {
            var cx = CurPenX;
            var cy = CurPenY;
            segment.Accept(_tracker);
            _builder.QuadraticCurveTo(cx, cy, LastX2, LastY2, CurPenX, CurPenY);
        }

        public override void VisitMovetoAbs(SvgPathSegMovetoAbs segment) => EmitMoveto(segment);

        public override void VisitMovetoRel(SvgPathSegMovetoRel segment) => EmitMoveto(segment);

        public override void VisitLinetoAbs(SvgPathSegLinetoAbs segment) => EmitLineto(segment);

        public override void VisitLinetoRel(SvgPathSegLinetoRel segment) => EmitLineto(segment);

        public override void VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment) => EmitLineto(segment);

        public override void VisitLinetoHorizontalRel(SvgPathSegLinetoHorizontalRel segment) => EmitLineto(segment);

        public override void VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment) => EmitLineto(segment);

        public override void VisitLinetoVerticalRel(SvgPathSegLinetoVerticalRel segment) => EmitLineto(segment);

        public override void VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment) => EmitCubicCurveto(segment);

        public override void VisitCurvetoCubicRel(SvgPathSegCurvetoCubicRel segment) => EmitCubicCurveto(segment);

        public override void VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment) => EmitSmoothCubicCurveto(segment);

        public override void VisitCurvetoCubicSmoothRel(SvgPathSegCurvetoCubicSmoothRel segment) => EmitSmoothCubicCurveto(segment);

        public override void VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment) => EmitQuadraticCurveTo(segment);

        public override void VisitCurvetoQuadraticRel(SvgPathSegCurvetoQuadraticRel segment) => EmitQuadraticCurveTo(segment);

        public override void VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment) => EmitSmoothQuadraticCurveto(segment);

        public override void VisitCurvetoQuadraticSmoothRel(SvgPathSegCurvetoQuadraticSmoothRel segment) => EmitSmoothQuadraticCurveto(segment);

        public override void VisitArcAbs(SvgPathSegArcAbs segment)
        {
            EmitArc(segment.RadiusX, segment.RadiusY, segment.Angle, segment.LargeArcFlag, segment.SweepFlag, segment.X, segment.Y);
            segment.Accept(_tracker);
        }

        public override void VisitArcRel(SvgPathSegArcRel segment)
        {
            EmitArc(segment.RadiusX, segment.RadiusY, segment.Angle, segment.LargeArcFlag, segment.SweepFlag, CurPenX + segment.X, CurPenY + segment.Y);
            segment.Accept(_tracker);
        }

        private void EmitArc(float rx, float ry, float angle, bool largeArcFlag, bool sweepFlag, float x2, float y2)
        {
            // Ported from nanosvg (https://github.com/memononen/nanosvg) which itself was 
            // ported from canvg (https://code.google.com/p/canvg/)

            var rotx = angle / 180.0f * (float)Math.PI;
            var x1 = CurPenX;
            var y1 = CurPenY;

            const float pi = (float)Math.PI;

            float VecMag(float vecX, float vecY) => (float)Math.Sqrt(vecX * vecX + vecY * vecY);

            float VecRat(float vecUx, float vecUy, float vecVx, float vecVy)
                => (vecUx * vecVx + vecUy * vecVy) / (VecMag(vecUx, vecUy) * VecMag(vecVx, vecVy));

            float VecAng(float vecUx, float vecUy, float vecVx, float vecVy)
            {
                var r = VecRat(vecUx, vecUy, vecVx, vecVy);
                if (r < -1.0f) r = -1.0f;
                if (r > 1.0f) r = 1.0f;
                return (vecUx * vecVy < vecUy * vecVx ? -1.0f : 1.0f) * (float)Math.Acos(r);
            }

            void XFormPoint(float vecX, float vecY, float[] transform, out float outX, out float outY)
            {
                outX = vecX * transform[0] + vecY * transform[2] + transform[4];
                outY = vecX * transform[1] + vecY * transform[3] + transform[5];
            }

            void XFormVec(float vecX, float vecY, float[] transform, out float outX, out float outY)
            {
                outX = vecX * transform[0] + vecY * transform[2];
                outY = vecX * transform[1] + vecY * transform[3];
            }

            var dx = x1 - x2;
            var dy = y1 - y2;
            var d = (float)Math.Sqrt(dx * dx + dy * dy);
            if (d < 1e-6f || rx < 1e-6f || ry < 1e-6f)
            {
                // The arc degenerates to a line
                _builder.LineTo(x2, y2);
                return;
            }

            var sinrx = (float)Math.Sin(rotx);
            var cosrx = (float)Math.Cos(rotx);

            // Convert to center point parameterization.
            // http://www.w3.org/TR/SVG11/implnote.html#ArcImplementationNotes
            // 1) Compute x1', y1'
            var x1p = cosrx * dx / 2.0f + sinrx * dy / 2.0f;
            var y1p = -sinrx * dx / 2.0f + cosrx * dy / 2.0f;
            d = (x1p * x1p) / (rx * rx) + (y1p * y1p) / (ry * ry);
            if (d > 1)
            {
                d = (float)Math.Sqrt(d);
                rx *= d;
                ry *= d;
            }

            // 2) Compute cx', cy'
            var s = 0.0f;
            var sa = (rx * rx) * (ry * ry) - (rx * rx) * (y1p * y1p) - (ry * ry) * (x1p * x1p);
            var sb = (rx * rx) * (y1p * y1p) + (ry * ry) * (x1p * x1p);
            if (sa < 0.0f) sa = 0.0f;
            if (sb > 0.0f)
            {
                s = (float)Math.Sqrt(sa / sb);
            }
            if (largeArcFlag == sweepFlag)
            {
                s = -s;
            }
            var cxp = s * rx * y1p / ry;
            var cyp = s * -ry * x1p / rx;

            // 3) Compute cx,cy from cx',cy'
            var cx = (x1 + x2) / 2.0f + cosrx * cxp - sinrx * cyp;
            var cy = (y1 + y2) / 2.0f + sinrx * cxp + cosrx * cyp;

            // 4) Calculate theta1, and delta theta.
            var ux = (x1p - cxp) / rx;
            var uy = (y1p - cyp) / ry;
            var vx = (-x1p - cxp) / rx;
            var vy = (-y1p - cyp) / ry;
            var a1 = VecAng(1.0f, 0.0f, ux, uy);    // Initial angle
            var da = VecAng(ux, uy, vx, vy);        // Delta angle

            if (!sweepFlag && da > 0)
            {
                da -= 2 * pi;
            }
            else if (sweepFlag && da < 0)
            {
                da += 2 * pi;
            }

            // Approximate the arc using cubic spline segments.
            var t = new float[6];
            t[0] = cosrx;
            t[1] = sinrx;
            t[2] = -sinrx;
            t[3] = cosrx;
            t[4] = cx;
            t[5] = cy;

            // Split arc into max 90 degree segments.
            // The loop assumes an iteration per end point (including start and end), this +1.
            var ndivs = (int)(Math.Abs(da) / (pi * 0.5f) + 1.0f);
            var hda = (da / (float)ndivs) / 2.0f;
            var kappa = Math.Abs(4.0f / 3.0f * (1.0f - (float)Math.Cos(hda)) / (float)Math.Sin(hda));
            if (da < 0.0f)
            {
                kappa = -kappa;
            }

            float px = 0, py = 0, ptanx = 0, ptany = 0;

            for (var i = 0; i <= ndivs; i++)
            {
                var a = a1 + da * ((float)i / ndivs);
                dx = (float)Math.Cos(a);
                dy = (float)Math.Sin(a);
                XFormPoint(dx * rx, dy * ry, t, out var x, out var y); // position
                XFormVec(-dy * rx * kappa, dx * ry * kappa, t, out var tanx, out var tany); // tangent
                if (i > 0)
                {
                    _builder.CubicCurveTo(px + ptanx, py + ptany, x - tanx, y - tany, x, y);
                }
                px = x;
                py = y;
                ptanx = tanx;
                ptany = tany;
            }
        }

        private void CloseSubPath(bool close = false)
        {
            Result.AddRange(_builder.EmitCommands());
            if (close)
            {
                Result.Add(new SvgPathSegClosePath());
            }
            _builder.Reset();
        }

        private readonly SvgPathSegPenAndControlPointTrackerVisitor _tracker = new SvgPathSegPenAndControlPointTrackerVisitor();
        private readonly NormalizedCommandsBuilder _builder = new NormalizedCommandsBuilder();

        private float CurPenX => _tracker.CurPenX;
        private float CurPenY => _tracker.CurPenY;
        private float LastX2 => _tracker.LastX2;
        private float LastY2 => _tracker.LastY2;

        private struct PointF
        {
            public readonly float X;
            public readonly float Y;

            public PointF(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        /// <summary>
        /// Converts the various line to commands to cubic bezier curves
        /// </summary>
        private sealed class NormalizedCommandsBuilder
        {
            private readonly List<PointF> _points = new List<PointF>();

            public void MoveTo(float x, float y)
            {
                if (!EnsureInitialPoint(x, y))
                {
                    _points[_points.Count - 1] = new PointF(x, y);
                }
            }

            public void LineTo(float x, float y)
            {
                EnsureInitialPoint();
                var p = _points[_points.Count - 1];
                var dx = x - p.X;
                var dy = y - p.Y;
                _points.Add(new PointF(p.X + dx / 3.0f, p.Y + dy / 3.0f));
                _points.Add(new PointF(x - dx / 3.0f, y - dy / 3.0f));
                _points.Add(new PointF(x, y));
            }

            public void QuadraticCurveTo(float curPenX, float curPenY, float x1, float y1, float x, float y)
            {
                // quadratic to cubic
                var cx1 = curPenX + 2.0f / 3.0f * (x1 - curPenX);
                var cy1 = curPenY + 2.0f / 3.0f * (y1 - curPenY);
                var cx2 = x + 2.0f / 3.0f * (x1 - x);
                var cy2 = y + 2.0f / 3.0f * (y1 - y);
                CubicCurveTo(cx1, cy1, cx2, cy2, x, y);
            }

            public void CubicCurveTo(float x1, float y1, float x2, float y2, float x, float y)
            {
                EnsureInitialPoint();
                _points.Add(new PointF(x1, y1));
                _points.Add(new PointF(x2, y2));
                _points.Add(new PointF(x, y));
            }

            public IEnumerable<SvgPathSeg> EmitCommands()
            {
                if (_points.Count == 0)
                {
                    yield break;
                }
                yield return new SvgPathSegMovetoAbs(_points[0].X, _points[0].Y);
                for (var i = 1; i < _points.Count; i += 3)
                {
                    var p1 = _points[i];
                    var p2 = _points[i + 1];
                    var p = _points[i + 2];
                    yield return new SvgPathSegCurvetoCubicAbs(p1.X, p1.Y, p2.X, p2.Y, p.X, p.Y);
                }
            }

            public void Reset() => _points.Clear();

            private bool EnsureInitialPoint(float x = 0, float y = 0)
            {
                if (_points.Count == 0)
                {
                    _points.Add(new PointF(x, y));
                    return true;
                }
                return false;
            }
        }
    }
}