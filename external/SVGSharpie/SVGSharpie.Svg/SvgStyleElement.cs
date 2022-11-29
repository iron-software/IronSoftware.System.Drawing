using System.Collections.Generic;
using System.Xml.Serialization;
using SVGSharpie.Css;

namespace SVGSharpie
{
    /// <inheritdoc cref="SvgElement" />
    /// <summary>
    /// The ‘style’ element allows style sheets to be embedded directly within SVG content. SVG's ‘style’ element 
    /// has the same attributes as the corresponding element in HTML.
    /// The SVGStyleElement interface corresponds to the ‘style’ element.
    /// </summary>
    [XmlType("style", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgStyleElement : SvgElement
    {
        /// <summary>
        /// Gets or sets the style sheet language of the element's contents. The style sheet language is specified as 
        /// a content type (e.g., "text/css").
        /// </summary>
        [XmlAttribute("type")]
        public string StyleType { get; set; }

        /// <summary>
        /// Gets or sets the intended destination medium for style information. It may be a single media descriptor or 
        /// a comma-separated list. The default value for this attribute is "all". 
        /// </summary>
        [XmlAttribute("media")]
        public string Media { get; set; }

        /// <summary>
        /// Gets or sets the advisory title for the ‘style’ element.
        /// </summary>
        [XmlAttribute("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets the rule content as a string
        /// </summary>
        [XmlText]
        public string ContentAsString
        {
            get => string.Join(" ", StyleRules);
            set => Parse(value);
        }

        /// <summary>
        /// Gets the list of rules in the style element
        /// </summary>
        [XmlIgnore]
        public List<SvgStyleRule> StyleRules { get; } = new List<SvgStyleRule>();

        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox() => null;

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitStyleElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitStyleElement(this);

        protected override SvgElement CreateClone()
            => new SvgStyleElement
            {
                StyleType = StyleType,
                Media = Media,
                Title = Title
            };

        private void Parse(string value)
        {
            var reader = new CssStringStreamReader(value);
            if (!CssStyleRulesParser.TryParseRules(reader, out var rules))
            {
                throw new CssParserException(reader, "CSS parser error");
            }
            foreach (var rule in rules)
            {
                StyleRules.Add(new SvgStyleRule(rule));
            }
        }
    }
}