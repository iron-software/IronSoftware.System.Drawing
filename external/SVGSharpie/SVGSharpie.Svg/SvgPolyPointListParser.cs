using System;
using System.Collections.Generic;

namespace SVGSharpie
{
    internal static class SvgPolyPointListParser
    {
        // SVG Spec: 9.7.1 The grammar for points specifications in ‘polyline’ and ‘polygon’ elements
        private static readonly char[] WhiteSpaceAndComma = { ' ', '\r', '\n', '\t', ',' };

        public static SvgPolyPointList Parse(string points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            var split = points.Split(WhiteSpaceAndComma, StringSplitOptions.RemoveEmptyEntries);
            var result = new SvgPolyPointList();
            var coords = new List<float>(2);

            void AddCoordsToResult()
            {
                if (coords.Count != 2) return;
                result.Add(new SvgPolyPoint(coords[0], coords[1]));
                coords.Clear();
            }

            foreach (var point in split)
            {
                AddCoordsToResult();
                if (float.TryParse(point, out var coord))
                {
                    coords.Add(coord);
                    continue;
                }

                //
                // it is possible for coordinates to not have an explicit separator if the second 
                // coordinate in the pair is a negative value, as per the grammar...
                //
                //   coordinate-pair: coordinate comma-wsp coordinate | coordinate negative - coordinate
                //

                if (coords.Count != 0)
                {
                    // can't have any coordinates on the stack already
                    throw new Exception($"Invalid coordinate '{point}'");
                }

                var signIndex = point.IndexOf('-', 1);
                while (signIndex > 0 && char.ToLowerInvariant(point[signIndex - 1]) == 'e')
                {
                    // validate that the sign doesn't appear after an exponent
                    signIndex = point.IndexOf('-', signIndex + 1);
                }
                if (signIndex <= 0)
                {
                    throw new Exception($"Invalid coordinate '{point}'");
                }

                coords.Add(float.Parse(point.Substring(0, signIndex)));
                coords.Add(float.Parse(point.Substring(signIndex + 1)));
            }

            AddCoordsToResult();
            return result;
        }
    }
}