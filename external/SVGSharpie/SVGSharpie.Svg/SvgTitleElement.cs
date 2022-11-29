using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SvgTitleElement interface corresponds to the 'title' descriptive element, which each container or graphics element in an SVG drawing can supply.
    /// </summary>
    [XmlType("title", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgTitleElement : SvgElement
    {
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        [XmlText]
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => null;

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitTitleElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitTitleElement(this);

        protected override SvgElement CreateClone()
            => new SvgTitleElement { Title = Title };
    }
}