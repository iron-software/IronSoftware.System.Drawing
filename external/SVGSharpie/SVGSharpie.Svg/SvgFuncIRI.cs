using System;

namespace SVGSharpie
{
    /// <summary>
    /// Represents an Internationalized Resource Identifier (IRI) with an optional fragment identifier, within an SVG document
    /// </summary>
    public sealed class SvgFuncIRI
    {
        public string Url { get; }

        public string FragmentIdentifier { get; }

        private SvgFuncIRI(string url, string fragmentIdentifier)
        {
            Url = url ?? string.Empty;
            FragmentIdentifier = fragmentIdentifier ?? string.Empty;
        }

        public static SvgFuncIRI Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var trimmedValue = value.Trim();
            if (!trimmedValue.StartsWith("url", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Invalid IRI, expected to start with 'url' but got '{value}'");
            }
        
            var open = trimmedValue.IndexOf('(') + 1;
            var close = trimmedValue.IndexOf(')', open);
            var urlValue = trimmedValue.Substring(open, close - open);

            return urlValue.StartsWith("#")
                ? new SvgFuncIRI(null, urlValue.Substring(1))
                : new SvgFuncIRI(urlValue, null);
        }
    }
}