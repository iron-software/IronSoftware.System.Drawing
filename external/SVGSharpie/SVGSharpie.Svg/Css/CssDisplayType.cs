using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Specifies how an element is to be displayed
    /// </summary>
    public enum CssDisplayType
    {
        [XmlEnum("inline")]
        Inline,
        [XmlEnum("block")]
        Block,
        [XmlEnum("list-item")]
        ListItem,
        [XmlEnum("run-in")]
        RunIn,
        [XmlEnum("compact")]
        Compact,
        [XmlEnum("marker")]
        Marker,
        [XmlEnum("table")]
        Table,
        [XmlEnum("inline-table")]
        InlineTable,
        [XmlEnum("table-row-group")]
        TableRowGroup,
        [XmlEnum("table-header-group")]
        TableHeaderGroup,
        [XmlEnum("table-footer-group")]
        TableFooterGroup,
        [XmlEnum("table-row")]
        TableRow,
        [XmlEnum("table-column-group")]
        TableColumnGroup,
        [XmlEnum("table-column")]
        TableColumn,
        [XmlEnum("table-cell")]
        TableCell,
        [XmlEnum("table-caption")]
        TableCaption,
        [XmlEnum("none")]
        None,
        [XmlEnum("inherit")]
        Inherit
    }
}