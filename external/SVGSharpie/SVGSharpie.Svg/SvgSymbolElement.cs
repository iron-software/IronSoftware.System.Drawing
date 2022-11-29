using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc cref="SvgGraphicsElement" />
    /// <summary>
    /// The SVGSymbolElement interface corresponds to the ‘symbol’ element.
    /// The ‘symbol’ element is used to define graphical template objects which can be instantiated by a ‘use’ element.
    /// The use of ‘symbol’ elements for graphics that are used multiple times in the same document adds structure and semantics.
    /// </summary>
    [XmlType("symbol", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgSymbolElement : SvgGraphicsElement, ISvgFitToViewbox
    {
        [XmlElement(typeof(SvgGElement))]
        [XmlElement(typeof(SvgDefsElement))]
        [XmlElement(typeof(SvgUseElement))]
        [XmlElement(typeof(SvgSymbolElement))]
        [XmlElement(typeof(SvgPathElement))]
        [XmlElement(typeof(SvgRectElement))]
        [XmlElement(typeof(SvgCircleElement))]
        [XmlElement(typeof(SvgEllipseElement))]
        [XmlElement(typeof(SvgLineElement))]
        [XmlElement(typeof(SvgPolylineElement))]
        [XmlElement(typeof(SvgPolygonElement))]
        [XmlElement(typeof(SvgTitleElement))]
        [XmlElement(typeof(SvgDescElement))]
        [XmlElement(typeof(SvgSvgElement))]
        [XmlElement(typeof(SvgStyleElement))]
        [XmlElement(typeof(SvgLinearGradientElement))]
        [XmlElement(typeof(SvgRadialGradientElement))]
        [XmlElement(typeof(SvgPatternElement))]
        [XmlElement(typeof(SvgTextElement))]
        [XmlElement(typeof(SvgTextPathElement))]
        [XmlElement(typeof(SvgMaskElement))]
        [XmlElement(typeof(SvgFilterElement))]
        [XmlElement(typeof(SvgClipPathElement))]
        public List<SvgElement> Children { get; private set; } = new List<SvgElement>();

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the container to which graphics are stretched to
        /// </summary>
        [XmlIgnore]
        public SvgRect? ViewBox { get; set; }

        /// <summary>
        /// Gets or sets the ‘viewBox’ attribute on the given ‘symbol’ element.
        /// </summary>
        [XmlAttribute("viewBox")]
        public string ViewBoxAsString
        {
            get => ViewBox.HasValue ? $"{ViewBox.Value.X} {ViewBox.Value.Y} {ViewBox.Value.Width} {ViewBox.Value.Height}" : string.Empty;
            set => ViewBox = string.IsNullOrEmpty(value) ? (SvgRect?)null : SvgRect.Parse(value);
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the description of how to stretch graphics to fit a viewport
        /// </summary>
        [XmlIgnore]
        public SvgPreserveAspectRatio PreserveAspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the ‘preserveAspectRatio’ attribute on the given ‘symbol’ element.
        /// </summary>
        [XmlAttribute("preserveAspectRatio")]
        public string PreserveAspectRatioAsString
        {
            get => PreserveAspectRatio?.ToString() ?? string.Empty;
            set => PreserveAspectRatio = string.IsNullOrEmpty(value) ? null : SvgPreserveAspectRatio.Parse(value);
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => null;

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitSymbolElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitSymbolElement(this);

        protected override SvgElement CreateClone()
            => new SvgSymbolElement
            {
                Children = new List<SvgElement>(Children.Select(i => i.DeepClone())),
                ViewBox = ViewBox,
                PreserveAspectRatio = PreserveAspectRatio
            };
    }
}