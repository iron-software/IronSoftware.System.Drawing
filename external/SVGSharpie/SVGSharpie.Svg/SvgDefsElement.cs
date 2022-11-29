using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the ‘defs’ element, which is a container element for referenced elements.  Elements that are descendants 
    /// of a ‘defs’ are not rendered directly; they are prevented from becoming part of the rendering tree just as if the 
    /// ‘defs’ element were a ‘g’ element and the ‘display’ property were set to none.
    /// </summary>
    [XmlType("defs", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgDefsElement : SvgElement
    {
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
        public List<SvgElement> Children { get; } = new List<SvgElement>();

        protected override SvgElement CreateClone()
            => new SvgDefsElement(Children.Select(i => i.DeepClone()));
        
        public SvgDefsElement()
        {
        }

        public SvgDefsElement(IEnumerable<SvgElement> children)
            => Children.AddRange(children);

        /// <inheritdoc cref="SvgElement.GetChildren"/>
        internal override IEnumerable<SvgElement> GetChildren() => Children;

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => new SvgRect(0, 0, 0, 0);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitDefsElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitDefsElement(this);
    }
}