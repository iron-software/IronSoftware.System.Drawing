using System.Collections.Generic;

namespace SVGSharpie.Utils
{
    internal sealed class SvgElementHierarchyInitializer : SvgElementWalker
    {
        private readonly Stack<SvgElement> _hierarchy = new Stack<SvgElement>();

        public override void DefaultVisit(SvgElement element)
        {
            element.Parent = _hierarchy.Count > 0 ? _hierarchy.Peek() : null;
            _hierarchy.Push(element);
            {
                base.DefaultVisit(element);
            }
            _hierarchy.Pop();
        }
    }
}
