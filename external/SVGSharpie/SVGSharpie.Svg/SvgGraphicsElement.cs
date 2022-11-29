using System.Collections.Generic;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGGraphicsElement interface represents SVG elements whose primary purpose is to directly render graphics into a group.
    /// </summary>
    public abstract class SvgGraphicsElement : SvgElement
    {
        internal override bool PartakesInRenderingTree =>
            PresentationStyleData.Display != CssDisplayType.None && base.PartakesInRenderingTree;

        /// <summary>
        /// Gets the CSS style declarations of the current element extended with the presentation attributes which may have been
        /// set on the element directly
        /// </summary>
        public SvgElementStyleData PresentationStyleData { get; }

        /// <inheritdoc />
        public override ISvgElementComputedStyle Style { get; }

        /// <summary>
        /// Gets the computed value of the transform property and its corresponding transform attribute of the given element.
        /// </summary>
        [XmlIgnore]
        public SvgTransformList Transform { get; private set; } = new SvgTransformList();

        /// <summary>
        /// Gets or sets the ‘transform’ attribute on the current element.
        /// </summary>
        [XmlAttribute("transform")]
        public string TransformAsString
        {
            get => Transform.ToString();
            set => Transform = SvgTransformList.Parse(value);
        }

        /// <summary>
        /// Creates the paint server for drawing the fill of the current element
        /// </summary>
        /// <returns>paint server to use to draw the fill of the current element</returns>
        public SvgPaintServer CreateFillPaintServer()
            => SvgPaintServerFactory.CreatePaintServer(this, Style.Fill, Style.Color, Style.FillOpacity);

        /// <summary>
        /// Creates the paint server for drawing the outline of the current element
        /// </summary>
        /// <returns>paint server to use to draw the outline of the current element</returns>
        public SvgPaintServer CreateStrokePaintServer()
            => SvgPaintServerFactory.CreatePaintServer(this, Style.Stroke, Style.Color, Style.StrokeOpacity);

        [XmlIgnore]
        public float StrokeWidth => Style.StrokeWidth.Value.Value;

        internal SvgGraphicsElement()
        {
            PresentationStyleData = new SvgElementStyleData();
            Style = new SvgElementComputedStyle(PresentationStyleData, base.Style);
        }

        protected override void PopulateClone(SvgElement element)
        {
            var clone = (SvgGraphicsElement)element;
            clone.Transform = Transform.DeepClone();
            PresentationStyleData.CopyTo(clone.PresentationStyleData);
            base.PopulateClone(element);
        }

        protected override void OnAnyAttribute(string name, string value)
        {
            if (PresentationAttributeNames.Contains(name))
            {
                if (!PresentationStyleData.TryPopulateProperty(name, value))
                {
                    //throw new Exception($"Invalid presentation attribute '{name}' with value '{value}'");
                }
            }
        }

        private static readonly HashSet<string> PresentationAttributeNames = new HashSet<string>
        {
            "alignment-baseline", "baseline-shift", "clip", "clip-path", "clip-rule", "color", "color-interpolation", "color-interpolation-filters", "color-profile",
            "color-rendering", "cursor", "direction", "display", "dominant-baseline", "enable-background", "fill", "fill-opacity", "fill-rule", "filter", "flood-color",
            "flood-opacity", "font-family", "font-size", "font-size-adjust", "font-stretch", "font-style", "font-variant", "font-weight", "glyph-orientation-horizontal",
            "glyph-orientation-vertical", "image-rendering", "kerning", "letter-spacing", "lighting-color", "marker-end", "marker-mid", "marker-start", "mask", "opacity",
            "overflow", "pointer-events", "shape-rendering", "stop-color", "stop-opacity", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap",
            "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "text-anchor", "text-decoration", "text-rendering", "unicode-bidi", "visibility",
            "word-spacing", "writing-mode"
        };
    }
}