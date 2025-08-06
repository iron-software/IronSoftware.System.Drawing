using FluentAssertions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class ColorFunctionality : TestsBase
    {
        public ColorFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_new_Color()
        {
            var color = new Color("#19191919");
            Assert.Equal(25, color.A);
            Assert.Equal(25, color.R);
            Assert.Equal(25, color.G);
            Assert.Equal(25, color.B);

            color = new Color("#800080");
            Assert.Equal(255, color.A);
            Assert.Equal(128, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(128, color.B);

            color = new Color("#F0F");
            Assert.Equal(255, color.A);
            Assert.Equal(255, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(255, color.B);

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => color = new Color("#F"));
            Assert.Equal($"#F is unable to convert to {typeof(Color)} because it requires a suitable length of string.", ex.Message);

            color = new Color(255, 255, 0, 255);
            Assert.Equal(255, color.A);
            Assert.Equal(255, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(255, color.B);

            color = new Color(255, 0, 255);
            Assert.Equal(255, color.A);
            Assert.Equal(255, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(255, color.B);
        }

        [FactWithAutomaticDisplayName]
        public void Create_correct_color_from_system_defined()
        {
            Assert.Equal(255, Color.YellowGreen.A);
            Assert.Equal(154, Color.YellowGreen.R);
            Assert.Equal(205, Color.YellowGreen.G);
            Assert.Equal(50, Color.YellowGreen.B);

            Assert.Equal(255, Color.Violet.A);
            Assert.Equal(238, Color.Violet.R);
            Assert.Equal(130, Color.Violet.G);
            Assert.Equal(238, Color.Violet.B);

            Assert.Equal(0, Color.Transparent.A);
            Assert.Equal(255, Color.Transparent.R);
            Assert.Equal(255, Color.Transparent.G);
            Assert.Equal(255, Color.Transparent.B);

            Assert.Equal(255, Color.Azure.A);
            Assert.Equal(240, Color.Azure.R);
            Assert.Equal(255, Color.Azure.G);
            Assert.Equal(255, Color.Azure.B);
        }

        [FactWithAutomaticDisplayName]
        public void Create_color_from_RGB()
        {
            var color = Color.FromArgb(64, 244, 208);

            Assert.Equal(255, color.A);
            Assert.Equal(64, color.R);
            Assert.Equal(244, color.G);
            Assert.Equal(208, color.B);
            Assert.Equal("#FF40F4D0", color.ToString());

            color = Color.FromArgb(0, 64, 244, 208);

            Assert.Equal(0, color.A);
            Assert.Equal(64, color.R);
            Assert.Equal(244, color.G);
            Assert.Equal(208, color.B);
            Assert.Equal("#0040F4D0", color.ToString());

        }

        [FactWithAutomaticDisplayName]
        public void Create_color_from_ARGB()
        {
            var color = Color.FromArgb(100, 64, 244, 208);

            Assert.Equal(100, color.A);
            Assert.Equal(64, color.R);
            Assert.Equal(244, color.G);
            Assert.Equal(208, color.B);
            Assert.Equal("#6440F4D0", color.ToString());

            var color1 = Color.FromArgb(50, color);
            Assert.Equal(50, color1.A);
            Assert.Equal(64, color1.R);
            Assert.Equal(244, color1.G);
            Assert.Equal(208, color1.B);
        }

        [FactWithAutomaticDisplayName]
        public void Get_Luminance_from_color()
        {
            Color color = Color.Black;
            Assert.Equal(0, color.GetLuminance());

            color = Color.Gray;
            Assert.Equal(50, color.GetLuminance());

            color = Color.White;
            Assert.Equal(100, color.GetLuminance());
        }

        [FactWithAutomaticDisplayName]
        public void Cast_System_Drawing_Color_from_Color()
        {
            System.Drawing.Color drawingColor = System.Drawing.Color.Red;
            Color red = drawingColor;
            Assert.Equal(255, red.A);
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            drawingColor = System.Drawing.Color.FromArgb(255, 0, 255, 0);
            Color green = drawingColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            drawingColor = System.Drawing.Color.FromArgb(0, 0, 255);
            Color blue = drawingColor;
            Assert.Equal(255, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);

            int iColorCode = Convert.ToInt32("1e81b0", 16);
            drawingColor = System.Drawing.Color.FromArgb(iColorCode);
            Color color = drawingColor;
            Assert.Equal(0, color.A);
            Assert.Equal(30, color.R);
            Assert.Equal(129, color.G);
            Assert.Equal(176, color.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_System_Drawing_Color_to_Color()
        {
            Color color = Color.Red;
            System.Drawing.Color red = color;
            Assert.Equal(255, red.A);
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            color = new Color(0, 255, 0);
            System.Drawing.Color green = color;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            color = new Color("#0000FF");
            System.Drawing.Color blue = color;
            Assert.Equal(255, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);

            int iColorCode = Convert.ToInt32("1e81b0", 16);
            color = Color.FromArgb(iColorCode);
            System.Drawing.Color drawingColor = color;
            Assert.Equal(0, drawingColor.A);
            Assert.Equal(30, drawingColor.R);
            Assert.Equal(129, drawingColor.G);
            Assert.Equal(176, drawingColor.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_SKColor_from_Color()
        {
            SkiaSharp.SKColor skColor = SkiaSharp.SKColors.Red;
            Color red = skColor;
            Assert.Equal(255, red.A);
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            skColor = new SkiaSharp.SKColor(0, 255, 0, 255);
            Color green = skColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            skColor = new SkiaSharp.SKColor(0, 0, 255);
            Color blue = skColor;
            Assert.Equal(255, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);

            uint iColorCode = Convert.ToUInt32("1e81b0", 16);
            skColor = new SkiaSharp.SKColor(iColorCode);
            Color color = skColor;
            Assert.Equal(0, color.A);
            Assert.Equal(30, color.R);
            Assert.Equal(129, color.G);
            Assert.Equal(176, color.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_SKColor_to_Color()
        {
            Color color = Color.Red;
            SkiaSharp.SKColor red = color;
            Assert.Equal(255, red.Alpha);
            Assert.Equal(255, red.Red);
            Assert.Equal(0, red.Green);
            Assert.Equal(0, red.Blue);

            color = new Color(0, 255, 0);
            SkiaSharp.SKColor green = color;
            Assert.Equal(255, green.Alpha);
            Assert.Equal(0, green.Red);
            Assert.Equal(255, green.Green);
            Assert.Equal(0, green.Blue);

            color = new Color("#0000FF");
            SkiaSharp.SKColor blue = color;
            Assert.Equal(255, blue.Alpha);
            Assert.Equal(0, blue.Red);
            Assert.Equal(0, blue.Green);
            Assert.Equal(255, blue.Blue);

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            SkiaSharp.SKColor skColor = color;
            Assert.Equal(0, skColor.Alpha);
            Assert.Equal(30, skColor.Red);
            Assert.Equal(129, skColor.Green);
            Assert.Equal(176, skColor.Blue);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Color_from_Color()
        {
            SixLabors.ImageSharp.Color imgColor = SixLabors.ImageSharp.Color.Red;
            Color red = imgColor;
            Assert.Equal(255, red.A);
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            imgColor = SixLabors.ImageSharp.Color.FromRgba(0, 255, 0, 255);
            Color green = imgColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            imgColor = SixLabors.ImageSharp.Color.FromRgb(0, 0, 255);
            Color blue = imgColor;
            Assert.Equal(255, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Color_to_Color()
        {
            Color color = Color.Red;
            SixLabors.ImageSharp.Color red = color;
            Assert.Equal("FF0000FF", red.ToHex());

            color = new Color(0, 255, 0);
            SixLabors.ImageSharp.Color green = color;
            Assert.Equal("00FF00FF", green.ToHex());

            color = new Color("#0000FF");
            SixLabors.ImageSharp.Color blue = color;
            Assert.Equal("0000FFFF", blue.ToHex());

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            SixLabors.ImageSharp.Color imgColor = color;
            Assert.Equal("1E81B000", imgColor.ToHex());
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgba32_from_Color()
        {
            SixLabors.ImageSharp.PixelFormats.Rgba32 imgColor = SixLabors.ImageSharp.Color.Red;
            Color red = imgColor;
            Assert.Equal(255, red.A);
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgba32(0, 255, 0, 255);
            Color green = imgColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgba32(0, 0, 255);
            Color blue = imgColor;
            Assert.Equal(255, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgba32_to_Color()
        {
            Color color = Color.Red;
            SixLabors.ImageSharp.PixelFormats.Rgba32 red = color;
            Assert.Equal("FF0000FF", red.ToHex());

            color = new Color(0, 255, 0);
            SixLabors.ImageSharp.PixelFormats.Rgba32 green = color;
            Assert.Equal("00FF00FF", green.ToHex());

            color = new Color("#0000FF");
            SixLabors.ImageSharp.PixelFormats.Rgba32 blue = color;
            Assert.Equal("0000FFFF", blue.ToHex());

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            SixLabors.ImageSharp.PixelFormats.Rgba32 imgColor = color;
            Assert.Equal("1E81B000", imgColor.ToHex());
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgb24_from_Color()
        {
            SixLabors.ImageSharp.PixelFormats.Rgb24 imgColor = SixLabors.ImageSharp.Color.Red;
            Color red = imgColor;
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgb24(0, 255, 0);
            Color green = imgColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgb24(0, 0, 255);
            Color blue = imgColor;
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgb24_to_Color()
        {
            Color color = Color.Red;
            SixLabors.ImageSharp.PixelFormats.Rgb24 red = color;
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            color = new Color(0, 255, 0);
            SixLabors.ImageSharp.PixelFormats.Rgb24 green = color;
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            color = new Color("#0000FF");
            SixLabors.ImageSharp.PixelFormats.Rgb24 blue = color;
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            SixLabors.ImageSharp.PixelFormats.Rgb24 imgColor = color;
            Assert.Equal(30, imgColor.R);
            Assert.Equal(129, imgColor.G);
            Assert.Equal(176, imgColor.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgb48_from_Color()
        {
            var imgColor = new SixLabors.ImageSharp.PixelFormats.Rgb48(255, 0, 0);
            Color red = imgColor;
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgb48(0, 255, 0);
            Color green = imgColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgb48(0, 0, 255);
            Color blue = imgColor;
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgb48_to_Color()
        {
            Color color = Color.Red;
            SixLabors.ImageSharp.PixelFormats.Rgb48 red = color;
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            color = new Color(0, 255, 0);
            SixLabors.ImageSharp.PixelFormats.Rgb48 green = color;
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            color = new Color("#0000FF");
            SixLabors.ImageSharp.PixelFormats.Rgb48 blue = color;
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            SixLabors.ImageSharp.PixelFormats.Rgb48 imgColor = color;
            Assert.Equal(30, imgColor.R);
            Assert.Equal(129, imgColor.G);
            Assert.Equal(176, imgColor.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgba64_from_Color()
        {
            SixLabors.ImageSharp.PixelFormats.Rgba64 imgColor = SixLabors.ImageSharp.Color.Red;
            Color red = imgColor;
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgba64(0, 255, 0, 255);
            Color green = imgColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            imgColor = new SixLabors.ImageSharp.PixelFormats.Rgba64(0, 0, 255, 255);
            Color blue = imgColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_ImageSharp_Rgba64_to_Color()
        {
            Color color = Color.Red;
            SixLabors.ImageSharp.PixelFormats.Rgba64 red = color;
            Assert.Equal(65535, red.A);
            Assert.Equal(65535, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            color = new Color(0, 255, 0);
            SixLabors.ImageSharp.PixelFormats.Rgba64 green = color;
            Assert.Equal(65535, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(65535, green.G);
            Assert.Equal(0, green.B);

            color = new Color("#0000FF");
            SixLabors.ImageSharp.PixelFormats.Rgba64 blue = color;
            Assert.Equal(65535, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(65535, blue.B);

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            SixLabors.ImageSharp.PixelFormats.Rgba64 imgColor = color;
            Assert.Equal(0, imgColor.A);
            Assert.Equal(7710, imgColor.R);
            Assert.Equal(33153, imgColor.G);
            Assert.Equal(45232, imgColor.B);
        }

        [FactWithAutomaticDisplayName]
        public void Should_Return_Argb()
        {
            System.Drawing.Color bmColor = System.Drawing.Color.Azure;
            IronSoftware.Drawing.Color ironColor = Color.Azure;
            IronSoftware.Drawing.Color fromImageSharp = SixLabors.ImageSharp.Color.Azure;
            IronSoftware.Drawing.Color rgba32 = new SixLabors.ImageSharp.PixelFormats.Rgba32(bmColor.R, bmColor.G, bmColor.B, bmColor.A);
            IronSoftware.Drawing.Color rgb24 = new SixLabors.ImageSharp.PixelFormats.Rgb24(bmColor.R, bmColor.G, bmColor.B);

            Assert.Equal(bmColor.ToArgb(), ironColor.ToArgb());
            Assert.Equal(bmColor.ToArgb(), fromImageSharp.ToArgb());
            Assert.Equal(bmColor.ToArgb(), rgba32.ToArgb());
            Assert.Equal(bmColor.ToArgb(), rgb24.ToArgb());
        }

        [FactWithAutomaticDisplayName]
        public void Should_Create_FromName()
        {
            var color = Color.FromName("red");
            _ = color.R.Should().Be(255);
            _ = color.G.Should().Be(0);
            _ = color.B.Should().Be(0);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("green");
            _ = color.R.Should().Be(0);
            _ = color.G.Should().Be(128);
            _ = color.B.Should().Be(0);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("blue");
            _ = color.R.Should().Be(0);
            _ = color.G.Should().Be(0);
            _ = color.B.Should().Be(255);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("yellow");
            _ = color.R.Should().Be(255);
            _ = color.G.Should().Be(255);
            _ = color.B.Should().Be(0);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("pink");
            _ = color.R.Should().Be(255);
            _ = color.G.Should().Be(192);
            _ = color.B.Should().Be(203);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("brown");
            _ = color.R.Should().Be(165);
            _ = color.G.Should().Be(42);
            _ = color.B.Should().Be(42);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("gray");
            _ = color.R.Should().Be(128);
            _ = color.G.Should().Be(128);
            _ = color.B.Should().Be(128);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("black");
            _ = color.R.Should().Be(0);
            _ = color.G.Should().Be(0);
            _ = color.B.Should().Be(0);
            _ = color.IsKnownColor.Should().BeTrue();

            color = Color.FromName("orange");
            _ = color.R.Should().Be(255);
            _ = color.G.Should().Be(165);
            _ = color.B.Should().Be(0);
            _ = color.IsKnownColor.Should().BeTrue();
            
            color = Color.FromName("RebeccaPurple");
            _ = color.A.Should().Be(255);
            _ = color.R.Should().Be(102);
            _ = color.G.Should().Be(51);
            _ = color.B.Should().Be(153);
            _ = color.IsKnownColor.Should().BeTrue();
            
            color = Color.FromName("NotAColor");
            _ = color.A.Should().Be(0);
            _ = color.R.Should().Be(0);
            _ = color.G.Should().Be(0);
            _ = color.B.Should().Be(0);
            _ = color.IsKnownColor.Should().BeFalse();
        }

        [FactWithAutomaticDisplayName]
        public void Should_Equal()
        {
            _ = Color.Red.Equals(Color.FromName("red")).Should().BeTrue();
            _ = (Color.Yellow == Color.FromName("yellow")).Should().BeTrue();
            _ = (Color.Gray != Color.FromName("darkgray")).Should().BeTrue();
            Color nullColor = null;
            _ = (nullColor != null).Should().BeFalse();
            _ = (nullColor == null).Should().BeTrue();
            var red = Color.FromName("red");
            _ = (red != null).Should().BeTrue();
            _ = (red == null).Should().BeFalse();
        }

        [Theory]
        [InlineData("1e81b0")]
        [InlineData("001e81b0")]
        [InlineData("FF1e81b0")]
        public void From32BitArgb_Should_Equal_Drawing(string colorCode)
        {
            int iColorCode = Convert.ToInt32(colorCode, 16);
            var drawingColor = System.Drawing.Color.FromArgb(iColorCode);
            var ironColor = Color.FromArgb(iColorCode);
            Assert.Equal(drawingColor.A, ironColor.A);
            Assert.Equal(drawingColor.R, ironColor.R);
            Assert.Equal(drawingColor.G, ironColor.G);
            Assert.Equal(drawingColor.B, ironColor.B);
        }

        [Theory]
        [InlineData(255, 177, 177, 177, "#B1B1B1")]
        [InlineData(255, 0, 0, 0, "#000000")]
        [InlineData(255, 255, 0, 0, "#FF0000")]
        [InlineData(255, 0, 255, 0, "#00FF00")]
        [InlineData(255, 0, 0, 255, "#0000FF")]
        [InlineData(255, 30, 129, 176, "#1E81B0")]
        public void ToHtml_ShouldReturnCorrectHtmlString(int a, int r, int g, int b, string expectedHtml)
        {
            // Arrange
            var color = new Color(a, r, g, b);

            // Act
            string actualHtml = color.ToHtmlCssColorCode();

            // Assert
            Assert.Equal(expectedHtml, actualHtml);
        }

        [Theory]
        [InlineData(0, 0, 0, 0.0)]     // Black
        [InlineData(255, 255, 255, 1.0)] // White
        [InlineData(255, 0, 0, 1.0)]     // Red
        [InlineData(0, 255, 0, 1.0)]     // Green
        [InlineData(0, 0, 255, 1.0)]     // Blue
        [InlineData(127, 127, 127, 0.4980392156862745)] // Gray
        public void TestBrightness(byte red, byte green, byte blue, double expectedBrightness)
        {
            Color color = Color.FromArgb(red, green, blue);
            double brightness = color.GetBrightness();
            Assert.Equal(expectedBrightness, brightness, 5);
        }

#if !NETFRAMEWORK
        [FactWithAutomaticDisplayName]
        public void Cast_Maui_from_Color()
        {
            Microsoft.Maui.Graphics.Color mColor = Microsoft.Maui.Graphics.Colors.Red;
            Color red = mColor;
            Assert.Equal(255, red.A);
            Assert.Equal(255, red.R);
            Assert.Equal(0, red.G);
            Assert.Equal(0, red.B);

            mColor = new Microsoft.Maui.Graphics.Color(0, 255, 0, 255);
            Color green = mColor;
            Assert.Equal(255, green.A);
            Assert.Equal(0, green.R);
            Assert.Equal(255, green.G);
            Assert.Equal(0, green.B);

            mColor = new Microsoft.Maui.Graphics.Color(0, 0, 255);
            Color blue = mColor;
            Assert.Equal(255, blue.A);
            Assert.Equal(0, blue.R);
            Assert.Equal(0, blue.G);
            Assert.Equal(255, blue.B);

            mColor = Microsoft.Maui.Graphics.Color.FromArgb("ff1e81b0");
            Color color = mColor;
            Assert.Equal(255, color.A);
            Assert.Equal(30, color.R);
            Assert.Equal(129, color.G);
            Assert.Equal(176, color.B);
        }

        [FactWithAutomaticDisplayName]
        public void Cast_Maui_to_Color()
        {
            Color color = Color.Red;
            Microsoft.Maui.Graphics.Color red = color;
            Assert.Equal(1, red.Alpha);
            Assert.Equal(1, red.Red);
            Assert.Equal(0, red.Green);
            Assert.Equal(0, red.Blue);

            color = new Color(0, 255, 0);
            Microsoft.Maui.Graphics.Color green = color;
            Assert.Equal(1, green.Alpha);
            Assert.Equal(0, green.Red);
            Assert.Equal(1, green.Green);
            Assert.Equal(0, green.Blue);

            color = new Color("#0000FF");
            Microsoft.Maui.Graphics.Color blue = color;
            Assert.Equal(1, blue.Alpha);
            Assert.Equal(0, blue.Red);
            Assert.Equal(0, blue.Green);
            Assert.Equal(1, blue.Blue);

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            Microsoft.Maui.Graphics.Color skColor = color;
            Assert.Equal(0, skColor.Alpha);
            Assert.Equal("0.118", skColor.Red.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));
            Assert.Equal("0.506", skColor.Green.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));
            Assert.Equal("0.690", skColor.Blue.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture));
        }
#endif
    }
}
