using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// The SVGStopElement interface corresponds to the ‘stop’ element.
    /// </summary>
    [XmlType("stop", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgStopElement : SvgElement
    {
        /// <summary>
        /// Gets or sets a value which indicates where the gradient stop is placed.  A number usually ranging from 0 to 1,
        /// or a percentage usually ranging from 0 to 100%.
        /// </summary>
        /// <remarks>
        /// For linear gradients, the ‘offset’ attribute represents a location along the gradient vector. For radial 
        /// gradients, it represents a percentage distance from (fx,fy) to the edge of the outermost/largest circle.
        /// </remarks>
        [XmlIgnore]
        public float? Offset { get; set; }

        /// <summary>
        /// Gets or sets the color to use at the gradient stop
        /// </summary>
        [XmlIgnore]
        public SvgColor? StopColor { get; set; } = SvgColor.Black;

        /// <summary>
        /// Gets or sets the ‘stop-opacity’ attribute on the given ‘stop’ element.
        /// </summary>
        [XmlAttribute("stop-opacity")]
        public float StopOpacity { get; set; } = 1;

        /// <summary>
        /// Gets or sets the ‘offset’ attribute on the given ‘stop’ element.
        /// </summary>
        [XmlAttribute("offset")]
        public string OffsetAsString
        {
            get => Offset?.ToString() ?? string.Empty;
            set => Offset = ParseOffset(value);
        }

        /// <summary>
        /// Gets or sets the ‘stop-color’ attribute on the given ‘stop’ element.
        /// </summary>
        [XmlAttribute("stop-color")]
        public string StopColorAsString
        {
            get => StopColor?.ToString() ?? string.Empty;
            set => StopColor = !string.IsNullOrEmpty(value) ? SvgColorTranslator.FromSvgColorCode(value) as SvgColor? : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => null;

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitStopElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitStopElement(this);

        protected override void PopulateClone(SvgElement element)
        {
            var clone = (SvgStopElement)element;
            clone.Offset = Offset;
            clone.StopColor = StopColor;
            clone.StopOpacity = StopOpacity;
            base.PopulateClone(element);
        }
        
        protected override SvgElement CreateClone()
            => new SvgStopElement();
        
        /// <remarks>
        /// The ‘offset’ attribute is either a 'number' (usually ranging from 0 to 1) or a 'percentage' (usually
        /// ranging from 0% to 100%) which indicates where the gradient stop is placed.
        /// </remarks>
        private static float? ParseOffset(string value)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                return null;
            }

            if (trimmed.EndsWith("%"))
            {
                return float.Parse(trimmed.Substring(0, trimmed.Length - 1)) / 100;
            }

            return float.Parse(trimmed);
        }
    }
}