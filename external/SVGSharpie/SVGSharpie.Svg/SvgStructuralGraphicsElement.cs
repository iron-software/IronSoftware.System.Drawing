using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SVGSharpie
{
    public abstract class SvgStructuralGraphicsElement : SvgGraphicsElement
    {
        internal override bool PartakesInRenderingTree => Children.Count > 0 && base.PartakesInRenderingTree;

        [XmlElement(typeof(SvgGElement))]
        [XmlElement(typeof(SvgDefsElement))]
        [XmlElement(typeof(SvgUseElement))]
        [XmlElement(typeof(SvgSymbolElement))]
        [XmlElement(typeof(SvgPathElement))]
        [XmlElement(typeof(SvgRectElement))]
        [XmlElement(typeof(SvgCircleElement))]
        [XmlElement(typeof(SvgEllipseElement))]
        [XmlElement(typeof(SvgLineElement))]
        [XmlElement(typeof(SvgPolylineElement))]
        [XmlElement(typeof(SvgPolygonElement))]
        [XmlElement(typeof(SvgTitleElement))]
        [XmlElement(typeof(SvgDescElement))]
        [XmlElement(typeof(SvgSvgElement))]
        [XmlElement(typeof(SvgStyleElement))]
        [XmlElement(typeof(SvgLinearGradientElement))]
        [XmlElement(typeof(SvgRadialGradientElement))]
        [XmlElement(typeof(SvgPatternElement))]
        [XmlElement(typeof(SvgTextElement))]
        [XmlElement(typeof(SvgTextPathElement))]
        [XmlElement(typeof(SvgMaskElement))]
        [XmlElement(typeof(SvgFilterElement))]
        [XmlElement(typeof(SvgClipPathElement))]
        public SvgElementList Children { get; }

        internal override IEnumerable<SvgElement> GetChildren() => Children;

        internal SvgStructuralGraphicsElement() => Children = new SvgElementList(this);

        protected override void PopulateClone(SvgElement element)
        {
            var structuralClone = (SvgStructuralGraphicsElement)element;
            structuralClone.Children.AddRange(Children.Select(i => i.DeepClone()));
            base.PopulateClone(element);
        }

        internal override void OnLoaded()
        {
            base.OnLoaded();
            foreach (var child in Children)
            {
                child.OnLoaded();
            }
        }
    }
}