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
    [XmlType("text", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgTextElement : SvgGraphicsElement
    {
        public SvgTextElement()
        {
            Children = new SvgElementList(this);
        }

        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitTextElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitTextElement(this);

        protected override SvgElement CreateClone() => new SvgTextElement();

        /// <summary>
        /// Gets or sets the x-axis coordinate of the side of the rectangle which has the smaller x-axis coordinate 
        /// value in the current user coordinate system.  If the attribute is not specified, the effect is as if a value 
        /// of "0" were specified.
        /// </summary>
        [XmlIgnore]
        public SvgLength? X { get; set; }

        /// <summary>
        /// Gets or sets the y-axis coordinate of the side of the rectangle which has the smaller y-axis coordinate 
        /// value in the current user coordinate system.  If the attribute is not specified, the effect is as if a value 
        /// of "0" were specified.
        /// </summary>
        [XmlIgnore]
        public SvgLength? Y { get; set; }

        /// <summary>
        /// Gets or sets the ‘x’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("x")]
        public string XAsString
        {
            get => X?.ToString() ?? string.Empty;
            set => X = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Horizontal) : null;
        }

        /// <summary>
        /// Gets or sets the ‘y’ attribute on the given ‘rect’ element.
        /// </summary>
        [XmlAttribute("y")]
        public string YAsString
        {
            get => Y?.ToString() ?? string.Empty;
            set => Y = !string.IsNullOrEmpty(value) ? (SvgLength?)new SvgLength(value, this, SvgLengthDirection.Vertical) : null;
        }

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
            var builder = new StringBuilder("<text");
            builder.Append(">");
            builder.Append(string.Join(string.Empty, Children));
            return builder.Append("</text>").ToString();
        }
    }
}