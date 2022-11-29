using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGTextElement interface corresponds to the ‘text’ element.
    /// </summary>
    [XmlType("tspan", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgTextSpanElement : SvgGraphicsElement
    {
        public SvgTextSpanElement()
        {
            Children = new SvgElementList(this);
        }
        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitTextSpanElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitTextSpanElement(this);

        protected override SvgElement CreateClone() => new SvgTextSpanElement();

        internal override IEnumerable<SvgElement> GetChildren() => Children;

        [XmlIgnore]
        public SvgElementList Children { get; }

        [XmlText(typeof(string))]
        [XmlElement(typeof(SvgTextSpanElement), ElementName = "tspan")]
        public object[] AllChildren
        {
            get
            {
                return Children.Select(x => (x is SvgInlineTextSpanElement a ? a.Text : (object)x)).ToArray();
            }
            set
            {
                if (value != null)
                {
                    foreach (var c in value)
                    {
                        if (c is string t)
                        {
                            Children.Add(new SvgInlineTextSpanElement() { Text = t });
                        }
                        else if (c is SvgElement elm)
                        {
                            Children.Add(elm);
                        }
                    }
                }
                else
                {
                    Children.Clear();
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder("<tspan");
            builder.Append(">");
            builder.Append(string.Join(string.Empty, Children));
            return builder.Append("</tspan>").ToString();
        }
    }
}