using FluentAssertions;
using System;
using System.Runtime.InteropServices;
using Xunit;
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

            Font font = new Font("Roboto Serif");
            font.FamilyName.Should().Be("Roboto Serif");
            font.Style.Should().Be(FontStyle.Regular);
            font.Size.Should().Be(12);
            font.Bold.Should().BeFalse();
            font.Italic.Should().BeFalse();

            font = new Font("Roboto", 20);
            font.FamilyName.Should().Be("Roboto");
            font.Style.Should().Be(FontStyle.Regular);
            font.Size.Should().Be(20);
            font.Bold.Should().BeFalse();
            font.Italic.Should().BeFalse();

            font = new Font("Roboto Mono", FontStyle.Bold | FontStyle.Strikeout);
            font.FamilyName.Should().Be("Roboto Mono");
            font.Style.Should().Be(FontStyle.Bold | FontStyle.Strikeout);
            font.Size.Should().Be(12);
            font.Bold.Should().BeTrue();
            font.Strikeout.Should().BeTrue();

            font = new Font("Roboto Flex", FontStyle.Italic | FontStyle.Underline, 30);
            font.FamilyName.Should().Be("Roboto Flex");
            font.Style.Should().Be(FontStyle.Italic | FontStyle.Underline);
            font.Size.Should().Be(30);
            font.Italic.Should().BeTrue();
            font.Underline.Should().BeTrue();

        }

        [FactWithAutomaticDisplayName]
        public void CastSKFont_to_Font()
        {
            SkiaSharp.SKFont skFont = SkiaSharp.SKTypeface.FromFamilyName("Courier New", SkiaSharp.SKFontStyleWeight.Normal, SkiaSharp.SKFontStyleWidth.Normal, SkiaSharp.SKFontStyleSlant.Upright).ToFont(30);
            Font font = skFont;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                font.FamilyName.Should().Be("Liberation Mono");
            }
            else
            {
                font.FamilyName.Should().Be("Courier New");
            }
            font.Size.Should().Be(30);
            font.Style.Should().Be(FontStyle.Regular);
            font.Bold.Should().BeFalse();
            font.Italic.Should().BeFalse();

            skFont = new SkiaSharp.SKFont(SkiaSharp.SKTypeface.FromFamilyName("Times New Roman", SkiaSharp.SKFontStyleWeight.Bold, SkiaSharp.SKFontStyleWidth.Normal, SkiaSharp.SKFontStyleSlant.Italic), 20);
            font = skFont;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                font.FamilyName.Should().Be("Liberation Serif");
            }
            else
            {
                font.FamilyName.Should().Be("Times New Roman");
            }
            font.Size.Should().Be(20);
            font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
            font.Bold.Should().BeTrue();
            font.Italic.Should().BeTrue();
        }

        [FactWithAutomaticDisplayName]
        public void CastSKFont_from_Font()
        {
            Font font = new Font("Courier New", 30);
            SkiaSharp.SKFont skFont = font;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                skFont.Typeface.FamilyName.Should().Be("Liberation Mono");
            }
            else
            {
                skFont.Typeface.FamilyName.Should().Be("Courier New");
            }
            skFont.Size.Should().Be(30);
            skFont.Typeface.FontStyle.Slant.Should().Be(SkiaSharp.SKFontStyleSlant.Upright);
            skFont.Typeface.FontStyle.Weight.Should().Be((int)SkiaSharp.SKFontStyleWeight.Normal);
            skFont.Typeface.FontStyle.Width.Should().Be((int)SkiaSharp.SKFontStyleWidth.Normal);
            skFont.Typeface.IsBold.Should().BeFalse();
            skFont.Typeface.IsItalic.Should().BeFalse();

            font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
            skFont = font;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                skFont.Typeface.FamilyName.Should().Be("Liberation Serif");
            }
            else
            {
                skFont.Typeface.FamilyName.Should().Be("Times New Roman");
            }
            skFont.Size.Should().Be(20);
            skFont.Typeface.FontStyle.Slant.Should().Be(SkiaSharp.SKFontStyleSlant.Italic);
            skFont.Typeface.FontStyle.Weight.Should().Be((int)SkiaSharp.SKFontStyleWeight.Bold);
            skFont.Typeface.FontStyle.Width.Should().Be((int)SkiaSharp.SKFontStyleWidth.Normal);
            skFont.Typeface.IsBold.Should().BeTrue();
            skFont.Typeface.IsItalic.Should().BeTrue();
        }

        [FactWithAutomaticDisplayName]
        public void CastSystemDrawingFont_to_Font()
        {
            System.Drawing.Font drawingFont = new System.Drawing.Font("Courier New", 30);
            Font font = drawingFont;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                font.FamilyName.Should().Be("Liberation Mono");
            }
            else
            {
                font.FamilyName.Should().Be("Courier New");
            }
            font.Size.Should().Be(30);
            font.Style.Should().Be(FontStyle.Regular);
            font.Bold.Should().BeFalse();
            font.Italic.Should().BeFalse();

            drawingFont = new System.Drawing.Font("Times New Roman", 20, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
            font = drawingFont;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                font.FamilyName.Should().Be("Liberation Serif");
            }
            else
            {
                font.FamilyName.Should().Be("Times New Roman");
            }
            font.Size.Should().Be(20);
            font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
            font.Bold.Should().BeTrue();
            font.Italic.Should().BeTrue();
        }

        [FactWithAutomaticDisplayName]
        public void CastSystemDrawingFont_from_Font()
        {
            Font font = new Font("Courier New", 30);
            System.Drawing.Font drawingFont = font;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                drawingFont.FontFamily.Name.Should().Be("Liberation Mono");
            }
            else
            {
                drawingFont.FontFamily.Name.Should().Be("Courier New");
            }
            drawingFont.Size.Should().Be(30);
            drawingFont.Style.Should().Be(System.Drawing.FontStyle.Regular);
            drawingFont.Bold.Should().BeFalse();
            drawingFont.Italic.Should().BeFalse();

            font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
            drawingFont = font;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                drawingFont.FontFamily.Name.Should().Be("Liberation Serif");
            }
            else
            {
                drawingFont.FontFamily.Name.Should().Be("Times New Roman");
            }
            drawingFont.Size.Should().Be(20);
            drawingFont.Style.Should().Be(System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
            drawingFont.Bold.Should().BeTrue();
            drawingFont.Italic.Should().BeTrue();
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLaborsFont_to_Font()
        {
            SixLabors.Fonts.Font sixLaborsFont = SixLabors.Fonts.SystemFonts.CreateFont("Courier New", 30);
            Font font = sixLaborsFont;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                font.FamilyName.Should().Be("Liberation Mono");
            }
            else
            {
                font.FamilyName.Should().Be("Courier New");
            }
            font.Size.Should().Be(30);
            font.Style.Should().Be(FontStyle.Regular);
            font.Bold.Should().BeFalse();
            font.Italic.Should().BeFalse();

            sixLaborsFont = SixLabors.Fonts.SystemFonts.CreateFont("Times New Roman", 20, SixLabors.Fonts.FontStyle.BoldItalic);
            font = sixLaborsFont;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                font.FamilyName.Should().Be("Liberation Serif");
            }
            else
            {
                font.FamilyName.Should().Be("Times New Roman");
            }
            font.Size.Should().Be(20);
            font.Style.Should().Be(FontStyle.Bold | FontStyle.Italic);
            font.Bold.Should().BeTrue();
            font.Italic.Should().BeTrue();
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLaborsFont_from_Font()
        {
            Font font = new Font("Courier New", 30);
            SixLabors.Fonts.Font sixLaborsFont = font;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                sixLaborsFont.Family.Name.Should().Be("Liberation Mono");
            }
            else
            {
                sixLaborsFont.Family.Name.Should().Be("Courier New");
            }
            sixLaborsFont.Size.Should().Be(30);
            sixLaborsFont.IsBold.Should().BeFalse();
            sixLaborsFont.IsItalic.Should().BeFalse();

            font = new Font("Times New Roman", FontStyle.Bold | FontStyle.Italic, 20);
            sixLaborsFont = font;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                sixLaborsFont.Family.Name.Should().Be("Liberation Serif");
            }
            else
            {
                sixLaborsFont.Family.Name.Should().Be("Times New Roman");
            }
            sixLaborsFont.Size.Should().Be(20);
            sixLaborsFont.IsBold.Should().BeTrue();
            sixLaborsFont.IsItalic.Should().BeTrue();
        }
    }
}
