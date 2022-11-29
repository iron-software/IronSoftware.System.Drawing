using System;

namespace SVGSharpie.Utils
{
    internal sealed class SvgPathSegPenAndControlPointTrackerVisitor : SvgPathSegVisitor
    {
        public float CurPenX { get; private set; }

        public float CurPenY { get; private set; }

        public float LastX2 { get; private set; }

        public float LastY2 { get; private set; }

        public override void DefaultVisit(SvgPathSeg segment)
            => throw new InvalidOperationException($"Unsupported path segment '{segment?.GetType()}'");

        public override void VisitClosePath(SvgPathSegClosePath segment)
        {
            // nop...
        }

        public override void VisitArcRel(SvgPathSegArcRel segment)
            => UpdateRelXy(segment.X, segment.Y);

        public override void VisitArcAbs(SvgPathSegArcAbs segment)
            => UpdateAbsXy(segment.X, segment.Y);

        public override void VisitCurvetoCubicRel(SvgPathSegCurvetoCubicRel segment)
            => UpdateRelXy(segment.X, segment.Y, segment.X2, segment.Y2);

        public override void VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment)
            => UpdateAbsXy(segment.X, segment.Y, segment.X2, segment.Y2);

        public override void VisitCurvetoCubicSmoothRel(SvgPathSegCurvetoCubicSmoothRel segment)
            => UpdateRelXy(segment.X, segment.Y, segment.X2, segment.Y2);

        public override void VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment)
            => UpdateAbsXy(segment.X, segment.Y, segment.X2, segment.Y2);

        public override void VisitCurvetoQuadraticRel(SvgPathSegCurvetoQuadraticRel segment)
            => UpdateRelXy(segment.X, segment.Y, segment.X1, segment.Y1);

        public override void VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment)
            => UpdateAbsXy(segment.X, segment.Y, segment.X1, segment.Y1);

        public override void VisitCurvetoQuadraticSmoothRel(SvgPathSegCurvetoQuadraticSmoothRel segment)
        {
            var cx = 2 * CurPenX - LastX2;
            var cy = 2 * CurPenY - LastY2;
            UpdateRelXy(segment.X, segment.Y);
            LastX2 = cx;
            LastY2 = cy;
        }

        public override void VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment)
            => UpdateAbsXy(segment.X, segment.Y, 2 * CurPenX - LastX2, 2 * CurPenY - LastY2);

        public override void VisitLinetoHorizontalRel(SvgPathSegLinetoHorizontalRel segment)
            => UpdateRelXy(segment.X, CurPenY);

        public override void VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment)
            => UpdateAbsXy(segment.X, CurPenY);

        public override void VisitLinetoRel(SvgPathSegLinetoRel segment)
            => UpdateRelXy(segment.X, segment.Y);

        public override void VisitLinetoAbs(SvgPathSegLinetoAbs segment)
            => UpdateAbsXy(segment.X, segment.Y);

        public override void VisitLinetoVerticalRel(SvgPathSegLinetoVerticalRel segment)
            => UpdateRelXy(CurPenX, segment.Y);

        public override void VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment)
            => UpdateAbsXy(CurPenX, segment.Y);

        public override void VisitMovetoRel(SvgPathSegMovetoRel segment)
            => UpdateRelXy(segment.X, segment.Y);

        public override void VisitMovetoAbs(SvgPathSegMovetoAbs segment)
            => UpdateAbsXy(segment.X, segment.Y);

        private void UpdateRelXy(float x, float y, float? x2 = null, float? y2 = null)
        {
            x += CurPenX;
            y += CurPenY;
            LastX2 = x2.HasValue ? x2.Value + CurPenX : x;
            LastY2 = y2.HasValue ? y2.Value + CurPenY : y;
            CurPenX = x;
            CurPenY = y;
        }

        private void UpdateAbsXy(float x, float y, float? x2 = null, float? y2 = null)
        {
            LastX2 = x2.GetValueOrDefault(CurPenX = x);
            LastY2 = y2.GetValueOrDefault(CurPenY = y);
        }
    }
}