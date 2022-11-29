using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc cref="SvgStructuralGraphicsElement" />
    /// <summary>
    /// The SVGSVGElement interface corresponds to the ‘svg’ element.
    /// </summary>
    [XmlRoot("svg", Namespace = SvgDocument.SvgNs)]
    [XmlType("svg", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgSvgElement : SvgStructuralGraphicsElement, ISvgFitToViewbox
    {
        /// <summary>
        /// Gets or sets the SVG language version to which this document fragment conforms
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; } = "1.1";

        /// <summary>
        /// Gets a value indicating whether the current ‘svg’ element is the outermost ‘svg’ element.
        /// </summary>
        public bool IsOutermost => ParentSvg == null;

        /// <summary>
        /// Gets or sets the x-axis coordinate of one corner of the rectangular region into which an embedded ‘svg’ element is placed.
        /// If the attribute is not specified, the effect is as if a value of '0' were specified.
        /// Has no meaning on <see cref="IsOutermost">outermost</see> svg elements.
        /// </summary>
        [XmlIgnore]
        public SvgLength? X { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of one corner of the rectangular region into which an embedded ‘svg’ element is placed.
        /// If the attribute is not specified, the effect is as if a value of '0' were specified.
        /// Has no meaning on <see cref="IsOutermost">outermost</see> svg elements.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Y { get; set; }

        /// <summary>
        /// Gets or sets the ‘x’ attribute on the given ‘svg’ element.
        /// </summary>
        [XmlAttribute("x")]
        public string XAsString
        {
            get => X?.ToString() ?? string.Empty;
            set => X = new SvgLength(value, this, SvgLengthDirection.Horizontal);
        }

        /// <summary>
        /// Gets or sets the ‘y’ attribute on the given ‘svg’ element.
        /// </summary>
        [XmlAttribute("y")]
        public string YAsString
        {
            get => Y?.ToString() ?? string.Empty;
            set => Y = new SvgLength(value, this, SvgLengthDirection.Vertical);
        }

        [XmlIgnore]
        public SvgLength? WidthAsLength { get; private set; }

        [XmlIgnore]
        public SvgLength? HeightAsLength { get; private set; }

        /// <summary>
        /// Gets or sets the width of the element, which for outermost svg elements, represents the intrinsic width of the SVG document fragment. 
        /// For embedded ‘svg’ elements, the width of the rectangular region into which the ‘svg’ element is placed. 
        /// A value of zero disables rendering of the element.
        /// </summary>
        [XmlIgnore]
        public float? Width
        {
            get => WidthAsLength?.Value;
            set => WidthAsLength = value != null ? (SvgLength?)new SvgLength(value.Value) : null;
        }

        /// <summary>
        /// Gets or sets the height of the element, which for outermost svg elements, represents the intrinsic height of the SVG document fragment. 
        /// For embedded ‘svg’ elements, the height of the rectangular region into which the ‘svg’ element is placed.  
        /// A value of zero disables rendering of the element.
        /// If the attribute is not specified, the effect is as if a value of '100%' were specified.
        /// </summary>
        [XmlIgnore]
        public float? Height
        {
            get => HeightAsLength?.Value;
            set => HeightAsLength = value != null ? (SvgLength?)new SvgLength(value.Value) : null;
        }

        /// <summary>
        /// Gets or sets the ‘width’ attribute on the given ‘svg’ element.
        /// </summary>
        [XmlAttribute("width")]
        public string WidthAsString
        {
            get => WidthAsLength?.ToString() ?? string.Empty;
            set => WidthAsLength = new SvgLength(value, this, SvgLengthDirection.Horizontal);
        }

        /// <summary>
        /// Gets or sets the ‘height’ attribute on the given ‘svg’ element.
        /// </summary>
        [XmlAttribute("height")]
        public string HeightAsString
        {
            get => HeightAsLength?.ToString() ?? string.Empty;
            set => HeightAsLength = new SvgLength(value, this, SvgLengthDirection.Vertical);
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the viewbox container to which graphics are stretched to
        /// </summary>
        [XmlIgnore]
        public SvgRect? ViewBox { get; set; }

        /// <summary>
        /// Gets or sets the ‘viewBox’ attribute on the given ‘svg’ element.
        /// </summary>
        [XmlAttribute("viewBox")]
        public string ViewBoxAsString
        {
            get => ViewBox.HasValue ? $"{ViewBox.Value.X} {ViewBox.Value.Y} {ViewBox.Value.Width} {ViewBox.Value.Height}" : string.Empty;
            set => ViewBox = string.IsNullOrEmpty(value) ? (SvgRect?)null : SvgRect.Parse(value);
        }

        public float ViewWidth => ViewBox?.Width ?? (WidthAsLength?.Value ?? 0);

        public float ViewHeight => ViewBox?.Height ?? (HeightAsLength?.Value ?? 0);

        public override string ToString() => $"<svg>{string.Join(string.Empty, Children)}</svg>";

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the description of how to stretch graphics to fit a viewport
        /// </summary>
        [XmlIgnore]
        public SvgPreserveAspectRatio PreserveAspectRatio { get; set; }

        [XmlAttribute("preserveAspectRatio")]
        public string PreserveAspectRatioAsString
        {
            get => PreserveAspectRatio?.ToString() ?? string.Empty;
            set => PreserveAspectRatio = string.IsNullOrEmpty(value) ? null : SvgPreserveAspectRatio.Parse(value);
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => new SvgRect(X?.Value ?? 0, Y?.Value ?? 0, WidthAsLength?.Value ?? 0, HeightAsLength?.Value ?? 0);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitSvgElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitSvgElement(this);

        /// <summary>
        /// Gets the child element by the specified <paramref name="id"/> or returns null if not found
        /// </summary>
        /// <param name="id">the id of the element to retrieve</param>
        /// <returns>element with the specified id or null if not found</returns>
        public SvgElement GetElementById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var queue = new Queue<SvgElement>();

            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Id == id)
                {
                    return current;
                }
                foreach (var child in current.GetChildren())
                {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        protected override SvgElement CreateClone() => new SvgSvgElement
        {
            Version = Version,
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            ViewBox = ViewBox,
            PreserveAspectRatio = PreserveAspectRatio
        };

        /// <summary>
        /// Gets all the style elements the current 'svg' element has access to, including those defined by any
        /// outer 'svg' elements
        /// </summary>
        internal IEnumerable<SvgStyleElement> StyleElements { get; private set; } = Enumerable.Empty<SvgStyleElement>();

        internal override void OnLoaded()
        {
            var rootSvg = this;
            while (rootSvg.ParentSvg != null)
            {
                rootSvg = rootSvg.ParentSvg;
            }

            StyleElements = rootSvg.Descendants().OfType<SvgStyleElement>().ToArray();
            base.OnLoaded();
        }
    }
}