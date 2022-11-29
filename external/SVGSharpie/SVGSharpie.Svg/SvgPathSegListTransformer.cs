using System;
using System.Collections.Generic;
using System.Linq;
using SVGSharpie.Utils;

namespace SVGSharpie
{
    /// <summary>
    /// Provides transformations over a <see cref="SvgPathSegList"/>
    /// </summary>
    public static class SvgPathSegListTransformer
    {
        private static readonly HashSet<SvgPathSegType> RelativePathSegTypes = new HashSet<SvgPathSegType>
        {
            SvgPathSegType.ArcRel, SvgPathSegType.CurvetoCubicRel, SvgPathSegType.CurvetoCubicSmoothRel,
            SvgPathSegType.CurvetoQuadraticRel, SvgPathSegType.CurvetoQuadraticSmoothRel, SvgPathSegType.LineToRel,
            SvgPathSegType.LinetoHorizontalRel, SvgPathSegType.LinetoVerticalRel, SvgPathSegType.MoveToRel
        };

        /// <summary>
        /// Converts all the curves in the path segment list to <see cref="SvgPathSegCurvetoCubicAbs">cubic bezier curves</see>
        /// </summary>
        /// <param name="list">list of path segments to transform</param>
        /// <returns>transformed list of path segments where all curves have been replaced with cubic bezier curves</returns>
        public static SvgPathSegList ConvertAllLinesAndCurvesToCubicCurves(SvgPathSegList list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            var transformer = new SvgPathSegLinesAndCurvestoCubicCurvesConverter();
            foreach (var item in list)
            {
                item.Accept(transformer);
            }
            transformer.Flush();
            return transformer.Result;
        }

        /// <summary>
        /// Converts all relative path segments in the specified list to absolute path 
        /// segments and returns the transformed list
        /// </summary>
        /// <param name="list">list of path segments to transform</param>
        /// <returns>transformed list of path segments where all relative path segments have been converted into their absolute counterparts</returns>
        public static SvgPathSegList ConvertToAbsolute(SvgPathSegList list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            var transformer = new SvgPathSegRelativeToAbsoluteConverter();
            return new SvgPathSegList(list.Select(item => item.Accept(transformer)));
        }

        /// <summary>
        /// Multiplies all path segments in the specified list by the specified matrix.  If there are any relative path segments in the 
        /// list then they will be converted to their absolute counterparts.
        /// </summary>
        /// <param name="list">list of path segments to transform</param>
        /// <param name="matrix">matrix to multiply the path segment coordinates by</param>
        /// <returns>transformed list of path segments where all coordinates have been multiplied by the specified matrix</returns>
        public static SvgPathSegList MultiplyByMatrix(SvgPathSegList list, SvgMatrix matrix)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));
            if (list.Any(i => RelativePathSegTypes.Contains(i.PathSegType)))
            {
                list = ConvertToAbsolute(list);
            }
            var transformer = new SvgPathSegMatrixTransformer(matrix);
            return new SvgPathSegList(list.Select(item => item.Accept(transformer)));
        }
    }
}