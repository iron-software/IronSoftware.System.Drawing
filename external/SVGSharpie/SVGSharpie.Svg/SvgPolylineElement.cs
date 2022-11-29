using System;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPolylineElement interface corresponds to the ‘polyline’ element.  
    /// The ‘polyline’ element defines a set of connected straight line segments. Typically, ‘polyline’ elements define open shapes.
    /// </summary>
    [XmlType("polyline", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgPolylineElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets the points that make up the polyline. All coordinate values are in the user coordinate system.
        /// </summary>
        [XmlIgnore]
        public SvgPolyPointList Points { get; private set; } = new SvgPolyPointList();

        /// <summary>
        /// Gets or sets the ‘points’ attribute on the given ‘polyline’ element.
        /// </summary>
        [XmlAttribute("points")]
        public string PointsAsString
        {
            get => Points.ToString();
            set => Points = SvgPolyPointListParser.Parse(value);
        }

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => Points.GetBBox();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitPolylineElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitPolylineElement(this);

        protected override SvgElement CreateClone()
            => new SvgPolylineElement { Points = Points.DeepClone() };
    }
}