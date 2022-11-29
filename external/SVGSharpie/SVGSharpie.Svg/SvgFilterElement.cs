using System;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGFilterElement interface corresponds to the ‘filter’ element.
    /// </summary>
    [XmlType("filter", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgFilterElement : SvgGraphicsElement
    {
        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitFilterElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitFilterElement(this);

        protected override SvgElement CreateClone() => new SvgFilterElement();
    }
}