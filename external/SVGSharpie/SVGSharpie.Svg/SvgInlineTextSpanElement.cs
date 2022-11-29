using System;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGTextElement interface corresponds to the ‘text’ element.
    /// </summary>
    public sealed class SvgInlineTextSpanElement : SvgElement
    {
        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitInlineTextSpanElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitInlineTextSpanElement(this);

        protected override SvgElement CreateClone() => new SvgTextSpanElement();

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}