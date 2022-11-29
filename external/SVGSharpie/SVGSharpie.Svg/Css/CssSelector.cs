namespace SVGSharpie.Css
{
    /// <inheritdoc />
    public abstract class CssSelector : ICssSelector
    {
        /// <summary>
        /// Gets the type of the current selector
        /// </summary>
        public abstract CssSelectorType SelectorType { get; }

        /// <summary>
        /// Gets the specificity of the current selector
        /// </summary>
        public abstract CssSpecificity Specificity { get; }

        internal CssSelector()
        {
        }

        /// <inheritdoc />
        public abstract void Accept(CssSelectorVisitor visitor);

        /// <inheritdoc />
        public abstract TResult Accept<TResult>(CssSelectorVisitor<TResult> visitor);
    }
}