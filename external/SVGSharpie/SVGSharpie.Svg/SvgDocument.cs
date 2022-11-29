using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SVGSharpie
{
    /// <summary>
    /// Represents the root object of the document object model hierarchy.
    /// </summary>
    public sealed class SvgDocument
    {
        internal const string SvgNs = "http://www.w3.org/2000/svg";

        internal const string XLinkNs = "http://www.w3.org/1999/xlink";

        /// <summary>
        /// Gets the title of a document as specified by the ‘title’ sub-element of the ‘svg’ root element
        /// (i.e., <svg><title>Here is the title</title>...</svg>)
        /// </summary>
        public string Title => RootElement?.Children.OfType<SvgTitleElement>().FirstOrDefault()?.Title ?? string.Empty;

        /// <summary>
        /// Gets the description of a document as specified by the ‘desc’ sub-element of the ‘svg’ root element
        /// </summary>
        public string Desc => RootElement?.Children.OfType<SvgDescElement>().FirstOrDefault()?.Description ?? string.Empty;

        /// <summary>
        /// Gets the root ‘svg’ in the document hierarchy.
        /// </summary>
        public SvgSvgElement RootElement { get; set; }

        public static SvgDocument Parse(Stream stream)
            => new StreamReader(stream).DisposeAfter(Parse);

        public static SvgDocument Parse(string xml)
            => new StringReader(xml).DisposeAfter(Parse);

        private static SvgDocument Parse(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var serializer = new XmlSerializer(typeof(SvgSvgElement));
            var root = (SvgSvgElement)serializer.Deserialize(reader);
            root.OnLoaded();
            return new SvgDocument
            {
                RootElement = root
            };
        }
    }
}