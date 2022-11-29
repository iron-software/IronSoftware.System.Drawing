using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Defines what happens if the gradient starts or ends inside the bounds of the object(s) being painted by the gradient.
    /// </summary>
    public enum SvgGradientSpreadMethod
    {
        /// <summary>
        /// Specifies to use the terminal colors of the gradient to fill the remainder of the target region
        /// </summary>
        [XmlEnum("pad")]
        Pad,
        /// <summary>
        /// Specifies to reflect the gradient pattern start-to-end, end-to-start, start-to-end, etc. continuously 
        /// until the target rectangle is filled
        /// </summary>
        [XmlEnum("reflect")]
        Reflect,
        /// <summary>
        /// Specifies to repeat the gradient pattern start-to-end, start-to-end, start-to-end, etc. continuously 
        /// until the target region is filled.
        /// </summary>
        [XmlEnum("repeat")]
        Repeat
    }
}