using System;
using System.Text;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGGElement interface corresponds to the &lt;g&gt; element.
    /// </summary>
    [XmlType("g", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgGElement : SvgStructuralGraphicsElement
    {
        /// <inheritdoc cref="SvgElement.GetBBox"/>
        public override SvgRect? GetBBox()
        {
            if (Children.Count == 0)
            {
                return null;
            }
            SvgRect? result = null;
            foreach (var child in Children)
            {
                if (!child.PartakesInRenderingTree)
                {
                    continue;
                }
                var childBbox = child.GetBBox();
                if (childBbox != null && child is SvgGraphicsElement graphical)
                {
                    var txBbox = childBbox.Value;
                    foreach (var transform in graphical.Transform)
                    {
                        var matrix = transform.Matrix;
                        switch (transform.TransformType)
                        {
                            case SvgTransformType.Translate:
                                txBbox = new SvgRect(txBbox.X + matrix.E, txBbox.Y + matrix.F, txBbox.Width, txBbox.Height);
                                break;
                            case SvgTransformType.Scale:
                                var sw = txBbox.Width * matrix.A;
                                var sh = txBbox.Height * matrix.D;
                                txBbox = new SvgRect(txBbox.X - (sw * 0.5f), txBbox.Y - (sh * 0.5f), sw, sh);
                                break;
                            case SvgTransformType.Rotate:
                            case SvgTransformType.Skewx:
                            case SvgTransformType.Skewy:
                            case SvgTransformType.Matrix:
                                throw new NotImplementedException();
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    childBbox = txBbox;
                }
                if (result == null)
                {
                    result = childBbox;
                }
                else if (childBbox != null && childBbox.Value.Width > 0 && childBbox.Value.Height > 0)
                {
                    result = result.Value.Merge(childBbox.Value);
                }
            }
            return result;
        }

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitGElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitGElement(this);

        public override string ToString()
        {
            var builder = new StringBuilder("<g");
            builder.Append(">");
            builder.Append(string.Join(string.Empty, Children));
            return builder.Append("</g>").ToString();
        }

        protected override SvgElement CreateClone()
            => new SvgGElement();
    }
}