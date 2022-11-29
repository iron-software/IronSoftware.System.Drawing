using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// The SVGLinearGradientElement interface corresponds to the ‘linearGradient’ element.
    /// </summary>
    [XmlType("linearGradient", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgLinearGradientElement : SvgGradientElement
    {
        /// <summary>
        /// Gets or sets the starting x- axis coordinate of the gradient vector.
        /// </summary>
        /// <remarks>
        /// The gradient vector provides starting and ending points onto which the gradient stops are mapped.
        /// </remarks>
        [XmlIgnore]
        public SvgLength? X1 { get; set; }

        /// <summary>
        /// Gets or sets the ending x- axis coordinate of the gradient vector.
        /// </summary>
        /// <remarks>
        /// The gradient vector provides starting and ending points onto which the gradient stops are mapped.
        /// </remarks>
        [XmlIgnore]
        public SvgLength? X2 { get; set; }

        /// <summary>
        /// Gets or sets the starting y- axis coordinate of the gradient vector.
        /// </summary>
        /// <remarks>
        /// The gradient vector provides starting and ending points onto which the gradient stops are mapped.
        /// </remarks>
        [XmlIgnore]
        public SvgLength? Y1 { get; set; }

        /// <summary>
        /// Gets or sets the ending y- axis coordinate of the gradient vector.
        /// </summary>
        /// <remarks>
        /// The gradient vector provides starting and ending points onto which the gradient stops are mapped.
        /// </remarks>
        [XmlIgnore]
        public SvgLength? Y2 { get; set; }

        /// <summary>
        /// Gets or sets the ‘x1’ attribute on the given ‘linearGradient’ element.
        /// </summary>
        [XmlAttribute("x1")]
        public string X1AsString
        {
            get => X1?.ToString() ?? string.Empty;
            set => X1 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘x2’ attribute on the given ‘linearGradient’ element.
        /// </summary>
        [XmlAttribute("x2")]
        public string X2AsString
        {
            get => X2?.ToString() ?? string.Empty;
            set => X2 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘x1’ attribute on the given ‘linearGradient’ element.
        /// </summary>
        [XmlAttribute("y1")]
        public string Y1AsString
        {
            get => Y1?.ToString() ?? string.Empty;
            set => Y1 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘x2’ attribute on the given ‘linearGradient’ element.
        /// </summary>
        [XmlAttribute("y2")]
        public string Y2AsString
        {
            get => Y2?.ToString() ?? string.Empty;
            set => Y2 = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => null;

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitLinearGradientElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitLinearGradientElement(this);
        
        protected override SvgElement CreateClone() => new SvgLinearGradientElement
        {
            X1 = X1,
            Y1 = X1,
            X2 = X2,
            Y2 = Y2,
        };
    }
}