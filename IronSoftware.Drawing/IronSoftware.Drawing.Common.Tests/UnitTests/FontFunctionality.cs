using FluentAssertions;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class FontFunctionality : TestsBase
    {
        public FontFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_new_Font()
        {

            var font = new Font("Roboto Serif");
            _ = font.FamilyName.Should().Be("Roboto Serif");
            _ = font.Style.Should().Be(FontStyle.Regular);
            _ = font.Size.Should().Be(12);
            _ = font.Bold.Should().BeFalse();
            _ = font.Italic.Should().BeFalse();

            font = new Font("Roboto", 20);
            _ = font.FamilyName.Should().Be("Roboto");
            _ = font.Style.Should().Be(FontStyle.Regular);
            _ = font.Size.Should().Be(20);
            _ = font.Bold.Should().BeFalse();
            _ = font.Italic.Should().BeFalse();

            font = new Font("Roboto Mono", FontStyle.Bold | FontStyle.Strikeout);
            _ = font.FamilyName.Should().Be("Roboto Mono");
            _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Strikeout);
            _ = font.Size.Should().Be(12);
            _ = font.Bold.Should().BeTrue();
            _ = font.Strikeout.Should().BeTrue();

            font = new Font("Roboto Flex", FontStyle.Italic | FontStyle.Underline, 30);
            _ = font.FamilyName.Should().Be("Roboto Flex");
            _ = font.Style.Should().Be(FontStyle.Italic | FontStyle.Underline);
            _ = font.Size.Should().Be(30);
            _ = font.Italic.Should().BeTrue();
            _ = font.Underline.Should().BeTrue();

        }

        [FactWithAutomaticDisplayName]
        public void CastSKFont_to_Font()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SkiaSharp.SKFont skFont = SkiaSharp.SKTypeface.FromFamilyName("Liberation Mono", SkiaSharp.SKFontStyleWeight.Normal, SkiaSharp.SKFontStyleWidth.Normal, SkiaSharp.SKFontStyleSlant.Upright).ToFont(30);
                Font font = skFont;
                _ = font.FamilyName.Should().Be("Liberation Mono");
                _ = font.Size.Should().Be(30);
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                skFont = new SkiaSharp.SKFont(SkiaSharp.SKTypeface.FromFamilyName("Liberation Serif", SkiaSharp.SKFontStyleWeight.Bold, SkiaSharp.SKFontStyleWidth.Normal, SkiaSharp.SKFontStyleSlant.Italic), 20);
                font = skFont;
                _ = font.FamilyName.Should().Be("Liberation Serif");
                _ = font.Size.Should().Be(20);
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
            else
            {
                SkiaSharp.SKFont skFont = SkiaSharp.SKTypeface.FromFamilyName("Courier New", SkiaSharp.SKFontStyleWeight.Normal, SkiaSharp.SKFontStyleWidth.Normal, SkiaSharp.SKFontStyleSlant.Upright).ToFont(30);
                Font font = skFont;
                _ = font.FamilyName.Should().Be("Courier New");
                _ = font.Size.Should().Be(30);
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                skFont = new SkiaSharp.SKFont(SkiaSharp.SKTypeface.FromFamilyName("Times New Roman", SkiaSharp.SKFontStyleWeight.Bold, SkiaSharp.SKFontStyleWidth.Normal, SkiaSharp.SKFontStyleSlant.Italic), 20);
                font = skFont;
                _ = font.FamilyName.Should().Be("Times New Roman");
                _ = font.Size.Should().Be(20);
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
        }

        [FactWithAutomaticDisplayName]
        public void CastSKFont_from_Font()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var font = new Font("Courier New", 30);
                SkiaSharp.SKFont skFont = font;
                _ = skFont.Typeface.FamilyName.Should().Be("Liberation Mono");
                _ = skFont.Size.Should().Be(30);
                _ = skFont.Typeface.FontStyle.Slant.Should().Be(SkiaSharp.SKFontStyleSlant.Upright);
                _ = skFont.Typeface.FontStyle.Weight.Should().Be((int)SkiaSharp.SKFontStyleWeight.Normal);
                _ = skFont.Typeface.FontStyle.Width.Should().Be((int)SkiaSharp.SKFontStyleWidth.Normal);
                _ = skFont.Typeface.IsBold.Should().BeFalse();
                _ = skFont.Typeface.IsItalic.Should().BeFalse();

                font = new Font("Liberation Serif", FontStyle.Bold | FontStyle.Italic, 20);
                skFont = font;
                _ = skFont.Typeface.FamilyName.Should().Be("Liberation Serif");
                _ = skFont.Size.Should().Be(20);
                _ = skFont.Typeface.FontStyle.Slant.Should().Be(SkiaSharp.SKFontStyleSlant.Italic);
                _ = skFont.Typeface.FontStyle.Weight.Should().Be((int)SkiaSharp.SKFontStyleWeight.Bold);
                _ = skFont.Typeface.FontStyle.Width.Should().Be((int)SkiaSharp.SKFontStyleWidth.Normal);
                _ = skFont.Typeface.IsBold.Should().BeTrue();
                _ = skFont.Typeface.IsItalic.Should().BeTrue();
            }
            else
            {
                var font = new Font("Courier New", 30);
                SkiaSharp.SKFont skFont = font;
                _ = skFont.Typeface.FamilyName.Should().Be("Courier New");
                _ = skFont.Size.Should().Be(30);
                _ = skFont.Typeface.FontStyle.Slant.Should().Be(SkiaSharp.SKFontStyleSlant.Upright);
                _ = skFont.Typeface.FontStyle.Weight.Should().Be((int)SkiaSharp.SKFontStyleWeight.Normal);
                _ = skFont.Typeface.FontStyle.Width.Should().Be((int)SkiaSharp.SKFontStyleWidth.Normal);
                _ = skFont.Typeface.IsBold.Should().BeFalse();
                _ = skFont.Typeface.IsItalic.Should().BeFalse();

                font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
                skFont = font;
                _ = skFont.Typeface.FamilyName.Should().Be("Times New Roman");
                _ = skFont.Size.Should().Be(20);
                _ = skFont.Typeface.FontStyle.Slant.Should().Be(SkiaSharp.SKFontStyleSlant.Italic);
                _ = skFont.Typeface.FontStyle.Weight.Should().Be((int)SkiaSharp.SKFontStyleWeight.Bold);
                _ = skFont.Typeface.FontStyle.Width.Should().Be((int)SkiaSharp.SKFontStyleWidth.Normal);
                _ = skFont.Typeface.IsBold.Should().BeTrue();
                _ = skFont.Typeface.IsItalic.Should().BeTrue();
            }
        }

        [IgnoreOnMacFact]
        public void CastSystemDrawingFont_to_Font()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var drawingFont = new System.Drawing.Font("Liberation Mono", 30);
                Font font = drawingFont;
                _ = font.FamilyName.Should().Be("Liberation Mono");
                _ = font.Size.Should().Be(30);
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                drawingFont = new System.Drawing.Font("Liberation Serif", 20, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                font = drawingFont;
                _ = font.FamilyName.Should().Be("Liberation Serif");
                _ = font.Size.Should().Be(20);
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
            else
            {
                var drawingFont = new System.Drawing.Font("Courier New", 30);
                Font font = drawingFont;
                _ = font.FamilyName.Should().Be("Courier New");
                _ = font.Size.Should().Be(30);
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                drawingFont = new System.Drawing.Font("Times New Roman", 20, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                font = drawingFont;
                _ = font.FamilyName.Should().Be("Times New Roman");
                _ = font.Size.Should().Be(20);
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
        }

        [IgnoreOnMacFact]
        public void CastSystemDrawingFont_from_Font()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var font = new Font("Liberation Mono", 30);
                System.Drawing.Font drawingFont = font;
                _ = drawingFont.FontFamily.Name.Should().Be("Liberation Mono");
                _ = drawingFont.Size.Should().Be(30);
                _ = drawingFont.Style.Should().Be(System.Drawing.FontStyle.Regular);
                _ = drawingFont.Bold.Should().BeFalse();
                _ = drawingFont.Italic.Should().BeFalse();

                font = new Font("Liberation Serif", FontStyle.Bold | FontStyle.Italic, 20);
                drawingFont = font;
                _ = drawingFont.FontFamily.Name.Should().Be("Liberation Serif");
                _ = drawingFont.Size.Should().Be(20);
                _ = drawingFont.Style.Should().Be(System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                _ = drawingFont.Bold.Should().BeTrue();
                _ = drawingFont.Italic.Should().BeTrue();
            }
            else
            {
                var font = new Font("Courier New", 30);
                System.Drawing.Font drawingFont = font;
                _ = drawingFont.FontFamily.Name.Should().Be("Courier New");
                _ = drawingFont.Size.Should().Be(30);
                _ = drawingFont.Style.Should().Be(System.Drawing.FontStyle.Regular);
                _ = drawingFont.Bold.Should().BeFalse();
                _ = drawingFont.Italic.Should().BeFalse();

                font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
                drawingFont = font;
                _ = drawingFont.FontFamily.Name.Should().Be("Times New Roman");
                _ = drawingFont.Size.Should().Be(20);
                _ = drawingFont.Style.Should().Be(System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                _ = drawingFont.Bold.Should().BeTrue();
                _ = drawingFont.Italic.Should().BeTrue();
            }
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLaborsFont_to_Font()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SixLabors.Fonts.Font sixLaborsFont = SixLabors.Fonts.SystemFonts.CreateFont("Liberation Mono", 30);
                Font font = sixLaborsFont;
                _ = font.FamilyName.Should().Be("Liberation Mono");
                _ = font.Size.Should().Be(30);
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                sixLaborsFont = SixLabors.Fonts.SystemFonts.CreateFont("Liberation Serif", 20, SixLabors.Fonts.FontStyle.BoldItalic);
                font = sixLaborsFont;
                _ = font.FamilyName.Should().Be("Liberation Serif");
                _ = font.Size.Should().Be(20);
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
            else
            {
                SixLabors.Fonts.Font sixLaborsFont = SixLabors.Fonts.SystemFonts.CreateFont("Courier New", 30);
                Font font = sixLaborsFont;
                _ = font.FamilyName.Should().Be("Courier New");
                _ = font.Size.Should().Be(30);
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                sixLaborsFont = SixLabors.Fonts.SystemFonts.CreateFont("Times New Roman", 20, SixLabors.Fonts.FontStyle.BoldItalic);
                font = sixLaborsFont;
                _ = font.FamilyName.Should().Be("Times New Roman");
                _ = font.Size.Should().Be(20);
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLaborsFont_from_Font()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var font = new Font("Liberation Mono", 30);
                SixLabors.Fonts.Font sixLaborsFont = font;
                _ = sixLaborsFont.Family.Name.Should().Be("Liberation Mono");
                _ = sixLaborsFont.Size.Should().Be(30);
                _ = sixLaborsFont.IsBold.Should().BeFalse();
                _ = sixLaborsFont.IsItalic.Should().BeFalse();

                font = new Font("Liberation Serif", FontStyle.Bold | FontStyle.Italic, 20);
                sixLaborsFont = font;
                _ = sixLaborsFont.Family.Name.Should().Be("Liberation Serif");
                _ = sixLaborsFont.Size.Should().Be(20);
                _ = sixLaborsFont.IsBold.Should().BeTrue();
                _ = sixLaborsFont.IsItalic.Should().BeTrue();
            }
            else
            {
                var font = new Font("Courier New", 30);
                SixLabors.Fonts.Font sixLaborsFont = font;
                _ = sixLaborsFont.Family.Name.Should().Be("Courier New");
                _ = sixLaborsFont.Size.Should().Be(30);
                _ = sixLaborsFont.IsBold.Should().BeFalse();
                _ = sixLaborsFont.IsItalic.Should().BeFalse();

                font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
                sixLaborsFont = font;
                _ = sixLaborsFont.Family.Name.Should().Be("Times New Roman");
                _ = sixLaborsFont.Size.Should().Be(20);
                _ = sixLaborsFont.IsBold.Should().BeTrue();
                _ = sixLaborsFont.IsItalic.Should().BeTrue();
            }
        }

#if !NETFRAMEWORK

        [FactWithAutomaticDisplayName]
        public void CastMauiFont_to_Font()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var mFont = new Microsoft.Maui.Graphics.Font("Liberation Mono");
                Font font = mFont;
                _ = font.FamilyName.Should().Be("Liberation Mono");
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                mFont = new Microsoft.Maui.Graphics.Font("Liberation Serif", 800, Microsoft.Maui.Graphics.FontStyleType.Italic);
                font = mFont;
                _ = font.FamilyName.Should().Be("Liberation Serif");
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
            else
            {
                var mFont = new Microsoft.Maui.Graphics.Font("Courier New");
                Font font = mFont;
                _ = font.FamilyName.Should().Be("Courier New");
                _ = font.Style.Should().Be(FontStyle.Regular);
                _ = font.Bold.Should().BeFalse();
                _ = font.Italic.Should().BeFalse();

                mFont = new Microsoft.Maui.Graphics.Font("Times New Roman", 800, Microsoft.Maui.Graphics.FontStyleType.Italic);
                font = mFont;
                _ = font.FamilyName.Should().Be("Times New Roman");
                _ = font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
                _ = font.Bold.Should().BeTrue();
                _ = font.Italic.Should().BeTrue();
            }
        }

        [FactWithAutomaticDisplayName]
        public void CastMauiFont_from_Font()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var font = new Font("Liberation Mono", 30);
                Microsoft.Maui.Graphics.Font mFont = font;
                _ = mFont.Name.Should().Be("Liberation Mono");
                _ = mFont.Weight.Should().Be(400);
                _ = mFont.StyleType.Should().Be(Microsoft.Maui.Graphics.FontStyleType.Normal);

                font = new Font("Liberation Serif", FontStyle.Bold | FontStyle.Italic, 20);
                mFont = font;
                _ = mFont.Name.Should().Be("Liberation Serif");
                _ = mFont.Weight.Should().Be(700);
                _ = mFont.StyleType.Should().Be(Microsoft.Maui.Graphics.FontStyleType.Italic);
            }
            else
            {
                var font = new Font("Courier New", 30);
                Microsoft.Maui.Graphics.Font mFont = font;
                _ = mFont.Name.Should().Be("Courier New");
                _ = mFont.Weight.Should().Be(400);
                _ = mFont.StyleType.Should().Be(Microsoft.Maui.Graphics.FontStyleType.Normal);

                font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
                mFont = font;
                _ = mFont.Name.Should().Be("Times New Roman");
                _ = mFont.Weight.Should().Be(700);
                _ = mFont.StyleType.Should().Be(Microsoft.Maui.Graphics.FontStyleType.Italic);
            }
        }
#endif
    }
}
