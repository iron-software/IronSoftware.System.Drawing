using System;
using System.Collections.Generic;
using System.Globalization;

namespace SVGSharpie
{
    public static class SvgColorTranslator
    {
        public static SvgColor FromSvgColorCode(string color)
        {
            if (color == null) throw new ArgumentNullException(nameof(color));
            if (color.StartsWith("rgb", StringComparison.OrdinalIgnoreCase) || color.StartsWith("rgba", StringComparison.OrdinalIgnoreCase))
            {
                var hasAlpha = color.StartsWith("rgba", StringComparison.OrdinalIgnoreCase);
                var length = 3;
                if (hasAlpha) { length = 4; }
                var openIndex = color.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1;
                var closeIndex = color.IndexOf(")", openIndex, StringComparison.OrdinalIgnoreCase);
                if (openIndex > 0 && closeIndex > 0)
                {
                    var values = color.Substring(openIndex, closeIndex - openIndex).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length != length)
                    {
                        throw new ArgumentException($"Invalid color '{color}'", nameof(color));
                    }

                    var firstValue = values[0].Trim();
                    if (firstValue.EndsWith("%"))
                    {
                        byte ParsePercent(string str)
                        {
                            var trimmed = str.Trim().TrimEnd('%');
                            var percent = Math.Max(0, Math.Min(100, float.Parse(trimmed)));
                            var value = Math.Floor(255 * (percent / 100));
                            return (byte)value;
                        }

                        return new SvgColor(ParsePercent(firstValue), ParsePercent(values[1]), ParsePercent(values[2]));
                    }

                    var r = Math.Max(0, Math.Min(255, int.Parse(firstValue)));
                    var g = Math.Max(0, Math.Min(255, int.Parse(values[1].Trim())));
                    var b = Math.Max(0, Math.Min(255, int.Parse(values[2].Trim())));
                    if (hasAlpha)
                    {
                        var a = Math.Max(0, Math.Min(255, int.Parse(values[3].Trim())));
                        return new SvgColor((byte)r, (byte)g, (byte)b, (byte)a);
                    }
                    else
                    {
                        return new SvgColor((byte)r, (byte)g, (byte)b);
                    }
                }
            }
            if (!color.StartsWith("#"))
            {
                if (!NamedKnownColors.TryGetValue(color, out var result))
                {
                    throw new ArgumentException($"Unknown color '{color}'", nameof(color));
                }
                return result;
            }
            if (color.Length == 4)      // #rgb
            {
                var r = byte.Parse(color.Substring(1, 1), NumberStyles.HexNumber);
                var g = byte.Parse(color.Substring(2, 1), NumberStyles.HexNumber);
                var b = byte.Parse(color.Substring(3, 1), NumberStyles.HexNumber);
                return new SvgColor((byte)((r << 4) | r), (byte)((g << 4) | g), (byte)((b << 4) | b));
            }
            if (color.Length == 5)      // #rgba
            {
                var r = byte.Parse(color.Substring(1, 1), NumberStyles.HexNumber);
                var g = byte.Parse(color.Substring(2, 1), NumberStyles.HexNumber);
                var b = byte.Parse(color.Substring(3, 1), NumberStyles.HexNumber);
                var a = byte.Parse(color.Substring(4, 1), NumberStyles.HexNumber);
                return new SvgColor((byte)((r << 4) | r), (byte)((g << 4) | g), (byte)((b << 4) | b));
            }
            if (color.Length == 7)      // #rrggbb
            {
                var r = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
                var g = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
                var b = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);
                return new SvgColor(r, g, b);
            }
            if (color.Length == 9)      // #rrggbbaa
            {
                var r = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
                var g = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
                var b = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);
                var a = byte.Parse(color.Substring(7, 2), NumberStyles.HexNumber);
                return new SvgColor(r, g, b, a);
            }
            throw new ArgumentException($"invalid color '{color}'", nameof(color));
        }

        //
        // SVG Spec - Recognized color keyword names
        // https://www.w3.org/TR/2003/REC-SVG11-20030114/types.html#ColorKeywords
        //

        private static readonly Dictionary<string, SvgColor> NamedKnownColors = new Dictionary<string, SvgColor>(StringComparer.OrdinalIgnoreCase)
        {
            ["transparent"] = new SvgColor(0, 0, 0, 0),

            ["aliceblue"] = new SvgColor(240, 248, 255),
            ["antiquewhite"] = new SvgColor(250, 235, 215),
            ["aqua"] = new SvgColor(0, 255, 255),
            ["aquamarine"] = new SvgColor(127, 255, 212),
            ["azure"] = new SvgColor(240, 255, 255),
            ["beige"] = new SvgColor(245, 245, 220),
            ["bisque"] = new SvgColor(255, 228, 196),
            ["black"] = new SvgColor(0, 0, 0),
            ["blanchedalmond"] = new SvgColor(255, 235, 205),
            ["blue"] = new SvgColor(0, 0, 255),
            ["blueviolet"] = new SvgColor(138, 43, 226),
            ["brown"] = new SvgColor(165, 42, 42),
            ["burlywood"] = new SvgColor(222, 184, 135),
            ["cadetblue"] = new SvgColor(95, 158, 160),
            ["chartreuse"] = new SvgColor(127, 255, 0),
            ["chocolate"] = new SvgColor(210, 105, 30),
            ["coral"] = new SvgColor(255, 127, 80),
            ["cornflowerblue"] = new SvgColor(100, 149, 237),
            ["cornsilk"] = new SvgColor(255, 248, 220),
            ["crimson"] = new SvgColor(220, 20, 60),
            ["cyan"] = new SvgColor(0, 255, 255),
            ["darkblue"] = new SvgColor(0, 0, 139),
            ["darkcyan"] = new SvgColor(0, 139, 139),
            ["darkgoldenrod"] = new SvgColor(184, 134, 11),
            ["darkgray"] = new SvgColor(169, 169, 169),
            ["darkgreen"] = new SvgColor(0, 100, 0),
            ["darkgrey"] = new SvgColor(169, 169, 169),
            ["darkkhaki"] = new SvgColor(189, 183, 107),
            ["darkmagenta"] = new SvgColor(139, 0, 139),
            ["darkolivegreen"] = new SvgColor(85, 107, 47),
            ["darkorange"] = new SvgColor(255, 140, 0),
            ["darkorchid"] = new SvgColor(153, 50, 204),
            ["darkred"] = new SvgColor(139, 0, 0),
            ["darksalmon"] = new SvgColor(233, 150, 122),
            ["darkseagreen"] = new SvgColor(143, 188, 143),
            ["darkslateblue"] = new SvgColor(72, 61, 139),
            ["darkslategray"] = new SvgColor(47, 79, 79),
            ["darkslategrey"] = new SvgColor(47, 79, 79),
            ["darkturquoise"] = new SvgColor(0, 206, 209),
            ["darkviolet"] = new SvgColor(148, 0, 211),
            ["deeppink"] = new SvgColor(255, 20, 147),
            ["deepskyblue"] = new SvgColor(0, 191, 255),
            ["dimgray"] = new SvgColor(105, 105, 105),
            ["dimgrey"] = new SvgColor(105, 105, 105),
            ["dodgerblue"] = new SvgColor(30, 144, 255),
            ["firebrick"] = new SvgColor(178, 34, 34),
            ["floralwhite"] = new SvgColor(255, 250, 240),
            ["forestgreen"] = new SvgColor(34, 139, 34),
            ["fuchsia"] = new SvgColor(255, 0, 255),
            ["gainsboro"] = new SvgColor(220, 220, 220),
            ["ghostwhite"] = new SvgColor(248, 248, 255),
            ["gold"] = new SvgColor(255, 215, 0),
            ["goldenrod"] = new SvgColor(218, 165, 32),
            ["gray"] = new SvgColor(128, 128, 128),
            ["grey"] = new SvgColor(128, 128, 128),
            ["green"] = new SvgColor(0, 128, 0),
            ["greenyellow"] = new SvgColor(173, 255, 47),
            ["honeydew"] = new SvgColor(240, 255, 240),
            ["hotpink"] = new SvgColor(255, 105, 180),
            ["indianred"] = new SvgColor(205, 92, 92),
            ["indigo"] = new SvgColor(75, 0, 130),
            ["ivory"] = new SvgColor(255, 255, 240),
            ["khaki"] = new SvgColor(240, 230, 140),
            ["lavender"] = new SvgColor(230, 230, 250),
            ["lavenderblush"] = new SvgColor(255, 240, 245),
            ["lawngreen"] = new SvgColor(124, 252, 0),
            ["lemonchiffon"] = new SvgColor(255, 250, 205),
            ["lightblue"] = new SvgColor(173, 216, 230),
            ["lightcoral"] = new SvgColor(240, 128, 128),
            ["lightcyan"] = new SvgColor(224, 255, 255),
            ["lightgoldenrodyellow"] = new SvgColor(250, 250, 210),
            ["lightgray"] = new SvgColor(211, 211, 211),
            ["lightgreen"] = new SvgColor(144, 238, 144),
            ["lightgrey"] = new SvgColor(211, 211, 211),
            ["lightpink"] = new SvgColor(255, 182, 193),
            ["lightsalmon"] = new SvgColor(255, 160, 122),
            ["lightseagreen"] = new SvgColor(32, 178, 170),
            ["lightskyblue"] = new SvgColor(135, 206, 250),
            ["lightslategray"] = new SvgColor(119, 136, 153),
            ["lightslategrey"] = new SvgColor(119, 136, 153),
            ["lightsteelblue"] = new SvgColor(176, 196, 222),
            ["lightyellow"] = new SvgColor(255, 255, 224),
            ["lime"] = new SvgColor(0, 255, 0),
            ["limegreen"] = new SvgColor(50, 205, 50),
            ["linen"] = new SvgColor(250, 240, 230),
            ["magenta"] = new SvgColor(255, 0, 255),
            ["maroon"] = new SvgColor(128, 0, 0),
            ["mediumaquamarine"] = new SvgColor(102, 205, 170),
            ["mediumblue"] = new SvgColor(0, 0, 205),
            ["mediumorchid"] = new SvgColor(186, 85, 211),
            ["mediumpurple"] = new SvgColor(147, 112, 219),
            ["mediumseagreen"] = new SvgColor(60, 179, 113),
            ["mediumslateblue"] = new SvgColor(123, 104, 238),
            ["mediumspringgreen"] = new SvgColor(0, 250, 154),
            ["mediumturquoise"] = new SvgColor(72, 209, 204),
            ["mediumvioletred"] = new SvgColor(199, 21, 133),
            ["midnightblue"] = new SvgColor(25, 25, 112),
            ["mintcream"] = new SvgColor(245, 255, 250),
            ["mistyrose"] = new SvgColor(255, 228, 225),
            ["moccasin"] = new SvgColor(255, 228, 181),
            ["navajowhite"] = new SvgColor(255, 222, 173),
            ["navy"] = new SvgColor(0, 0, 128),
            ["oldlace"] = new SvgColor(253, 245, 230),
            ["olive"] = new SvgColor(128, 128, 0),
            ["olivedrab"] = new SvgColor(107, 142, 35),
            ["orange"] = new SvgColor(255, 165, 0),
            ["orangered"] = new SvgColor(255, 69, 0),
            ["orchid"] = new SvgColor(218, 112, 214),
            ["palegoldenrod"] = new SvgColor(238, 232, 170),
            ["palegreen"] = new SvgColor(152, 251, 152),
            ["paleturquoise"] = new SvgColor(175, 238, 238),
            ["palevioletred"] = new SvgColor(219, 112, 147),
            ["papayawhip"] = new SvgColor(255, 239, 213),
            ["peachpuff"] = new SvgColor(255, 218, 185),
            ["peru"] = new SvgColor(205, 133, 63),
            ["pink"] = new SvgColor(255, 192, 203),
            ["plum"] = new SvgColor(221, 160, 221),
            ["powderblue"] = new SvgColor(176, 224, 230),
            ["purple"] = new SvgColor(128, 0, 128),
            ["red"] = new SvgColor(255, 0, 0),
            ["rosybrown"] = new SvgColor(188, 143, 143),
            ["royalblue"] = new SvgColor(65, 105, 225),
            ["saddlebrown"] = new SvgColor(139, 69, 19),
            ["salmon"] = new SvgColor(250, 128, 114),
            ["sandybrown"] = new SvgColor(244, 164, 96),
            ["seagreen"] = new SvgColor(46, 139, 87),
            ["seashell"] = new SvgColor(255, 245, 238),
            ["sienna"] = new SvgColor(160, 82, 45),
            ["silver"] = new SvgColor(192, 192, 192),
            ["skyblue"] = new SvgColor(135, 206, 235),
            ["slateblue"] = new SvgColor(106, 90, 205),
            ["slategray"] = new SvgColor(112, 128, 144),
            ["slategrey"] = new SvgColor(112, 128, 144),
            ["snow"] = new SvgColor(255, 250, 250),
            ["springgreen"] = new SvgColor(0, 255, 127),
            ["steelblue"] = new SvgColor(70, 130, 180),
            ["tan"] = new SvgColor(210, 180, 140),
            ["teal"] = new SvgColor(0, 128, 128),
            ["thistle"] = new SvgColor(216, 191, 216),
            ["tomato"] = new SvgColor(255, 99, 71),
            ["turquoise"] = new SvgColor(64, 224, 208),
            ["violet"] = new SvgColor(238, 130, 238),
            ["wheat"] = new SvgColor(245, 222, 179),
            ["white"] = new SvgColor(255, 255, 255),
            ["whitesmoke"] = new SvgColor(245, 245, 245),
            ["yellow"] = new SvgColor(255, 255, 0),
            ["yellowgreen"] = new SvgColor(154, 205, 50),
        };
    }
}