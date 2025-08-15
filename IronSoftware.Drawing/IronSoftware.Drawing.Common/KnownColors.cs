using System.Collections.Generic;
using System.ComponentModel;

namespace IronSoftware.Drawing
{
    internal class KnownColors
    {
        internal static uint[] ArgbValues = new uint[] {
            0x00000000,	/* 000 - Empty */
			0xFFD4D0C8,	/* 001 - ActiveBorder */
			0xFF0054E3,	/* 002 - ActiveCaption */
			0xFFFFFFFF,	/* 003 - ActiveCaptionText */
			0xFF808080,	/* 004 - AppWorkspace */
			0xFFECE9D8,	/* 005 - Control */
			0xFFACA899,	/* 006 - ControlDark */
			0xFF716F64,	/* 007 - ControlDarkDark */
			0xFFF1EFE2,	/* 008 - ControlLight */
			0xFFFFFFFF,	/* 009 - ControlLightLight */
			0xFF000000,	/* 010 - ControlText */
			0xFF004E98,	/* 011 - Desktop */
			0xFFACA899,	/* 012 - GrayText */
			0xFF316AC5,	/* 013 - Highlight */
			0xFFFFFFFF,	/* 014 - HighlightText */
			0xFF000080,	/* 015 - HotTrack */
			0xFFD4D0C8,	/* 016 - InactiveBorder */
			0xFF7A96DF,	/* 017 - InactiveCaption */
			0xFFD8E4F8,	/* 018 - InactiveCaptionText */
			0xFFFFFFE1,	/* 019 - Info */
			0xFF000000,	/* 020 - InfoText */
			0xFFFFFFFF,	/* 021 - Menu */
			0xFF000000,	/* 022 - MenuText */
			0xFFD4D0C8,	/* 023 - ScrollBar */
			0xFFFFFFFF,	/* 024 - Window */
			0xFF000000,	/* 025 - WindowFrame */
			0xFF000000,	/* 026 - WindowText */
			0x00FFFFFF,	/* 027 - Transparent */
			0xFFF0F8FF,	/* 028 - AliceBlue */
			0xFFFAEBD7,	/* 029 - AntiqueWhite */
			0xFF00FFFF,	/* 030 - Aqua */
			0xFF7FFFD4,	/* 031 - Aquamarine */
			0xFFF0FFFF,	/* 032 - Azure */
			0xFFF5F5DC,	/* 033 - Beige */
			0xFFFFE4C4,	/* 034 - Bisque */
			0xFF000000,	/* 035 - Black */
			0xFFFFEBCD,	/* 036 - BlanchedAlmond */
			0xFF0000FF,	/* 037 - Blue */
			0xFF8A2BE2,	/* 038 - BlueViolet */
			0xFFA52A2A,	/* 039 - Brown */
			0xFFDEB887,	/* 040 - BurlyWood */
			0xFF5F9EA0,	/* 041 - CadetBlue */
			0xFF7FFF00,	/* 042 - Chartreuse */
			0xFFD2691E,	/* 043 - Chocolate */
			0xFFFF7F50,	/* 044 - Coral */
			0xFF6495ED,	/* 045 - CornflowerBlue */
			0xFFFFF8DC,	/* 046 - Cornsilk */
			0xFFDC143C,	/* 047 - Crimson */
			0xFF00FFFF,	/* 048 - Cyan */
			0xFF00008B,	/* 049 - DarkBlue */
			0xFF008B8B,	/* 050 - DarkCyan */
			0xFFB8860B,	/* 051 - DarkGoldenrod */
			0xFFA9A9A9,	/* 052 - DarkGray */
			0xFF006400,	/* 053 - DarkGreen */
			0xFFBDB76B,	/* 054 - DarkKhaki */
			0xFF8B008B,	/* 055 - DarkMagenta */
			0xFF556B2F,	/* 056 - DarkOliveGreen */
			0xFFFF8C00,	/* 057 - DarkOrange */
			0xFF9932CC,	/* 058 - DarkOrchid */
			0xFF8B0000,	/* 059 - DarkRed */
			0xFFE9967A,	/* 060 - DarkSalmon */
			0xFF8FBC8B,	/* 061 - DarkSeaGreen */
			0xFF483D8B,	/* 062 - DarkSlateBlue */
			0xFF2F4F4F,	/* 063 - DarkSlateGray */
			0xFF00CED1,	/* 064 - DarkTurquoise */
			0xFF9400D3,	/* 065 - DarkViolet */
			0xFFFF1493,	/* 066 - DeepPink */
			0xFF00BFFF,	/* 067 - DeepSkyBlue */
			0xFF696969,	/* 068 - DimGray */
			0xFF1E90FF,	/* 069 - DodgerBlue */
			0xFFB22222,	/* 070 - Firebrick */
			0xFFFFFAF0,	/* 071 - FloralWhite */
			0xFF228B22,	/* 072 - ForestGreen */
			0xFFFF00FF,	/* 073 - Fuchsia */
			0xFFDCDCDC,	/* 074 - Gainsboro */
			0xFFF8F8FF,	/* 075 - GhostWhite */
			0xFFFFD700,	/* 076 - Gold */
			0xFFDAA520,	/* 077 - Goldenrod */
			0xFF808080,	/* 078 - Gray */
			0xFF008000,	/* 079 - Green */
			0xFFADFF2F,	/* 080 - GreenYellow */
			0xFFF0FFF0,	/* 081 - Honeydew */
			0xFFFF69B4,	/* 082 - HotPink */
			0xFFCD5C5C,	/* 083 - IndianRed */
			0xFF4B0082,	/* 084 - Indigo */
			0xFFFFFFF0,	/* 085 - Ivory */
			0xFFF0E68C,	/* 086 - Khaki */
			0xFFE6E6FA,	/* 087 - Lavender */
			0xFFFFF0F5,	/* 088 - LavenderBlush */
			0xFF7CFC00,	/* 089 - LawnGreen */
			0xFFFFFACD,	/* 090 - LemonChiffon */
			0xFFADD8E6,	/* 091 - LightBlue */
			0xFFF08080,	/* 092 - LightCoral */
			0xFFE0FFFF,	/* 093 - LightCyan */
			0xFFFAFAD2,	/* 094 - LightGoldenrodYellow */
			0xFFD3D3D3,	/* 095 - LightGray */
			0xFF90EE90,	/* 096 - LightGreen */
			0xFFFFB6C1,	/* 097 - LightPink */
			0xFFFFA07A,	/* 098 - LightSalmon */
			0xFF20B2AA,	/* 099 - LightSeaGreen */
			0xFF87CEFA,	/* 100 - LightSkyBlue */
			0xFF778899,	/* 101 - LightSlateGray */
			0xFFB0C4DE,	/* 102 - LightSteelBlue */
			0xFFFFFFE0,	/* 103 - LightYellow */
			0xFF00FF00,	/* 104 - Lime */
			0xFF32CD32,	/* 105 - LimeGreen */
			0xFFFAF0E6,	/* 106 - Linen */
			0xFFFF00FF,	/* 107 - Magenta */
			0xFF800000,	/* 108 - Maroon */
			0xFF66CDAA,	/* 109 - MediumAquamarine */
			0xFF0000CD,	/* 110 - MediumBlue */
			0xFFBA55D3,	/* 111 - MediumOrchid */
			0xFF9370DB,	/* 112 - MediumPurple */
			0xFF3CB371,	/* 113 - MediumSeaGreen */
			0xFF7B68EE,	/* 114 - MediumSlateBlue */
			0xFF00FA9A,	/* 115 - MediumSpringGreen */
			0xFF48D1CC,	/* 116 - MediumTurquoise */
			0xFFC71585,	/* 117 - MediumVioletRed */
			0xFF191970,	/* 118 - MidnightBlue */
			0xFFF5FFFA,	/* 119 - MintCream */
			0xFFFFE4E1,	/* 120 - MistyRose */
			0xFFFFE4B5,	/* 121 - Moccasin */
			0xFFFFDEAD,	/* 122 - NavajoWhite */
			0xFF000080,	/* 123 - Navy */
			0xFFFDF5E6,	/* 124 - OldLace */
			0xFF808000,	/* 125 - Olive */
			0xFF6B8E23,	/* 126 - OliveDrab */
			0xFFFFA500,	/* 127 - Orange */
			0xFFFF4500,	/* 128 - OrangeRed */
			0xFFDA70D6,	/* 129 - Orchid */
			0xFFEEE8AA,	/* 130 - PaleGoldenrod */
			0xFF98FB98,	/* 131 - PaleGreen */
			0xFFAFEEEE,	/* 132 - PaleTurquoise */
			0xFFDB7093,	/* 133 - PaleVioletRed */
			0xFFFFEFD5,	/* 134 - PapayaWhip */
			0xFFFFDAB9,	/* 135 - PeachPuff */
			0xFFCD853F,	/* 136 - Peru */
			0xFFFFC0CB,	/* 137 - Pink */
			0xFFDDA0DD,	/* 138 - Plum */
			0xFFB0E0E6,	/* 139 - PowderBlue */
			0xFF800080,	/* 140 - Purple */
			0xFF663399,	/* 141 - RebeccaPurple */
			0xFFFF0000,	/* 142 - Red */
			0xFFBC8F8F,	/* 143 - RosyBrown */
			0xFF4169E1,	/* 144 - RoyalBlue */
			0xFF8B4513,	/* 145 - SaddleBrown */
			0xFFFA8072,	/* 146 - Salmon */
			0xFFF4A460,	/* 147 - SandyBrown */
			0xFF2E8B57,	/* 148 - SeaGreen */
			0xFFFFF5EE,	/* 149 - SeaShell */
			0xFFA0522D,	/* 150 - Sienna */
			0xFFC0C0C0,	/* 151 - Silver */
			0xFF87CEEB,	/* 152 - SkyBlue */
			0xFF6A5ACD,	/* 153 - SlateBlue */
			0xFF708090,	/* 154 - SlateGray */
			0xFFFFFAFA,	/* 155 - Snow */
			0xFF00FF7F,	/* 156 - SpringGreen */
			0xFF4682B4,	/* 157 - SteelBlue */
			0xFFD2B48C,	/* 158 - Tan */
			0xFF008080,	/* 159 - Teal */
			0xFFD8BFD8,	/* 160 - Thistle */
			0xFFFF6347,	/* 161 - Tomato */
			0xFF40E0D0,	/* 162 - Turquoise */
			0xFFEE82EE,	/* 163 - Violet */
			0xFFF5DEB3,	/* 164 - Wheat */
			0xFFFFFFFF,	/* 165 - White */
			0xFFF5F5F5,	/* 166 - WhiteSmoke */
			0xFFFFFF00,	/* 167 - Yellow */
			0xFF9ACD32,	/* 168 - YellowGreen */
			0xFFECE9D8,	/* 169 - ButtonFace */
			0xFFFFFFFF,	/* 170 - ButtonHighlight */
			0xFFACA899,	/* 171 - ButtonShadow */
			0xFF3D95FF,	/* 172 - GradientActiveCaption */
			0xFF9DB9EB,	/* 173 - GradientInactiveCaption */
			0xFFECE9D8,	/* 174 - MenuBar */
			0xFF316AC5,	/* 175 - MenuHighlight */
		};

        internal static string[] Names = {
            string.Empty,
            "ActiveBorder",
            "ActiveCaption",
            "ActiveCaptionText",
            "AppWorkspace",
            "Control",
            "ControlDark",
            "ControlDarkDark",
            "ControlLight",
            "ControlLightLight",
            "ControlText",
            "Desktop",
            "GrayText",
            "Highlight",
            "HighlightText",
            "HotTrack",
            "InactiveBorder",
            "InactiveCaption",
            "InactiveCaptionText",
            "Info",
            "InfoText",
            "Menu",
            "MenuText",
            "ScrollBar",
            "Window",
            "WindowFrame",
            "WindowText",
            "Transparent",
            "AliceBlue",
            "AntiqueWhite",
            "Aqua",
            "Aquamarine",
            "Azure",
            "Beige",
            "Bisque",
            "Black",
            "BlanchedAlmond",
            "Blue",
            "BlueViolet",
            "Brown",
            "BurlyWood",
            "CadetBlue",
            "Chartreuse",
            "Chocolate",
            "Coral",
            "CornflowerBlue",
            "Cornsilk",
            "Crimson",
            "Cyan",
            "DarkBlue",
            "DarkCyan",
            "DarkGoldenrod",
            "DarkGray",
            "DarkGreen",
            "DarkKhaki",
            "DarkMagenta",
            "DarkOliveGreen",
            "DarkOrange",
            "DarkOrchid",
            "DarkRed",
            "DarkSalmon",
            "DarkSeaGreen",
            "DarkSlateBlue",
            "DarkSlateGray",
            "DarkTurquoise",
            "DarkViolet",
            "DeepPink",
            "DeepSkyBlue",
            "DimGray",
            "DodgerBlue",
            "Firebrick",
            "FloralWhite",
            "ForestGreen",
            "Fuchsia",
            "Gainsboro",
            "GhostWhite",
            "Gold",
            "Goldenrod",
            "Gray",
            "Green",
            "GreenYellow",
            "Honeydew",
            "HotPink",
            "IndianRed",
            "Indigo",
            "Ivory",
            "Khaki",
            "Lavender",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "LimeGreen",
            "Linen",
            "Magenta",
            "Maroon",
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "MediumTurquoise",
            "MediumVioletRed",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "Olive",
            "OliveDrab",
            "Orange",
            "OrangeRed",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "Purple",
            "RebeccaPurple",
            "Red",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SandyBrown",
            "SeaGreen",
            "SeaShell",
            "Sienna",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Yellow",
            "YellowGreen",
            "ButtonFace",
            "ButtonHighlight",
            "ButtonShadow",
            "GradientActiveCaption",
            "GradientInactiveCaption",
            "MenuBar",
            "MenuHighlight"
        };
        private static Dictionary<string, uint> _argbByName = null;
        private static Dictionary<uint, string> _nameByArgb = null;
        private static Dictionary<string, KnownColor> _knownColorByName = null;

        internal static Dictionary<string, uint> ArgbByName
        {
            get
            {
                if (_argbByName == null)
                {
                    _argbByName = new Dictionary<string, uint>();
                    for (int i = 0; i < ArgbValues.Length; ++i)
                    {
                        _argbByName[Names[i].ToLower()] = ArgbValues[i];
                    }
                }

                return _argbByName;
            }
        }

        internal static Dictionary<uint, string> NameByArgb
        {
            get
            {
                if (_nameByArgb == null)
                {
                    _nameByArgb = new Dictionary<uint, string>();
                    for (int i = 0; i < Names.Length; ++i)
                    {
                        _nameByArgb[ArgbValues[i]] = Names[i];
                    }
                }

                return _nameByArgb;
            }
        }

        internal static Dictionary<string, KnownColor> KnownColorByName
        {
            get
            {
                if (_knownColorByName == null)
                {
                    _knownColorByName = new Dictionary<string, KnownColor>();
                    for (int i = 0; i < Names.Length; ++i)
                    {
                        // The key is the lowercased name for case-insensitive matching
                        _knownColorByName[Names[i].ToLower()] = (KnownColor)i;
                    }
                }
                return _knownColorByName;
            }
        }

        public static Color FromKnownColor(KnownColor kc)
        {
            short n = (short)kc;
            if ((n <= 0) || (n >= ArgbValues.Length))
            {
                // For invalid KnownColor enum values, return transparent black, which is not a "known" color.
                return new Color(0, 0, 0, 0);
            }

            // Create a color and set its immutable IsKnownColor flag to true.
            return new Color((int)ArgbValues[n], true);
        }

        public static string GetName(short kc)
        {
            if (kc > 0 && kc < Names.Length)
            {
                return Names[kc];
            }

            return string.Empty;
        }

        public static string GetName(KnownColor kc)
        {
            return GetName((short)kc);
        }

        public static Color FindColorMatch(Color c)
        {
            uint argb = (uint)c.ToArgb();
            for (int i = 0; i < ArgbValues.Length; i++)
            {
                if (argb == ArgbValues[i])
                {
                    return FromKnownColor((KnownColor)i);
                }
            }

            return Color.Empty;
        }

        // When this method is called, we teach any new color(s) to the Color class
        // NOTE: This is called (reflection) by System.Windows.Forms.Theme (this isn't dead code)
        [Browsable(false)]
        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Update(int knownColor, int color)
        {
            ArgbValues[knownColor] = (uint)color;
        }
    }
}
