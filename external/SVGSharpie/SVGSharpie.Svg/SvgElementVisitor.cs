namespace SVGSharpie
{
    /// <summary>
    /// Represents a visitor that only visits the single <see cref="SvgElement">Node</see> passed into its Visit method.
    /// </summary>
    public class SvgElementVisitor
    {
        public virtual void Visit(SvgElement element)
            => element?.Accept(this);

        public virtual void DefaultVisit(SvgElement element)
        {
            // nop
        }

        public virtual void VisitGElement(SvgGElement element)
            => DefaultVisit(element);

        public virtual void VisitRectElement(SvgRectElement element)
            => DefaultVisit(element);

        public virtual void VisitPathElement(SvgPathElement element)
            => DefaultVisit(element);

        public virtual void VisitSvgElement(SvgSvgElement element)
            => DefaultVisit(element);

        public virtual void VisitTitleElement(SvgTitleElement element)
            => DefaultVisit(element);

        public virtual void VisitDescElement(SvgDescElement element)
            => DefaultVisit(element);

        public virtual void VisitCircleElement(SvgCircleElement element)
            => DefaultVisit(element);

        public virtual void VisitEllipseElement(SvgEllipseElement element)
            => DefaultVisit(element);

        public virtual void VisitLineElement(SvgLineElement element)
            => DefaultVisit(element);

        public virtual void VisitPolylineElement(SvgPolylineElement element)
            => DefaultVisit(element);

        public virtual void VisitPolygonElement(SvgPolygonElement element)
            => DefaultVisit(element);

        public virtual void VisitDefsElement(SvgDefsElement element)
            => DefaultVisit(element);

        public virtual void VisitUseElement(SvgUseElement element)
            => DefaultVisit(element);

        public virtual void VisitSymbolElement(SvgSymbolElement element)
            => DefaultVisit(element);

        public virtual void VisitStyleElement(SvgStyleElement element)
            => DefaultVisit(element);

        public virtual void VisitLinearGradientElement(SvgLinearGradientElement element)
            => DefaultVisit(element);

        public virtual void VisitRadialGradientElement(SvgRadialGradientElement element)
            => DefaultVisit(element);

        public virtual void VisitStopElement(SvgStopElement element)
            => DefaultVisit(element);

        public virtual void VisitPatternElement(SvgPatternElement element)
            => DefaultVisit(element);

        public virtual void VisitTextElement(SvgTextElement element)
            => DefaultVisit(element);

        public virtual void VisitTextSpanElement(SvgTextSpanElement element)
            => DefaultVisit(element);

        public virtual void VisitInlineTextSpanElement(SvgInlineTextSpanElement element)
            => DefaultVisit(element);

        
        public virtual void VisitMaskElement(SvgMaskElement element)
            => DefaultVisit(element);

        public virtual void VisitFilterElement(SvgFilterElement element)
            => DefaultVisit(element);

        public virtual void VisitTextPathElement(SvgTextPathElement element)
            => DefaultVisit(element);

        public virtual void VisitClipPathElement(SvgClipPathElement element)
            => DefaultVisit(element);
    }

    /// <summary>
    /// Represents a visitor that only visits the single <see cref="SvgElement">Node</see> passed into its Visit method.
    /// </summary>
    public class SvgElementVisitor<TResult>
    {
        public virtual TResult Visit(SvgElement element)
            => element != null ? element.Accept(this) : default(TResult);

        public virtual TResult DefaultVisit(SvgElement element)
            => default(TResult);

        public virtual TResult VisitGElement(SvgGElement element)
            => DefaultVisit(element);

        public virtual TResult VisitRectElement(SvgRectElement element)
            => DefaultVisit(element);

        public virtual TResult VisitPathElement(SvgPathElement element)
            => DefaultVisit(element);

        public virtual TResult VisitSvgElement(SvgSvgElement element)
            => DefaultVisit(element);

        public virtual TResult VisitTitleElement(SvgTitleElement element)
            => DefaultVisit(element);

        public virtual TResult VisitDescElement(SvgDescElement element)
            => DefaultVisit(element);

        public virtual TResult VisitCircleElement(SvgCircleElement element)
            => DefaultVisit(element);

        public virtual TResult VisitEllipseElement(SvgEllipseElement element)
            => DefaultVisit(element);

        public virtual TResult VisitLineElement(SvgLineElement element)
            => DefaultVisit(element);

        public virtual TResult VisitPolylineElement(SvgPolylineElement element)
            => DefaultVisit(element);

        public virtual TResult VisitPolygonElement(SvgPolygonElement element)
            => DefaultVisit(element);

        public virtual TResult VisitDefsElement(SvgDefsElement element)
            => DefaultVisit(element);

        public virtual TResult VisitUseElement(SvgUseElement element)
            => DefaultVisit(element);

        public virtual TResult VisitSymbolElement(SvgSymbolElement element)
            => DefaultVisit(element);

        public virtual TResult VisitStyleElement(SvgStyleElement element)
            => DefaultVisit(element);

        public virtual TResult VisitLinearGradientElement(SvgLinearGradientElement element)
            => DefaultVisit(element);

        public virtual TResult VisitRadialGradientElement(SvgRadialGradientElement element)
            => DefaultVisit(element);

        public virtual TResult VisitStopElement(SvgStopElement element)
            => DefaultVisit(element);

        public virtual TResult VisitPatternElement(SvgPatternElement element)
            => DefaultVisit(element);

        public virtual TResult VisitTextElement(SvgTextElement element)
            => DefaultVisit(element);

        public virtual TResult VisitTextSpanElement(SvgTextSpanElement element)
            => DefaultVisit(element);

        public virtual TResult VisitInlineTextSpanElement(SvgInlineTextSpanElement element)
            => DefaultVisit(element);
        

        public virtual TResult VisitMaskElement(SvgMaskElement element)
            => DefaultVisit(element);

        public virtual TResult VisitFilterElement(SvgFilterElement element)
            => DefaultVisit(element);

        public virtual TResult VisitTextPathElement(SvgTextPathElement element)
            => DefaultVisit(element);

        public virtual TResult VisitClipPathElement(SvgClipPathElement element)
            => DefaultVisit(element);
    }

    public class SvgElementWalker : SvgElementVisitor
    {
        public override void DefaultVisit(SvgElement element)
            => VisitChildren(element);

        /// <summary>
        /// Visits all the children of the specified element
        /// </summary>
        /// <param name="element">the element of which to visit their children</param>
        protected virtual void VisitChildren(SvgElement element)
        {
            if (element == null) return;
            foreach (var child in element.GetChildren())
            {
                Visit(child);
            }
        }
    }
}