using System.Text;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// The SVGClipPathElement interface corresponds to the ‘clipPath’ element.
    /// </summary>
    [XmlType("clipPath", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgClipPathElement : SvgStructuralGraphicsElement
    {
        public override SvgRect? GetBBox() => null;

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitClipPathElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitClipPathElement(this);

        public override string ToString()
        {
            var builder = new StringBuilder("<clipPath");
            builder.Append(">");
            builder.Append(string.Join(string.Empty, Children));
            return builder.Append("</clipPath>").ToString();
        }

        protected override SvgElement CreateClone() => new SvgClipPathElement();
    }
}