namespace SVGSharpie
{
    /// <summary>
    /// Represents a visitor that only visits the single <see cref="SvgPathSeg">segment</see> passed into its Visit method.
    /// </summary>
    public class SvgPathSegVisitor
    {
        public virtual void Visit(SvgPathSeg segment)
            => segment?.Accept(this);

        public virtual void DefaultVisit(SvgPathSeg segment)
        {
            // nop
        }

        public virtual void VisitClosePath(SvgPathSegClosePath segment)
            => DefaultVisit(segment);

        public virtual void VisitMovetoAbs(SvgPathSegMovetoAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitMovetoRel(SvgPathSegMovetoRel segment)
            => DefaultVisit(segment);

        public virtual void VisitLinetoAbs(SvgPathSegLinetoAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitLinetoRel(SvgPathSegLinetoRel segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoCubicRel(SvgPathSegCurvetoCubicRel segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoQuadraticRel(SvgPathSegCurvetoQuadraticRel segment)
            => DefaultVisit(segment);

        public virtual void VisitArcAbs(SvgPathSegArcAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitArcRel(SvgPathSegArcRel segment)
            => DefaultVisit(segment);

        public virtual void VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitLinetoHorizontalRel(SvgPathSegLinetoHorizontalRel segment)
            => DefaultVisit(segment);

        public virtual void VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitLinetoVerticalRel(SvgPathSegLinetoVerticalRel segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoCubicSmoothRel(SvgPathSegCurvetoCubicSmoothRel segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment)
            => DefaultVisit(segment);

        public virtual void VisitCurvetoQuadraticSmoothRel(SvgPathSegCurvetoQuadraticSmoothRel segment)
            => DefaultVisit(segment);

    }

    /// <summary>
    /// Represents a visitor that only visits the single <see cref="SvgPathSeg">segment</see> passed into its Visit method.
    /// </summary>
    public class SvgPathSegVisitor<TResult>
    {
        public virtual TResult Visit(SvgPathSeg segment)
            => segment != null ? segment.Accept(this) : default(TResult);

        public virtual TResult DefaultVisit(SvgPathSeg segment)
            => default(TResult);

        public virtual TResult VisitClosePath(SvgPathSegClosePath segment)
            => DefaultVisit(segment);

        public virtual TResult VisitMovetoAbs(SvgPathSegMovetoAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitMovetoRel(SvgPathSegMovetoRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitLinetoAbs(SvgPathSegLinetoAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitLinetoRel(SvgPathSegLinetoRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoCubicRel(SvgPathSegCurvetoCubicRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoQuadraticRel(SvgPathSegCurvetoQuadraticRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitArcAbs(SvgPathSegArcAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitArcRel(SvgPathSegArcRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitLinetoHorizontalRel(SvgPathSegLinetoHorizontalRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitLinetoVerticalRel(SvgPathSegLinetoVerticalRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoCubicSmoothRel(SvgPathSegCurvetoCubicSmoothRel segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment)
            => DefaultVisit(segment);

        public virtual TResult VisitCurvetoQuadraticSmoothRel(SvgPathSegCurvetoQuadraticSmoothRel segment)
            => DefaultVisit(segment);

    }
}