using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Defines the coordinate system for the spatial attributes of <see cref="SvgGradientElement"/>, 
    /// <see cref="SvgPatternElement"/>, clipPath, mask, and filter elements
    /// </summary>
    public enum SvgUnitTypes
    {
        /// <summary>
        /// Specifies that spatial values represent values in the coordinate system that results from taking the 
        /// current user coordinate system in place at the time when the gradient element is referenced (i.e., the 
        /// user coordinate system for the element referencing the gradient element via a ‘fill’ or ‘stroke’ property) 
        /// and then applying the transform specified by attribute ‘gradientTransform’.
        /// </summary>
        [XmlEnum("userSpaceOnUse")]
        UserSpaceOnUse = 1,
        /// <summary>
        /// Specifies that the user coordinate system is established using the bounding box of the element to which 
        /// the gradient is applied.
        /// </summary>
        [XmlEnum("objectBoundingBox")]
        ObjectBoundingBox = 2
    }
}