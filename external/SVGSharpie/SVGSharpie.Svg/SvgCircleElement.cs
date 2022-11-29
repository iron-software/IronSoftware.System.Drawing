using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGCircleElement interface corresponds to the ‘circle’ element.
    /// The ‘circle’ element defines a circle based on a center point and a radius.
    /// </summary>
    [XmlType("circle", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgCircleElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets or sets the x-axis coordinate of the center of the circle.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Cx { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the center of the circle.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Cy { get; set; }

        /// <summary>
        /// Gets or sets the radius of the cirlce
        /// </summary>
        [XmlIgnore]
        public SvgLength? R { get; set; }

        [XmlAttribute("cx")]
        public string CxAsString
        {
            get => Cx?.ToString() ?? string.Empty;
            set => Cx = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        [XmlAttribute("cy")]
        public string CyAsString
        {
            get => Cy?.ToString() ?? string.Empty;
            set => Cy = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        [XmlAttribute("r")]
        public string RAsString
        {
            get => R?.ToString() ?? string.Empty;
            set => R = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox()
        {
            var radius = R?.Value ?? 0;
            var left = (Cx?.Value ?? 0) - radius;
            var top = (Cy?.Value ?? 0) - radius;
            return new SvgRect(left, top, radius * 2, radius * 2);
        }

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitCircleElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitCircleElement(this);

        protected override SvgElement CreateClone()
        {
            return new SvgCircleElement
            {
                Cx = Cx,
                Cy = Cy,
                R = R
            };
        }
    }
}