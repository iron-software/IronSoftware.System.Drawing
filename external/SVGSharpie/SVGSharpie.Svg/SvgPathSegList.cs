using System.Collections.Generic;
using System.Linq;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a list of SVGPathSeg objects.
    /// </summary>
    public sealed class SvgPathSegList : List<SvgPathSeg>
    {
        public SvgPathSegList()
        {
        }

        public SvgPathSegList(int capacity) : base(capacity)
        {
        }

        public SvgPathSegList(IEnumerable<SvgPathSeg> collection) : base(collection)
        {
        }

        public SvgPathSegList DeepClone() => new SvgPathSegList(this.Select(i => i.DeepClone()));

        public override string ToString()
            => string.Join(string.Empty, this);

        public static SvgPathSegList Parse(string pathMarkupSyntax)
        {
            return SvgPathSegListParser.Parse(pathMarkupSyntax);
        }
    }

    public static class SvgPathSegListExtensions
    {
        /// <summary>
        /// Converts all relative path segments in the specified list to their absolute counterparts and returns the new list.
        /// </summary>
        /// <param name="list">list of path segments to transform</param>
        /// <returns>transformed list of path segments where all relative path segments have been converted into their absolute counterparts</returns>
        public static SvgPathSegList ConvertToAbsolute(this SvgPathSegList list)
            => SvgPathSegListTransformer.ConvertToAbsolute(list);

        /// <summary>
        /// Converts all the curves in the path segment list to <see cref="SvgPathSegCurvetoCubicAbs">cubic bezier curves</see>
        /// </summary>
        /// <param name="list">list of path segments to transform</param>
        /// <returns>transformed list of path segments where all curves have been replaced with cubic bezier curves</returns>
        public static SvgPathSegList ConvertAllLinesAndCurvesToCubicCurves(this SvgPathSegList list)
            => SvgPathSegListTransformer.ConvertAllLinesAndCurvesToCubicCurves(list);

        /// <summary>
        /// Multiplies all path segments in the specified list by the specified matrix.  If there are any relative path segments in the 
        /// list then they will be converted to their absolute counterparts.
        /// </summary>
        /// <param name="list">list of path segments to transform</param>
        /// <param name="matrix">matrix to multiply the path segment coordinates by</param>
        /// <returns>transformed list of path segments where all coordinates have been multiplied by the specified matrix</returns>
        public static SvgPathSegList MultiplyByMatrix(this SvgPathSegList list, SvgMatrix matrix)
            => SvgPathSegListTransformer.MultiplyByMatrix(list, matrix);
    }
}