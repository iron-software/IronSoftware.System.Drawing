using System;
using System.Collections.Generic;
using SVGSharpie.Css;

namespace SVGSharpie
{
    public sealed class SvgStyleRule
    {
        /// <summary>
        /// Gets the selectors of the current rule
        /// </summary>
        public IReadOnlyList<CssSelector> Selectors { get; }

        /// <summary>
        /// Gets the body of the current rule
        /// </summary>
        public SvgElementStyleData Rules { get; } = new SvgElementStyleData();

        public SvgStyleRule(CssStyleRule cssStyleRule)
        {
            Selectors = cssStyleRule?.Selectors ?? throw new ArgumentNullException(nameof(cssStyleRule));
            foreach (var p in cssStyleRule.Properties)
            {
                if (!Rules.TryPopulateProperty(p.Key, p.Value))
                {
                    // we should act like other things and ignore unknow properites
                    //throw new Exception($"Unknown style property '{p.Key}:{p.Value}'");
                }
            }
        }

        public override string ToString() => string.Join(", ", Selectors);
    }
}