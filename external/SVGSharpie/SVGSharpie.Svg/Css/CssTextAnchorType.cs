using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Specifies how an text is to be renderd
    /// </summary>
    public enum CssTextAnchorType
    {
        /// <summary>
        /// Specifies the current text is start aligned.
        /// </summary>
        [XmlEnum("start")]
        Start,
        /// <summary>
        /// Specifies the current text is end aligned.
        /// </summary>
        [XmlEnum("end")]
        End,
        /// <summary>
        /// Specifies the current text is left aligned.
        /// </summary>
        [XmlEnum("middle")]
        Middle,
        /// <summary>
        /// Specifies the text-align value is to be inherited from the parent
        /// </summary>
        [XmlEnum("inherit")]
        Inherit
    }
}