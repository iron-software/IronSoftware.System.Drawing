namespace SVGSharpie.Css
{
    public enum CssCombinatorType
    {
        /// <summary>
        /// Describe an element that is the descendant of another element in the document tree (e.g. 'A B' or 'A >> B')
        /// </summary>
        Descendant,
        /// <summary>
        /// Describes a childhood relationship between two elements (e.g. 'body > p')
        /// </summary>
        Child,
        /// <summary>
        /// Describes elements that share the same parent in the document tree and the first element immediately 
        /// precedes the second one (e.g. 'h1.opener + h2')
        /// </summary>
        NextSibling,
        /// <summary>
        /// Describes elements that share the same parent in the document tree and the first element precedes (not 
        /// necessarily immediately) the second one. (e.g. 'h1 ~ pre')
        /// </summary>
        SubsequentSibling
    }
}