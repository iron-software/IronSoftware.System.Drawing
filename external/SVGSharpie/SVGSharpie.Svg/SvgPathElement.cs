using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathElement interface corresponds to the &lt;path&gt; element.
    /// </summary>
    [XmlType("path", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgPathElement : SvgGeometryElement
    {
        /// <summary>
        /// Gets or sets the path data
        /// </summary>
        [XmlAttribute("d")]
        public string PathData
        {
            get => Segments.ToString();
            set => Segments = SvgPathSegList.Parse(value);
        }

        /// <summary>
        /// Gets the list of segments in the path
        /// </summary>
        public SvgPathSegList Segments { get; private set; } = new SvgPathSegList();

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox()
        {
            if (Segments.Count == 0)
            {
                return null;
            }
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitPathElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitPathElement(this);

        protected override SvgElement CreateClone()
            => new SvgPathElement
            {
                Segments = Segments.DeepClone()
            };
    }
}