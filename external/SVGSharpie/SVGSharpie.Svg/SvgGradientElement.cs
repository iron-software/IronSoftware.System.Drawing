using System.Collections.Generic;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// The SVGGradientElement interface is a base interface used by <see cref="SvgLinearGradientElement"/> and 
    /// <see cref="SvgRadialGradientElement"/>.  A gradient element is one that defines a gradient paint server. 
    /// SVG 1.1 defines the following gradient elements: <see cref="SvgLinearGradientElement"/> and 
    /// <see cref="SvgRadialGradientElement"/>.
    /// </summary>
    public abstract class SvgGradientElement : SvgElement
    {
        /// <summary>
        /// Gets or sets a value defining the coordinate system to use for spatial attributes
        /// </summary>
        [XmlAttribute("gradientUnits")]
        public SvgUnitTypes GradientUnits { get; set; } = SvgUnitTypes.ObjectBoundingBox;

        /// <summary>
        /// Gets or sets a value defining what happens if the gradient starts or ends inside the bounds of the 
        /// target rectangle
        /// </summary>
        [XmlAttribute("spreadMethod")]
        public SvgGradientSpreadMethod SpreadMethod { get; set; } = SvgGradientSpreadMethod.Pad;

        /// <summary>
        /// Gets an optional additional transformation from the gradient coordinate system onto the target coordinate 
        /// system (i.e., userSpaceOnUse or objectBoundingBox).
        /// </summary>
        /// <remarks>
        /// This allows for things such as skewing the gradient. This additional transformation matrix is post-multiplied 
        /// to (i.e., inserted to the right of) any previously defined transformations, including the implicit 
        /// transformation necessary to convert from object bounding box units to user space.
        /// </remarks>
        [XmlIgnore]
        public SvgTransformList GradientTransform { get; private set; } = new SvgTransformList();

        /// <summary>
        /// Gets or sets the ‘gradientTransform’ attribute on the given element.
        /// </summary>
        [XmlAttribute("gradientTransform")]
        public string GradientTransformAsString
        {
            get => GradientTransform.ToString();
            set => GradientTransform = SvgTransformList.Parse(value);
        }

        /// <summary>
        /// Gets or sets the collection of stop elements of the current gradient element
        /// </summary>
        [XmlElement("stop", typeof(SvgStopElement))]
        public SvgStopElementList Stops { get; }

        protected internal SvgGradientElement() => Stops = new SvgStopElementList(this);

        internal override IEnumerable<SvgElement> GetChildren() => Stops;

        protected override void PopulateClone(SvgElement element)
        {
            var clone = (SvgGradientElement)element;
            clone.GradientTransform = GradientTransform.DeepClone();
            clone.GradientUnits = GradientUnits;
            clone.SpreadMethod = SpreadMethod;
            base.PopulateClone(element);
        }

        internal override void OnLoaded()
        {
            base.OnLoaded();
            foreach (var stop in Stops)
            {
                stop.OnLoaded();
            }
        }
    }
}