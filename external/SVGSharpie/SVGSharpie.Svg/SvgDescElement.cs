using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SvgDescElement interface corresponds to the 'desc' descriptive element, which each container or graphics element in an SVG drawing can supply.
    /// </summary>
    [XmlType("desc", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgDescElement : SvgElement
    {
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlText]
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => new SvgRect(0, 0, 0, 0);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitDescElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitDescElement(this);

        protected override SvgElement CreateClone()
            => new SvgDescElement { Description = Description };
    }
}