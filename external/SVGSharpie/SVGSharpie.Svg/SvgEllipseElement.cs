using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGEllipseElement interface corresponds to the ‘ellipse’ element.
    /// The ‘ellipse’ element defines a circle based on a center point and a radius.
    /// </summary>
    [XmlType("ellipse", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgEllipseElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets or sets the x-axis coordinate of the center of the ellipse.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Cx { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the center of the ellipse.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Cy { get; set; }

        /// <summary>
        /// Gets or sets the x-axis radius of the ellipse
        /// </summary>
        [XmlIgnore]
        public SvgLength? Rx { get; set; }

        /// <summary>
        /// Gets or sets the y-axis radius of the ellipse
        /// </summary>
        [XmlIgnore]
        public SvgLength? Ry { get; set; }

        /// <summary>
        /// Gets or sets the ‘cx’ attribute on the given ‘ellipse’ element.
        /// </summary>
        [XmlAttribute("cx")]
        public string CxAsString
        {
            get => Cx?.ToString() ?? string.Empty;
            set => Cx = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘cy’ attribute on the given ‘ellipse’ element.
        /// </summary>
        [XmlAttribute("cy")]
        public string CyAsString
        {
            get => Cy?.ToString() ?? string.Empty;
            set => Cy = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘rx’ attribute on the given ‘ellipse’ element.
        /// </summary>
        [XmlAttribute("rx")]
        public string RxAsString
        {
            get => Rx?.ToString() ?? string.Empty;
            set => Rx = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘ry’ attribute on the given ‘ellipse’ element.
        /// </summary>
        [XmlAttribute("ry")]
        public string RyAsString
        {
            get => Ry?.ToString() ?? string.Empty;
            set => Ry = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox()
        {
            var rx = Rx?.Value ?? 0;
            var ry = Ry?.Value ?? 0;
            var left = (Cx?.Value ?? 0) - rx;
            var top = (Cy?.Value ?? 0) - ry;
            return new SvgRect(left, top, rx * 2, ry * 2);
        }

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitEllipseElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitEllipseElement(this);

        protected override SvgElement CreateClone()
            => new SvgEllipseElement
            {
                Cx = Cx,
                Cy = Cy,
                Rx = Rx,
                Ry = Ry
            };
    }
}