using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVGSharpie
{
    internal static class SvgTransformListParser
    {
        public static SvgTransformList Parse(string markup)
        {
            if (markup == null) throw new ArgumentNullException(nameof(markup));
            var result = new SvgTransformList();

            for (var i = 0; i < markup.Length; i++)
            {
                while (i < markup.Length && !char.IsLetter(markup[i])) i++;
                if (i >= markup.Length) break;

                var open = markup.IndexOf('(', i);
                var close = markup.IndexOf(')', open + 1);
                if (open == -1 || close == -1)
                {
                    throw new Exception($"Expected '(' and ')' pair after '{markup.Substring(i)}'");
                }
                var transformName = markup.Substring(i, open - i);
                var transformArgs = markup.Substring(open + 1, close - open - 1);
                switch (transformName)
                {
                    case "matrix":
                        result.Add(CreateMatrixTransformationFromArgs(transformArgs));
                        break;
                    case "translate":
                        result.Add(CreateTranslateTransformationFromArgs(transformArgs));
                        break;
                    case "scale":
                        result.Add(CreateScaleTransformationFromArgs(transformArgs));
                        break;
                    case "rotate":
                        result.Add(CreateRotateTransformationFromArgs(transformArgs));
                        break;
                    case "scewX":
                    case "scewY":
                        throw new NotImplementedException("Skew transformations not implemented");
                }
                i = close;
            }

            return result;
        }

        private static SvgTransform CreateMatrixTransformationFromArgs(string args)
        {
            // matrix(<a> <b> <c> <d> <e> <f>), which specifies a transformation in the form of a transformation matrix of six values.
            var split = SplitStringOfNumbers(args);
            if (split.Length != 6)
            {
                throw new Exception($"Invalid matrix transformation arguments '{args}'");
            }
            var values = split.Select(float.Parse).ToArray();
            return new SvgTransform(SvgTransformType.Matrix, new SvgMatrix(values));
        }

        private static SvgTransform CreateScaleTransformationFromArgs(string args)
        {
            // scale(<sx> [<sy>]), which specifies a scale operation by sx and sy. If <sy> is not provided, it is assumed to be equal to <sx>.
            var split = SplitStringOfNumbers(args);
            if (split.Length < 1 || split.Length > 2)
            {
                throw new Exception($"Invalid scale transformation arguments '{args}'");
            }
            var x = float.Parse(split[0]);
            var y = split.Length == 2 ? float.Parse(split[1]) : x;
            return new SvgTransform(SvgTransformType.Scale, SvgMatrix.CreateScale(x, y));
        }

        private static SvgTransform CreateRotateTransformationFromArgs(string args)
        {
            // rotate(<rotate-angle> [<cx> <cy>]), which specifies a rotation by <rotate-angle> degrees about a given point.
            //
            // If optional parameters <cx> and <cy> are not supplied, the rotate is about the origin of the current user coordinate system. 
            //  The operation corresponds to the matrix [cos(a) sin(a) -sin(a) cos(a) 0 0].
            //
            // If optional parameters<cx> and<cy> are supplied, the rotate is about the point(cx, cy). 
            //  The operation represents the equivalent of the following specification: 
            //  translate(< cx >, < cy >) rotate(< rotate - angle >) translate(-< cx >, -< cy >).

            var split = SplitStringOfNumbers(args);
            if (split.Length != 1 && split.Length != 3)
            {
                throw new Exception($"Invalid rotate transformation arguments '{args}'");
            }
            var angle = float.Parse(split[0]);
            var cx = split.Length > 1 ? (float?)float.Parse(split[1]) : null;
            var cy = split.Length > 1 ? (float?)float.Parse(split[2]) : null;
            var matrix = SvgMatrix.CreateRotate(angle, cx, cy);
            return new SvgTransform(SvgTransformType.Rotate, matrix, angle);
        }

        private static SvgTransform CreateTranslateTransformationFromArgs(string args)
        {
            // translate(<tx> [<ty>]), which specifies a translation by tx and ty. If <ty> is not provided, it is assumed to be zero.
            var split = SplitStringOfNumbers(args);
            if (split.Length < 1 || split.Length > 2)
            {
                throw new Exception($"Invalid translate transformation arguments '{args}'");
            }
            var tx = float.Parse(split[0]);
            var ty = split.Length == 2 ? float.Parse(split[1]) : tx;
            return new SvgTransform(SvgTransformType.Translate, SvgMatrix.CreateTranslate(tx, ty));
        }

        private static string[] SplitStringOfNumbers(string str)
        {
            var result = new List<string>();
            var builder = new StringBuilder(16);

            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (c == ' ' || c == ',')
                {
                    if (builder.Length > 0)
                    {
                        result.Add(builder.ToString());
                        builder.Clear();
                    }

                    continue;
                }

                if (c == '-')
                {
                    if (builder.Length > 0)
                    {
                        result.Add(builder.ToString());
                        builder.Clear();
                    }
                }

                builder.Append(c);
            }

            if (builder.Length > 0)
            {
                result.Add(builder.ToString());
            }

            return result.ToArray();
        }
    }
}