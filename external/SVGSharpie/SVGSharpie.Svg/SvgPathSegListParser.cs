using System;
using System.Text;

namespace SVGSharpie
{
    /// <summary>
    /// Provides support for reading the path markup syntax
    /// </summary>
    internal static class SvgPathSegListParser
    {
        /// <summary>
        /// Parses the data in string specified and returns the list of commands found therein
        /// </summary>
        public static SvgPathSegList Parse(string markup)
        {
            if (markup == null) throw new ArgumentNullException(nameof(markup));

            var result = new SvgPathSegList();
            var buffer = new StringBuilder();
            var lastCommand = '\0';
            var args = new float[7];

            var stream = new MarkupStringStream(markup);

            while (!stream.IsAtEnd)
            {
                if (!stream.SkipWhitespaceUntilNextToken()) break;
                var isNewCommand = char.IsLetter(stream.Peek);
                var command = lastCommand = isNewCommand ? stream.Read() : lastCommand;
                var argCount = GetPathCommandArgs(command);

                for (var arg = 0; arg < argCount; arg++)
                {
                    buffer.Clear();

                    stream.SkipWhitespaceUntilNextToken();

                    var gotDot = false;

                    while (!stream.IsAtEnd && IsNumeric(stream.Peek))
                    {
                        var c = stream.Peek;
                        if (((c == '-' || c == '+') && buffer.Length > 0) || (c == '.' && gotDot))
                        {
                            var last = buffer[buffer.Length - 1];
                            if (last != 'e' && last != 'E')
                            {
                                break;
                            }
                        }

                        gotDot |= c == '.';
                        buffer.Append(stream.Read());
                    }

                    args[arg] = float.Parse(buffer.ToString());
                }

                result.Add(CreatePathSegment(command, isNewCommand, args));
            }

            return result;
        }

        private static bool IsNumeric(char c) => char.IsDigit(c) || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E';

        private static SvgPathSeg CreatePathSegment(char command, bool isNewCommand, float[] args)
        {
            var relative = char.IsLower(command);
            switch (char.ToUpper(command))
            {
                case 'M':
                    //
                    // 8.3.2 The "moveto" commands
                    // If a moveto is followed by multiple pairs of coordinates, the subsequent pairs are treated as implicit lineto commands.
                    //
                    return isNewCommand
                        ? CreateMoveto(args[0], args[1], relative)
                        : CreateLineto(args[0], args[1], relative);
                case 'L':
                    return CreateLineto(args[0], args[1], relative);
                case 'V':
                    return CreateVerticalLineto(args[0], relative);
                case 'H':
                    return CreateHorizontalLineto(args[0], relative);
                case 'Z':
                    return new SvgPathSegClosePath();
                case 'C':
                    return CreateCubicCurveto(args[0], args[1], args[2], args[3], args[4], args[5], relative);
                case 'S':
                    return CreateCurvetoCubicSmoothRel(args[0], args[1], args[2], args[3], relative);
                case 'Q':
                    return CreateQuadraticCurveto(args[0], args[1], args[2], args[3], relative);
                case 'T':
                    return CreateCurvetoQuadraticSmoothRel(args[0], args[1], relative);
                case 'A':
                    return CreateArc(args[0], args[1], args[2], ConvertToFlag(args[3]), ConvertToFlag(args[4]), args[5], args[6], relative);
                default:
                    throw new InvalidOperationException($"Invalid svg path command '{command}'");
            }
        }

        private static SvgPathSeg CreateMoveto(float x, float y, bool relative) =>
            relative ? new SvgPathSegMovetoRel(x, y) as SvgPathSeg : new SvgPathSegMovetoAbs(x, y);

        private static SvgPathSeg CreateCubicCurveto(float x1, float y1, float x2, float y2, float x, float y, bool relative) =>
            relative ? new SvgPathSegCurvetoCubicRel(x1, y1, x2, y2, x, y) as SvgPathSeg : new SvgPathSegCurvetoCubicAbs(x1, y1, x2, y2, x, y);

        private static SvgPathSeg CreateCurvetoCubicSmoothRel(float x2, float y2, float x, float y, bool relative) =>
            relative ? new SvgPathSegCurvetoCubicSmoothRel(x2, y2, x, y) as SvgPathSeg : new SvgPathSegCurvetoCubicSmoothAbs(x2, y2, x, y);

        private static SvgPathSeg CreateQuadraticCurveto(float x1, float y1, float x, float y, bool relative) =>
            relative ? new SvgPathSegCurvetoQuadraticRel(x1, y1, x, y) as SvgPathSeg : new SvgPathSegCurvetoQuadraticAbs(x1, y1, x, y);

        private static SvgPathSeg CreateCurvetoQuadraticSmoothRel(float x, float y, bool relative) =>
            relative ? new SvgPathSegCurvetoQuadraticSmoothRel(x, y) as SvgPathSeg : new SvgPathSegCurvetoQuadraticSmoothAbs(x, y);

        private static SvgPathSeg CreateArc(float rx, float ry, float angle, bool largeArcFlag, bool sweepFlag, float x, float y, bool relative) =>
            relative ? new SvgPathSegArcRel(rx, ry, angle, largeArcFlag, sweepFlag, x, y) as SvgPathSeg : new SvgPathSegArcAbs(rx, ry, angle, largeArcFlag, sweepFlag, x, y);

        private static SvgPathSeg CreateLineto(float x, float y, bool relative) =>
            relative ? new SvgPathSegLinetoRel(x, y) as SvgPathSeg : new SvgPathSegLinetoAbs(x, y);

        private static SvgPathSeg CreateHorizontalLineto(float x, bool relative) =>
            relative ? new SvgPathSegLinetoHorizontalRel(x) as SvgPathSeg : new SvgPathSegLinetoHorizontalAbs(x);

        private static SvgPathSeg CreateVerticalLineto(float y, bool relative) =>
            relative ? new SvgPathSegLinetoVerticalRel(y) as SvgPathSeg : new SvgPathSegLinetoVerticalAbs(y);

        private static bool ConvertToFlag(double x) => (int)x == 1;

        private static int GetPathCommandArgs(char command)
        {
            switch (char.ToUpper(command))
            {
                case 'Z':
                    return 0;
                case 'V':
                case 'H':
                    return 1;
                case 'M':
                case 'L':
                case 'T':
                    return 2;
                case 'S':
                case 'Q':
                    return 4;
                case 'C':
                    return 6;
                case 'A':
                    return 7;
                default:
                    throw new InvalidOperationException($"Invalid svg path segment command '{command}'");
            }
        }

        private sealed class MarkupStringStream
        {
            private readonly string _data;

            private int _position;

            public bool IsAtEnd => _position >= _data.Length;

            public char Peek => _data[_position];

            public MarkupStringStream(string data)
            {
                _data = data;
            }

            public char Read()
            {
                return _data[_position++];
            }

            public bool SkipWhitespaceUntilNextToken()
            {
                //
                // 8.3.1 General information about path data
                // Superfluous white space and separators such as commas can be eliminated
                //
                while (_position < _data.Length && (char.IsWhiteSpace(_data[_position]) || _data[_position] == ','))
                {
                    _position++;
                }
                return _position < _data.Length;
            }
        }
    }
}