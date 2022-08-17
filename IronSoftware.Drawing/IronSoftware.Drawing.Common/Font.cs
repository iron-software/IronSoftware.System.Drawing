using System;
using System.Collections.ObjectModel;
using System.Drawing;

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
        /// Gets the em-size of this Font measured in the units specified by the Unit property.
        /// </summary>
        public float Size { get; internal set; } = 12;

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName.
        /// </summary>
        /// <param name="FamilyName">The FontFamily of the new Font.</param>
        public Font(string FamilyName)
        {
            this.FamilyName = FamilyName;
        }

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName and FontStyle enumeration.
        /// </summary>
        /// <param name="FamilyName">The FontFamily of the new Font.</param>
        /// <param name="Style">The FontStyle to apply to the new Font. Multiple values of the FontStyle enumeration can be combined with the OR operator.</param>
        public Font(string FamilyName, FontStyle Style)
        {
            this.FamilyName = FamilyName;
            this.Style = Style;
        }

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName, FontStyle enumeration, FontWeight, Bold, Italic and Size.
        /// </summary>
        /// <param name="FamilyName">The FontFamily of the new Font.</param>
        /// <param name="Style">The FontStyle to apply to the new Font. Multiple values of the FontStyle enumeration can be combined with the OR operator.</param>
        /// <param name="Size">The em-size of the new font in the units specified by the unit parameter.</param>
        public Font(string FamilyName, FontStyle Style, float Size)
        {
            this.FamilyName = FamilyName;
            this.Style = Style;
            this.Size = Size;
        }

        /// <summary>
        /// Initializes a new Font that uses the specified existing FamilyName, FontWeight, Bold, Italic and Size.
        /// </summary>
        /// <param name="FamilyName">The FontFamily of the new Font.</param>
        /// <param name="Size">The em-size of the new font in the units specified by the unit parameter.</param>
        public Font(string FamilyName, float Size)
        {
            this.FamilyName = FamilyName;
            this.Size = Size;
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Font objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods to use <see cref="Font"/> as parameters and return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="Font">System.Drawing.Font will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(System.Drawing.Font Font)
        {
            return new Font(Font.FontFamily.Name, (FontStyle)Font.Style, Font.Size);
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods to use <see cref="Font"/> as parameters and return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="Font"><see cref="Font"/> is explicitly cast to an System.Drawing.Font </param>
        static public implicit operator System.Drawing.Font(Font Font)
        {
            return new System.Drawing.Font(new FontFamily(Font.FamilyName), Font.Size, (System.Drawing.FontStyle)Font.Style);
        }

        /// <summary>
        /// Implicitly casts SixLabors.Fonts.Font objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods to use <see cref="Font"/> as parameters and return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="Font">SixLabors.Fonts.Font will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(SixLabors.Fonts.Font Font)
        {
            FontStyle fontStyle;
            if (Font.IsBold && Font.IsItalic)
            {
                fontStyle = FontStyle.Bold | FontStyle.Italic;
            }
            else if (Font.IsBold)
            {
                fontStyle = FontStyle.Bold;
            }
            else if (Font.IsItalic)
            {
                fontStyle = FontStyle.Italic;
            }
            else
            {
                fontStyle = FontStyle.Regular;
            }
            return new Font(Font.Family.Name, fontStyle, Font.Size);
        }

        /// <summary>
        /// Implicitly casts SixLabors.Fonts.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods to use <see cref="Font"/> as parameters and return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="Font"><see cref="Font"/> is explicitly cast to an SixLabors.Fonts.Font </param>
        static public implicit operator SixLabors.Fonts.Font(Font Font)
        {
            return SixLabors.Fonts.SystemFonts.CreateFont(Font.FamilyName, Font.Size, (SixLabors.Fonts.FontStyle)Font.Style);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKFont objects to <see cref="Font"/>.  
        /// <para>When your .NET Class methods to use <see cref="Font"/> as parameters and return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="Font">SkiaSharp.SKFont will automatically be cast to <see cref="Font"/> </param>
        public static implicit operator Font(SkiaSharp.SKFont Font)
        {
            FontStyle fontStyle;
            if (Font.Typeface.IsBold && Font.Typeface.IsItalic)
            {
                fontStyle = FontStyle.Bold | FontStyle.Italic;
            }
            else if (Font.Typeface.IsBold)
            {
                fontStyle = FontStyle.Bold;
            }
            else if (Font.Typeface.IsItalic)
            {
                fontStyle = FontStyle.Italic;
            }
            else
            {
                fontStyle = FontStyle.Regular;
            }
            Font result = new Font(Font.Typeface.FamilyName, fontStyle, Font.Size);

            return result;
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Font objects from <see cref="Font"/>.  
        /// <para>When your .NET Class methods to use <see cref="Font"/> as parameters and return types, you now automatically support Font as well.</para>
        /// </summary>
        /// <param name="Font"><see cref="Font"/> is explicitly cast to an SkiaSharp.SKFont </param>
        static public implicit operator SkiaSharp.SKFont(Font Font)
        {
            SkiaSharp.SKFontStyle sKFontStyle;
            switch (Font.Style)
            {
                case FontStyle.Bold: sKFontStyle = SkiaSharp.SKFontStyle.Bold; break;
                case FontStyle.Italic: sKFontStyle = SkiaSharp.SKFontStyle.Italic; break;
                case FontStyle.Bold | FontStyle.Italic: sKFontStyle = SkiaSharp.SKFontStyle.BoldItalic; break;
                
                default: sKFontStyle = SkiaSharp.SKFontStyle.Normal; break;
            }
            return new SkiaSharp.SKFont(SkiaSharp.SKTypeface.FromFamilyName(Font.FamilyName, sKFontStyle), Font.Size);
        }
    }

    /// <summary>
    /// Specifies style information applied to text.
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
        Strikeout = 8
    }
}
