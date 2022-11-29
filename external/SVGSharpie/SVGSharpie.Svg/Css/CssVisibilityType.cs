using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Specifies how an element is to be displayed
    /// </summary>
    public enum CssVisibilityType
    {
        /// <summary>
        /// Specifies the current graphics element is visible.
        /// </summary>
        [XmlEnum("visible")]
        Visible,
        /// <summary>
        /// Specifies the current graphics element is invisible (i.e., nothing is painted on the canvas).
        /// </summary>
        [XmlEnum("hidden")]
        Hidden,
        /// <summary>
        /// Specifies the current graphics element is invisible (i.e., nothing is painted on the canvas).
        /// </summary>
        [XmlEnum("collapse")]
        Collapse,
        /// <summary>
        /// Specifies the visibility value is to be inherited from the parent
        /// </summary>
        [XmlEnum("inherit")]
        Inherit
    }
}