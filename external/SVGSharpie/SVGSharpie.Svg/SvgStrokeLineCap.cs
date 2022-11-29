using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Specifies the shape to be used at the end of open subpaths when they are stroked
    /// </summary>
    public enum SvgStrokeLineCap
    {
        [XmlEnum("butt")]
        Butt,
        [XmlEnum("round")]
        Round,
        [XmlEnum("square")]
        Square,
        [XmlEnum("inherit")]
        Inherit
    }
}