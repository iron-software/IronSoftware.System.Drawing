using System;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGMaskElement interface corresponds to the ‘mask’ element.
    /// </summary>
    [XmlType("mask", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgMaskElement : SvgElement
    {
        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitMaskElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitMaskElement(this);

        protected override SvgElement CreateClone() => new SvgMaskElement();
    }
}