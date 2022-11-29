using System;

namespace SVGSharpie.Utils
{
    /// <summary>
    /// Transforms all absolute path segment coordinates by a matrix, an error will be thrown for relative segments
    /// </summary>
    internal sealed class SvgPathSegMatrixTransformer : SvgPathSegVisitor<SvgPathSeg>
    {
        private readonly SvgMatrix _matrix;

        public SvgPathSegMatrixTransformer(SvgMatrix matrix)
        {
            _matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
        }

        public override SvgPathSeg DefaultVisit(SvgPathSeg segment)
        {
            throw new NotSupportedException($"Relative path segments are not supported, got '{segment?.GetType()}'");
        }

        public override SvgPathSeg VisitClosePath(SvgPathSegClosePath segment)
            => segment;

        public override SvgPathSeg VisitArcAbs(SvgPathSegArcAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegArcAbs(
                segment.RadiusX, segment.RadiusY, segment.Angle, segment.LargeArcFlag, segment.SweepFlag, tx, ty);
        }

        public override SvgPathSeg VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X1, segment.Y1, out var tx1, out var ty1);
            SvgMatrix.Multiply(_matrix, segment.X2, segment.Y2, out var tx2, out var ty2);
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegCurvetoCubicAbs(tx1, ty1, tx2, ty2, tx, ty);
        }

        public override SvgPathSeg VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X2, segment.Y2, out var tx2, out var ty2);
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegCurvetoCubicSmoothAbs(tx2, ty2, tx, ty);
        }

        public override SvgPathSeg VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X1, segment.Y1, out var tx1, out var ty1);
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegCurvetoQuadraticAbs(tx1, ty1, tx, ty);
        }

        public override SvgPathSeg VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegCurvetoQuadraticSmoothAbs(tx, ty);
        }

        public override SvgPathSeg VisitLinetoAbs(SvgPathSegLinetoAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegLinetoAbs(tx, ty);
        }

        public override SvgPathSeg VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X, 0, out var tx, out var _);
            return new SvgPathSegLinetoHorizontalAbs(tx);
        }

        public override SvgPathSeg VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment)
        {
            SvgMatrix.Multiply(_matrix, 0, segment.Y, out var _, out var ty);
            return new SvgPathSegLinetoVerticalAbs(ty);
        }

        public override SvgPathSeg VisitMovetoAbs(SvgPathSegMovetoAbs segment)
        {
            SvgMatrix.Multiply(_matrix, segment.X, segment.Y, out var tx, out var ty);
            return new SvgPathSegMovetoAbs(tx, ty);
        }
    }
}