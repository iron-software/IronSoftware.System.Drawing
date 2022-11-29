namespace SVGSharpie
{
    /// <summary>
    /// Represents a visitor that only visits the single <see cref="SvgElement">Node</see> passed into its Visit method.
    /// </summary>
    public class SvgPaintServerVisitor
    {
        public virtual void DefaultVisit(SvgPaintServer paintServer)
        {
            // nop
        }

        public virtual void VisitGradientPaintServer(SvgGradientPaintServer paintServer)
            => DefaultVisit(paintServer);

        public virtual void VisitSolidColorPaintServer(SvgSolidColorPaintServer paintServer)
            => DefaultVisit(paintServer);

        public virtual void VisitLinearGradientPaintServer(SvgLinearGradientPaintServer paintServer)
            => VisitGradientPaintServer(paintServer);

        public virtual void VisitRadialGradientPaintServer(SvgRadialGradientPaintServer paintServer)
            => VisitGradientPaintServer(paintServer);
    }
    
    /// <summary>
    /// Represents a visitor that only visits the single <see cref="SvgElement">Node</see> passed into its Visit method.
    /// </summary>
    public class SvgPaintServerVisitor<TResult>
    {
        public virtual TResult DefaultVisit(SvgPaintServer paintServer)
            => default(TResult);

        public virtual TResult VisitGradientPaintServer(SvgGradientPaintServer paintServer)
            => DefaultVisit(paintServer);

        public virtual TResult VisitSolidColorPaintServer(SvgSolidColorPaintServer paintServer)
            => DefaultVisit(paintServer);

        public virtual TResult VisitLinearGradientPaintServer(SvgLinearGradientPaintServer paintServer)
            => VisitGradientPaintServer(paintServer);

        public virtual TResult VisitRadialGradientPaintServer(SvgRadialGradientPaintServer paintServer)
            => VisitGradientPaintServer(paintServer);
    }
}