using System;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGFilterElement interface corresponds to the ‘filter’ element.
    /// </summary>
    [XmlType("textPath", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgTextPathElement : SvgElement
    {
        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitTextPathElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitTextPathElement(this);

        protected override SvgElement CreateClone() => new SvgTextPathElement();
    }
}