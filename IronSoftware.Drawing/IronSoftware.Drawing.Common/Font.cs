using System;
using System.Linq;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// Defines a particular format for text, including font face, size, and style attributes.
    /// </summary>
    public partial class Font
    {

        /// <summary>
        /// Gets the family name for the typeface.
        /// </summary>
        public string FamilyName { get; internal set; }

        /// <summary>
        /// Gets the font style for the typeface.
        /// </summary>
        public FontStyle Style { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether this Font is bold.
        /// </summary>
        public bool Bold
        {
            get
            {
                return Style.HasFlag(FontStyle.Bold);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this font has the italic style applied.
        /// </summary>
        public bool Italic
        {
            get
            {
                return Style.HasFlag(FontStyle.Italic);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this Font is underlined.
        /// </summary>
        public bool Underline
        {
            get
            {
                return Style.HasFlag(FontStyle.Underline);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this Font specifies a horizontal line through the font.
        /// </summary>
        public bool Strikeout
        {
            get
            {
                return Style.HasFlag(FontStyle.Strikeout);
            }
        }

        /// <summary>
        /// Gets the "em-size of this Font measured in the units specified by the Unit property.
        /// </summary>
        public float Size { get; internal set; } = 12;

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName.
        /// <br/><para><b>Further Documentation:</b><br/><a href="https://ironsoftware.com/open-source/csharp/drawing/examples/create-and-cast-font/">Code Example</a></para>
        /// </summary>
        /// <param name="familyName">The FontFamily of the new Font.</param>
        public Font(string familyName)
        {
            FamilyName = familyName;
        }

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName and FontStyle enumeration.
        /// <br/><para><b>Further Documentation:</b><br/><a href="https://ironsoftware.com/open-source/csharp/drawing/examples/create-and-cast-font/">Code Example</a></para>
        /// </summary>
        /// <param name="familyName">The FontFamily of the new Font.</param>
        /// <param name="style">The FontStyle to apply to the new Font. Multiple values of the FontStyle enumeration can be combined with the OR operator.</param>
        public Font(string familyName, FontStyle style)
        {
            FamilyName = familyName;
            Style = style;
        }

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName, FontStyle enumeration, FontWeight, Bold, Italic and Size.
        /// <br/><para><b>Further Documentation:</b><br/><a href="https://ironsoftware.com/open-source/csharp/drawing/examples/create-and-cast-font/">Code Example</a></para>
        /// </summary>
        /// <param name="familyName">The FontFamily of the new Font.</param>
        /// <param name="style">The FontStyle to apply to the new Font. Multiple values of the FontStyle enumeration can be combined with the OR operator.</param>
        /// <param name="size">The em-size of the new font in the units specified by the unit parameter.</param>
        public Font(string familyName, FontStyle style, float size)
        {
            FamilyName = familyName;
            Style = style;
            Size = size;
        }

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName, FontWeight, Bold, Italic and Size.
        /// <br/><para><b>Further Documentation:</b><br/><a href="https://ironsoftware.com/open-source/csharp/drawing/examples/create-and-cast-font/">Code Example</a></para>
        /// </summary>
        /// <param name="familyName">The FontFamily of the new Font.</param>
        /// <param name="size">The em-size of the new font in the units specified by the unit parameter.</param>
        public Font(string familyName, float size)
        {
            FamilyName = familyName;
            Size = size;
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Font objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font">System.Drawing.Font will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(System.Drawing.Font font)
        {
            return new Font(font.FontFamily.Name, (FontStyle)font.Style, font.Size);
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font"><see cref="Font"/> is explicitly cast to a System.Drawing.Font </param>
        public static implicit operator System.Drawing.Font(Font font)
        {
            return new System.Drawing.Font(new System.Drawing.FontFamily(font.FamilyName), font.Size, (System.Drawing.FontStyle)font.Style);
        }

        /// <summary>
        /// Implicitly casts SixLabors.Fonts.Font objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font">SixLabors.Fonts.Font will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(SixLabors.Fonts.Font font)
        {
            FontStyle fontStyle;
            if (font.IsBold && font.IsItalic)
            {
                fontStyle = FontStyle.Bold | FontStyle.Italic;
            }
            else if (font.IsBold)
            {
                fontStyle = FontStyle.Bold;
            }
            else if (font.IsItalic)
            {
                fontStyle = FontStyle.Italic;
            }
            else
            {
                fontStyle = FontStyle.Regular;
            }

            return new Font(font.Family.Name, fontStyle, font.Size);
        }

        /// <summary>
        /// Implicitly casts to SixLabors.Fonts.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font"><see cref="Font"/> is explicitly cast to a SixLabors.Fonts.Font </param>
        public static implicit operator SixLabors.Fonts.Font(Font font)
        {
            return SixLabors.Fonts.SystemFonts.CreateFont(font.FamilyName, font.Size, (SixLabors.Fonts.FontStyle)font.Style);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKFont objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font">SkiaSharp.SKFont will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(SkiaSharp.SKFont font)
        {
            FontStyle fontStyle;
            if (font.Typeface.IsBold && font.Typeface.IsItalic)
            {
                fontStyle = FontStyle.Bold | FontStyle.Italic;
            }
            else if (font.Typeface.IsBold)
            {
                fontStyle = FontStyle.Bold;
            }
            else if (font.Typeface.IsItalic)
            {
                fontStyle = FontStyle.Italic;
            }
            else
            {
                fontStyle = FontStyle.Regular;
            }

            var result = new Font(font.Typeface.FamilyName, fontStyle, font.Size);

            return result;
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font"><see cref="Font"/> is explicitly cast to a SkiaSharp.SKFont </param>
        public static implicit operator SkiaSharp.SKFont(Font font)
        {
            SkiaSharp.SKFontStyle sKFontStyle = font.Style switch
            {
                FontStyle.Bold => SkiaSharp.SKFontStyle.Bold,
                FontStyle.Italic => SkiaSharp.SKFontStyle.Italic,
                FontStyle.Bold | FontStyle.Italic => SkiaSharp.SKFontStyle.BoldItalic,
                _ => SkiaSharp.SKFontStyle.Normal,
            };

            return new SkiaSharp.SKFont(SkiaSharp.SKTypeface.FromFamilyName(font.FamilyName, sKFontStyle), font.Size);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Font objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font">Microsoft.Maui.Graphics.Font will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(Microsoft.Maui.Graphics.Font font)
        {
            FontStyle style;
            if (font.Weight >= 700 && (font.StyleType == Microsoft.Maui.Graphics.FontStyleType.Italic || font.StyleType == Microsoft.Maui.Graphics.FontStyleType.Oblique))
            {
                style = FontStyle.Bold | FontStyle.Italic;
            }
            else if (font.Weight >= 700)
            {
                style = FontStyle.Bold;
            }
            else if (font.StyleType is Microsoft.Maui.Graphics.FontStyleType.Italic or Microsoft.Maui.Graphics.FontStyleType.Oblique)
            {
                style = FontStyle.Italic;
            }
            else
            {
                style = FontStyle.Regular;
            }

            return new Font(font.Name, style);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font"><see cref="Font"/> is explicitly cast to a Microsoft.Maui.Graphics.Font </param>
        public static implicit operator Microsoft.Maui.Graphics.Font(Font font)
        {
            int fontWeight = 400;
            Microsoft.Maui.Graphics.FontStyleType fontStyleType = Microsoft.Maui.Graphics.FontStyleType.Normal;
            if (font.Bold)
            {
                fontWeight = 700;
            }

            if (font.Italic)
            {
                fontStyleType = Microsoft.Maui.Graphics.FontStyleType.Italic;
            }

            return new Microsoft.Maui.Graphics.Font(font.FamilyName, fontWeight, fontStyleType);
        }

        /// <summary>
        /// Implicitly casts IronPdf.Font.FontTypes objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="fontTypes">IronPdf.Font.FontTypes will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(IronPdf.Font.FontTypes fontTypes)
        {
            FontStyle style;
            string[] names = fontTypes.Name.Split('-');
            if (names.Length > 1)
            {
                style = names[1] switch
                {
                    "Bold" => FontStyle.Bold,
                    "Italic" or "Oblique" => FontStyle.Italic,
                    "BoldItalic" or "BoldOblique" => FontStyle.BoldItalic,
                    _ => FontStyle.Regular,
                };
            }
            else
            {
                style = FontStyle.Regular;
            }

            string fontName = fontTypes.Name.Split('-')[0] switch
            {
                "CourierNew" => "Courier New",
                "TimesNewRoman" => "Times New Roman",
                _ => fontTypes.Name.Split('-')[0]
            };

            return new Font(fontName, style);
        }

        private static readonly string[] _obliqueFonts = { "Courier", "Helvetica" };

        /// <summary>
        /// Implicitly casts to IronPdf.Font.FontTypes objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods use <see cref="Font"/> as parameters or return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="font"><see cref="Font"/> is explicitly cast to a IronPdf.Font.FontTypes </param>
        public static implicit operator IronPdf.Font.FontTypes(Font font)
        {
            string fontName = font.FamilyName.Replace(" ", "");
            fontName += font.Style switch
            {
                FontStyle.Bold => "-Bold",
                FontStyle.Italic => _obliqueFonts.Contains(fontName) ? "-Oblique" : "-Italic",
                FontStyle.BoldItalic => _obliqueFonts.Contains(fontName) ? "-BoldOblique" : "-BoldItalic",
                _ => ""
            };

            return IronPdf.Font.FontTypes.FromString(fontName);
        }
    }

    /// <summary>
    /// Specifies font style information applied to text.
    /// </summary>
    [Flags]
    public enum FontStyle
    {

        /// <summary>
        /// Normal text.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Bold text.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Underlined text.
        /// </summary>
        Underline = 4,

        /// <summary>
        /// Text with a line through the middle.
        /// </summary>
        Strikeout = 8,

        /// <summary>
        /// Bold and Italic text.
        /// </summary>
        BoldItalic = 16
    }
}
