using IronSoftware.Drawing.Extensions;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// Supported PDF Fonts
    /// </summary>
    public class FontTypes : Enumeration
    {
        /// E
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string FontFilePath { get; private set; } = null;

        /// E
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static FontTypes GenerateInstance(int id, string name, string fontFilePath = null)
        {
            return new(id, name, fontFilePath);
        }

        internal FontTypes(int id, string name, string fontFilePath = null) : base(id, name)
        {
            FontFilePath = fontFilePath;
        }

        /// <summary>
        /// Represents the Arial font type.
        /// </summary>
        public static FontTypes Arial => new(1, "Arial");
        /// <summary>
        /// Represents the Arial-Bold font type.
        /// </summary>
        public static FontTypes ArialBold => new(2, "Arial-Bold");
        /// <summary>
        /// Represents the Arial-BoldItalic font type.
        /// </summary>
        public static FontTypes ArialBoldItalic => new(3, "Arial-BoldItalic");
        /// <summary>
        /// Represents the Arial-Italic font type.
        /// </summary>
        public static FontTypes ArialItalic => new(4, "Arial-Italic");
        /// <summary>
        /// Represents the Courier font type.
        /// </summary>
        public static FontTypes Courier => new(5, "Courier");
        /// <summary>
        /// Represents the Courier-BoldOblique font type.
        /// </summary>
        public static FontTypes CourierBoldOblique => new(6, "Courier-BoldOblique");
        /// <summary>
        /// Represents the Courier-Oblique font type.
        /// </summary>
        public static FontTypes CourierOblique => new(7, "Courier-Oblique");
        /// <summary>
        /// Represents the Courier-Bold font type.
        /// </summary>
        public static FontTypes CourierBold => new(8, "Courier-Bold");
        /// <summary>
        /// Represents the CourierNew font type.
        /// </summary>
        public static FontTypes CourierNew => new(9, "CourierNew");
        /// <summary>
        /// Represents the CourierNew-Bold font type.
        /// </summary>
        public static FontTypes CourierNewBold => new(10, "CourierNew-Bold");
        /// <summary>
        /// Represents the CourierNew-BoldItalic font type.
        /// </summary>
        public static FontTypes CourierNewBoldItalic => new(11, "CourierNew-BoldItalic");
        /// <summary>
        /// Represents the CourierNew-Italic font type.
        /// </summary>
        public static FontTypes CourierNewItalic => new(12, "CourierNew-Italic");
        /// <summary>
        /// Represents the Helvetica font type.
        /// </summary>
        public static FontTypes Helvetica => new(13, "Helvetica");
        /// <summary>
        /// Represents the Helvetica-Bold font type.
        /// </summary>
        public static FontTypes HelveticaBold => new(14, "Helvetica-Bold");
        /// <summary>
        /// Represents the Helvetica-BoldOblique font type.
        /// </summary>
        public static FontTypes HelveticaBoldOblique => new(15, "Helvetica-BoldOblique");
        /// <summary>
        /// Represents the Helvetica-Oblique font type.
        /// </summary>
        public static FontTypes HelveticaOblique => new(16, "Helvetica-Oblique");
        /// <summary>
        /// Represents the Symbol font type.
        /// </summary>
        public static FontTypes Symbol => new(17, "Symbol");
        /// <summary>
        /// Represents the TimesNewRoman font type.
        /// </summary>
        public static FontTypes TimesNewRoman => new(18, "TimesNewRoman");
        /// <summary>
        /// Represents the TimesNewRoman-Bold font type.
        /// </summary>
        public static FontTypes TimesNewRomanBold => new(19, "TimesNewRoman-Bold");
        /// <summary>
        /// Represents the TimesNewRoman-BoldItalic font type.
        /// </summary>
        public static FontTypes TimesNewRomanBoldItalic => new(20, "TimesNewRoman-BoldItalic");
        /// <summary>
        /// Represents the TimesNewRoman-Italic font type.
        /// </summary>
        public static FontTypes TimesNewRomanItalic => new(21, "TimesNewRoman-Italic");
        /// <summary>
        /// Represents the ZapfDingbats font type.
        /// </summary>
        public static FontTypes ZapfDingbats => new(22, "ZapfDingbats");

        //public static FontTypes Custom(string name, string fontFilePath) => new(23, name, fontFilePath);

        /// <summary>
        /// Returns the corresponding <see cref="FontTypes"/> based on the provided font name string.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <returns>The corresponding <see cref="FontTypes"/> object.</returns>
        /// <exception cref="System.InvalidCastException">Thrown when the provided font name is not a recognized standard PDF font type.</exception>
        public static FontTypes FromString(string fontName)
        {
            return fontName switch
            {
                "Arial" => Arial,
                "Arial-Bold" => ArialBold,
                "Arial-BoldItalic" => ArialBoldItalic,
                "Arial-Italic" => ArialItalic,
                "Courier" => Courier,
                "Courier-BoldOblique" => CourierBoldOblique,
                "Courier-Oblique" => CourierOblique,
                "Courier-Bold" => CourierBold,
                "CourierNew" => CourierNew,
                "CourierNew-Bold" => CourierNewBold,
                "CourierNew-BoldItalic" => CourierNewBoldItalic,
                "CourierNew-Italic" => CourierNewItalic,
                "Helvetica" => Helvetica,
                "Helvetica-Bold" => HelveticaBold,
                "Helvetica-BoldOblique" => HelveticaBoldOblique,
                "Helvetica-Oblique" => HelveticaOblique,
                "Symbol" => Symbol,
                "TimesNewRoman" => TimesNewRoman,
                "TimesNewRoman-Bold" => TimesNewRomanBold,
                "TimesNewRoman-BoldItalic" => TimesNewRomanBoldItalic,
                "TimesNewRoman-Italic" => TimesNewRomanItalic,
                "ZapfDingbats" => ZapfDingbats,
                _ => throw new System.InvalidCastException($"You have set a non-PDF standard FontType: {fontName}, Please select one from IronSoftware.Drawing.FontTypes.")
            };
        }
    }
}