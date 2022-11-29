using System;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGRectElement interface corresponds to the ‘rect’ element.
    /// </summary>
    [XmlType("rect", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgRectElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets or sets the x-axis coordinate of the side of the rectangle which has the smaller x-axis coordinate 
        /// value in the current user coordinate system.  If the attribute is not specified, the effect is as if a value 
        /// of "0" were specified.
        /// </summary>
        [XmlIgnore]
        public SvgLength? X { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the side of the rectangle which has the smaller y-axis coordinate 
        /// value in the current user coordinate system.  If the attribute is not specified, the effect is as if a value 
        /// of "0" were specified.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangle
        /// </summary>
        [XmlIgnore]
        public SvgLength? Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangle
        /// </summary>
        [XmlIgnore]
        public SvgLength? Height { get; set; }

        /// <summary>
        /// Gets or sets the x-axis radius of the ellipse used to round off the corners of the rectangle.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Rx { get; set; }

        /// <summary>
        /// Gets or sets the y-axis radius of the ellipse used to round off the corners of the rectangle.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Ry { get; set; }

        /// <summary>
        /// Gets the final x-axis radius of the ellipse used to round off the corners of the rectangle.
        /// If <see cref="Rx"/> is specified it will be used, otherwise if <see cref="Ry"/> is specified it will be used.
        /// If neither are specified the result is 0.
        /// The result will also be clamped to a maximum of half the <see cref="Width"/>.
        /// </summary>
        public float RadiusX => Math.Max(0, Math.Min(Rx?.Value ?? (Ry?.Value ?? 0), Width?.Value ?? 0));

        /// <summary>
        /// Gets the final y-axis radius of the ellipse used to round off the corners of the rectangle.
        /// If <see cref="Ry"/> is specified it will be used, otherwise if <see cref="Rx"/> is specified it will be used.
        /// If neither are specified the result is 0.
        /// The result will also be clamped to a maximum of half the <see cref="Height"/>.
        /// </summary>
        public float RadiusY => Ry?.Value ?? (Rx?.Value ?? 0);

        /// <summary>
        /// Gets or sets the ‘x’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("x")]
        public string XAsString
        {
            get => X?.ToString() ?? string.Empty;
            set => X = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘y’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("y")]
        public string YAsString
        {
            get => Y?.ToString() ?? string.Empty;
            set => Y = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘width’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("width")]
        public string WidthAsString
        {
            get => Width?.ToString() ?? string.Empty;
            set => Width = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘height’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("height")]
        public string HeightAsString
        {
            get => Height?.ToString() ?? string.Empty;
            set => Height = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <summary>
        /// Gets or sets the ‘rx’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("rx")]
        public string RxAsString
        {
            get => Rx?.ToString() ?? string.Empty;
            set => Rx = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘ry’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("ry")]
        public string RyAsString
        {
            get => Ry?.ToString() ?? string.Empty;
            set => Ry = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => new SvgRect(X?.Value ?? 0, Y?.Value ?? 0, Width?.Value ?? 0, Height?.Value ?? 0);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitRectElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitRectElement(this);

        protected override SvgElement CreateClone()
            => new SvgRectElement
            {
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                Rx = Rx,
                Ry = Ry
            };
    }
}