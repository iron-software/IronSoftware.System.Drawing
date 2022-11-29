using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGLineElement interface corresponds to the ‘line’ element.  
    /// The ‘line’ element defines a line segment that starts at one point and ends at another.
    /// </summary>
    [XmlType("line", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgLineElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets or sets the x-axis coordinate of the start of the line.
        /// </summary>
        [XmlIgnore]
        public SvgLength? X1 { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the start of the line.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Y1 { get; set; }

        /// <summary>
        /// Gets or sets the x-axis coordinate of the end of the line.
        /// </summary>
        [XmlIgnore]
        public SvgLength? X2 { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the end of the line.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Y2 { get; set; }

        /// <summary>
        /// Gets or sets the ‘x1’ attribute on the given ‘line’ element.
        /// </summary>
        [XmlAttribute("x1")]
        public string X1AsString
        {
            get => X1?.ToString() ?? string.Empty;
            set => X1 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘y1’ attribute on the given ‘line’ element.
        /// </summary>
        [XmlAttribute("y1")]
        public string Y1AsString
        {
            get => Y1?.ToString() ?? string.Empty;
            set => Y1 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘x2’ attribute on the given ‘line’ element.
        /// </summary>
        [XmlAttribute("x2")]
        public string X2AsString
        {
            get => X2?.ToString() ?? string.Empty;
            set => X2 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘y2’ attribute on the given ‘line’ element.
        /// </summary>
        [XmlAttribute("y2")]
        public string Y2AsString
        {
            get => Y2?.ToString() ?? string.Empty;
            set => Y2 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox()
        {
            var minX = X1?.Value ?? 0;
            var minY = Y1?.Value ?? 0;
            var maxX = X2?.Value ?? minX;
            var maxY = Y2?.Value ?? minY;
            return new SvgRect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitLineElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitLineElement(this);

        protected override SvgElement CreateClone()
            => new SvgLineElement
            {
                X1 = X1,
                Y1 = Y1,
                X2 = X2,
                Y2 = Y2
            };
    }
}