namespace SVGSharpie.Utils
{
    internal sealed class SvgUseElementRewriter : SvgElementVisitor<SvgElement>
    {
        public override SvgElement DefaultVisit(SvgElement element)
            => element;

        public override SvgElement VisitUseElement(SvgUseElement element)
        {
            return base.VisitUseElement(element);
        }
    }
}