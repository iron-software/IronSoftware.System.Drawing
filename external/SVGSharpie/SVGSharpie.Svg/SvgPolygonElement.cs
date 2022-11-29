using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPolygonElement interface corresponds to the ‘polygon’ element.  
    /// The ‘polygon’ element defines a closed shape consisting of a set of connected straight line segments.
    /// </summary>
    [XmlType("polygon", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgPolygonElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets the points that make up the polygon. All coordinate values are in the user coordinate system.
        /// </summary>
        [XmlIgnore]
        public SvgPolyPointList Points { get; private set; } = new SvgPolyPointList();

        /// <summary>
        /// Gets or sets the ‘points’ attribute on the given ‘polygon’ element.
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
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitPolygonElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitPolygonElement(this);

        protected override SvgElement CreateClone()
            => new SvgPolygonElement { Points = Points.DeepClone() };
    }
}