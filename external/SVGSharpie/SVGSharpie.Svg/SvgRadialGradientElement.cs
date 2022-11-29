using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// The SVGRadialGradientElement interface corresponds to the ‘radialGradient’ element.
    /// </summary>
    [XmlType("radialGradient", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgRadialGradientElement : SvgGradientElement
    {
        /// <summary>
        /// Gets or sets the the x- axis coordinate defining the largest (i.e., outermost) circle for the radial gradient
        /// </summary>
        [XmlIgnore]
        public SvgLength? CircleX { get; set; }

        /// <summary>
        /// Gets or sets the the y- axis coordinate defining the largest (i.e., outermost) circle for the radial gradient
        /// </summary>
        [XmlIgnore]
        public SvgLength? CircleY { get; set; }

        /// <summary>
        /// Gets or sets the the radius defining the largest (i.e., outermost) circle for the radial gradient
        /// </summary>
        /// <remarks>
        /// If the attribute is not specified, the effect is as if a value of '50%' were specified.
        /// </remarks>
        [XmlIgnore]
        public SvgLength? CircleRadius { get; set; }

        /// <summary>
        /// Gets or sets the the x- axis focal point of the gradient.  The gradient will be drawn such that the 0% 
        /// gradient stop is mapped to (FocalX, FocalY). 
        /// </summary>
        [XmlIgnore]
        public SvgLength? FocalX { get; set; }

        /// <summary>
        /// Gets or sets the the y- axis focal point of the gradient.  The gradient will be drawn such that the 0% 
        /// gradient stop is mapped to (FocalX, FocalY). 
        /// </summary>
        [XmlIgnore]
        public SvgLength? FocalY { get; set; }

        /// <summary>
        /// Gets or sets the ‘cx’ attribute on the given ‘radialGradient’ element.
        /// </summary>
        [XmlAttribute("cx")]
        public string CxAsString
        {
            get => CircleX?.ToString() ?? string.Empty;
            set => CircleX = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘cy’ attribute on the given ‘radialGradient’ element.
        /// </summary>
        [XmlAttribute("cy")]
        public string CyAsString
        {
            get => CircleY?.ToString() ?? string.Empty;
            set => CircleY = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘r’ attribute on the given ‘radialGradient’ element.
        /// </summary>
        [XmlAttribute("r")]
        public string RAsString
        {
            get => CircleRadius?.ToString() ?? string.Empty;
            set => CircleRadius = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘fx’ attribute on the given ‘radialGradient’ element.
        /// </summary>
        [XmlAttribute("fx")]
        public string FxAsString
        {
            get => FocalX?.ToString() ?? string.Empty;
            set => FocalX = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘fx’ attribute on the given ‘radialGradient’ element.
        /// </summary>
        [XmlAttribute("fy")]
        public string FyAsString
        {
            get => FocalY?.ToString() ?? string.Empty;
            set => FocalY = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => null;
        
        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitRadialGradientElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitRadialGradientElement(this);

        protected override SvgElement CreateClone() => new SvgRadialGradientElement
        {
            CircleX = CircleX,
            CircleY = CircleY,
            CircleRadius = CircleRadius,
            FocalX = FocalX,
            FocalY = FocalY
        };
    }
}