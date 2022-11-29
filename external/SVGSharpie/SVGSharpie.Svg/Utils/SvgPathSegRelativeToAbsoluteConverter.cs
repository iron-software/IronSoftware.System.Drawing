using System;

namespace SVGSharpie.Utils
{
    /// <inheritdoc />
    /// <summary>
    /// Transforms all relative path segment commands to their absolute command equivalents.
    /// For example a <see cref="T:SVGSharpie.SvgPathSegMovetoRel" /> will be transformed into a 
    /// <see cref="M:SVGSharpie.Utils.SvgPathSegRelativeToAbsoluteConverter.VisitMovetoAbs(SVGSharpie.SvgPathSegMovetoAbs)" />.
    /// </summary>
    internal sealed class SvgPathSegRelativeToAbsoluteConverter : SvgPathSegVisitor<SvgPathSeg>
    {
        private float _x;
        private float _y;

        public override SvgPathSeg DefaultVisit(SvgPathSeg segment)
            => throw new InvalidOperationException($"Unsupported path segment '{segment?.GetType()}'");

        public override SvgPathSeg VisitClosePath(SvgPathSegClosePath segment)
            => segment;

        public override SvgPathSeg VisitArcRel(SvgPathSegArcRel segment)
            => new SvgPathSegArcAbs(segment.RadiusX, segment.RadiusY, segment.Angle, segment.LargeArcFlag, segment.SweepFlag, _x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitArcAbs(SvgPathSegArcAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        public override SvgPathSeg VisitCurvetoCubicRel(SvgPathSegCurvetoCubicRel segment)
            => new SvgPathSegCurvetoCubicAbs(_x + segment.X1, _y + segment.Y1, _x + segment.X2, _y + segment.Y2, _x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        public override SvgPathSeg VisitCurvetoCubicSmoothRel(SvgPathSegCurvetoCubicSmoothRel segment)
            => new SvgPathSegCurvetoCubicSmoothAbs(_x + segment.X2, _y + segment.Y2, _x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        public override SvgPathSeg VisitCurvetoQuadraticRel(SvgPathSegCurvetoQuadraticRel segment)
            => new SvgPathSegCurvetoQuadraticAbs(_x + segment.X1, _y + segment.Y1, _x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        public override SvgPathSeg VisitCurvetoQuadraticSmoothRel(SvgPathSegCurvetoQuadraticSmoothRel segment)
            => new SvgPathSegCurvetoQuadraticSmoothAbs(_x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        public override SvgPathSeg VisitLinetoHorizontalRel(SvgPathSegLinetoHorizontalRel segment)
            => new SvgPathSegLinetoHorizontalAbs(_x += segment.X);

        public override SvgPathSeg VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment)
            => UpdateXyReturn(segment.X, _y, segment);

        public override SvgPathSeg VisitLinetoRel(SvgPathSegLinetoRel segment)
            => new SvgPathSegLinetoAbs(_x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitLinetoAbs(SvgPathSegLinetoAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        public override SvgPathSeg VisitLinetoVerticalRel(SvgPathSegLinetoVerticalRel segment)
            => new SvgPathSegLinetoVerticalAbs(_y += segment.Y);

        public override SvgPathSeg VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment)
            => UpdateXyReturn(_x, segment.Y, segment);

        public override SvgPathSeg VisitMovetoRel(SvgPathSegMovetoRel segment)
            => new SvgPathSegMovetoAbs(_x += segment.X, _y += segment.Y);

        public override SvgPathSeg VisitMovetoAbs(SvgPathSegMovetoAbs segment)
            => UpdateXyReturn(segment.X, segment.Y, segment);

        private SvgPathSeg UpdateXyReturn(float x, float y, SvgPathSeg segment)
        {
            _x = x;
            _y = y;
            return segment;
        }
    }
}
