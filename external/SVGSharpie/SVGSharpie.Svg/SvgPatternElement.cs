using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// The SVGPatternElement interface corresponds to the ‘pattern’ element.  A pattern is used to fill or stroke an 
    /// object using a pre-defined graphic object which can be replicated ("tiled") at fixed intervals in x and y to 
    /// cover the areas to be painted. Patterns are defined using a ‘pattern’ element and then referenced by properties 
    /// ‘fill’ and ‘stroke’ on a given graphics element to indicate that the given element shall be filled or stroked 
    /// with the referenced pattern.
    /// </summary>
    /// <remarks>
    /// Attributes ‘x’, ‘y’, ‘width’, ‘height’ and ‘patternUnits’ define a reference rectangle somewhere on the infinite 
    /// canvas. The reference rectangle has its top/left at (x, y) and its bottom/right at (x + width, y + height). The 
    /// tiling theoretically extends a series of such rectangles to infinity in X and Y (positive and negative), with 
    /// rectangles starting at (x + m*width, y + n* height) for each possible integer value for m and n.
    /// </remarks>
    [XmlType("pattern", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgPatternElement : SvgElement
    {
        /// <summary>
        /// Gets or sets a value defining the coordinate system to use for spatial attributes (x, y, width, height)
        /// </summary>
        [XmlAttribute("patternUnits")]
        public SvgUnitTypes PatternUnits { get; set; }

        /// <summary>
        /// Gets or sets a value defining the coordinate system to use for the contents of the pattern
        /// </summary>
        /// <remarks>
        /// No effect if the 'viewBox' attribute is specified
        /// </remarks>
        [XmlAttribute("patternContentUnits")]
        public SvgUnitTypes PatternContentUnits { get; set; }

        /// <summary>
        /// Gets an optional additional transformation from the pattern coordinate system onto the target coordinate system
        /// </summary>
        /// <remarks>
        /// This allows for things such as skewing the pattern tiles. This additional transformation matrix is post-
        /// multiplied to (i.e., inserted to the right of) any previously defined transformations, including the implicit 
        /// transformation necessary to convert from object bounding box units to user space.
        /// </remarks>
        [XmlIgnore]
        public SvgTransformList PatternTransform { get; private set; } = new SvgTransformList();

        /// <summary>
        /// Gets or sets the x- axis offset of the tile placement
        /// </summary>
        [XmlIgnore]
        public SvgLength? X { get; set; }

        /// <summary>
        /// Gets or sets the y- axis offset of the tile placement
        /// </summary>
        [XmlIgnore]
        public SvgLength? Y { get; set; }

        /// <summary>
        /// Gets or sets the width of a tile
        /// </summary>
        [XmlIgnore]
        public SvgLength? Width { get; set; }

        /// <summary>
        /// Gets or sets the height of a tile
        /// </summary>
        [XmlIgnore]
        public SvgLength? Height { get; set; }

        /// <summary>
        /// Gets or sets the ‘x’ attribute on the given ‘pattern’ element.
        /// </summary>
        [XmlAttribute("x")]
        public string XAsString
        {
            get => X?.ToString() ?? string.Empty;
            set => X = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘y’ attribute on the given ‘pattern’ element.
        /// </summary>
        [XmlAttribute("y")]
        public string YAsString
        {
            get => Y?.ToString() ?? string.Empty;
            set => Y = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘width’ attribute on the given ‘pattern’ element.
        /// </summary>
        [XmlAttribute("width")]
        public string WidthAsString
        {
            get => Width?.ToString() ?? string.Empty;
            set => Width = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘height’ attribute on the given ‘pattern’ element.
        /// </summary>
        [XmlAttribute("height")]
        public string HeightAsString
        {
            get => Height?.ToString() ?? string.Empty;
            set => Height = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘patternTransform’ attribute on the given element.
        /// </summary>
        [XmlAttribute("patternTransform")]
        public string GradientTransformAsString
        {
            get => PatternTransform.ToString();
            set => PatternTransform = SvgTransformList.Parse(value);
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => new SvgRect(X?.Value ?? 0, Y?.Value ?? 0, Width?.Value ?? 0, Height?.Value ?? 0);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitPatternElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitPatternElement(this);

        protected override SvgElement CreateClone() => new SvgPatternElement
        {
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            PatternUnits = PatternUnits,
            PatternContentUnits = PatternContentUnits,
            PatternTransform = PatternTransform.DeepClone()
        };
    }
}