namespace SVGSharpie.Css
{
    /// <summary>
    /// Describes how to match an <see cref="CssAttributeSelector"/> and its value to an elements attribute
    /// </summary>
    public enum CssAttributeSelectorOperation
    {
        /// <summary>
        /// Matches an element with the attribute, whatever the value of the attribute (e.g. '[att]')
        /// </summary>
        Has,
        /// <summary>
        /// Matches an element with the attribute whose value is exactly that specified (e.g. '[agg=val]')
        /// </summary>
        Exact,
        /// <summary>
        /// Matches an element with the attribute whose value is a whitespace-separated list of words, one of which is exactly the value specified (e.g. '[agg~=val]')
        /// </summary>
        WordExact,
        /// <summary>
        /// Matches an element with the attribute whose value is either exactly the one specified or beginning with the value and immediately followed by "-" (e.g. '[agg|=val]')
        /// </summary>
        ExactOrHyphenatedPrefix,
        /// <summary>
        /// Matches an element with the attribute whose value begins with the specified prefix (e.g. '[att^=val]'), if the specified value is empty then 
        /// the selector does not represent anything.
        /// </summary>
        Prefix,
        /// <summary>
        /// Matches an element with the attribute whose value ends with the specified prefix (e.g. '[att$=val]'), if the specified value is empty then 
        /// the selector does not represent anything.
        /// </summary>
        Suffix,
        /// <summary>
        /// Matches an element with the attribute whose value contains at least one instance of the value substring (e.g. '[att*=val]'), 
        /// if the specified value is empty then the selector does not represent anything.
        /// </summary>
        Contains
    }
}