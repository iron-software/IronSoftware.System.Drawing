using System.Collections.Generic;
using System.Linq;

namespace SVGSharpie
{
    public sealed class SvgTransformList : List<SvgTransform>
    {
        public SvgTransformList()
        {
        }

        public SvgTransformList(IEnumerable<SvgTransform> collection)
            :base(collection)
        {
        }

        public static SvgTransformList Parse(string markup) => SvgTransformListParser.Parse(markup);

        public override string ToString() => string.Join(string.Empty, this);

        internal SvgTransformList DeepClone() => new SvgTransformList(this.Select(i => i.DeepClone()));
    }
}