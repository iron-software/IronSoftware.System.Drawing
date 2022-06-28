using SkiaSharp;
using System;
using System.Drawing.Imaging;
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
            Color color = new Color("#19191919");
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

            var ex = Assert.Throws<InvalidOperationException>(() => color = new Color("#F"));
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
            Color color = Color.FromArgb(64, 244, 208);

            Assert.Equal(255, color.A);
            Assert.Equal(64, color.R);
            Assert.Equal(244, color.G);
            Assert.Equal(208, color.B);
            Assert.Equal("#FF40F4D0", color.ToString());

        }

        [FactWithAutomaticDisplayName]
        public void Create_color_from_ARGB()
        {
            Color color = Color.FromArgb(100, 64, 244, 208);

            Assert.Equal(100, color.A);
            Assert.Equal(64, color.R);
            Assert.Equal(244, color.G);
            Assert.Equal(208, color.B);
            Assert.Equal("#6440F4D0", color.ToString());
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

            Int32 iColorCode = Convert.ToInt32("1e81b0", 16);
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

            color = Color.FromArgb(Convert.ToInt32("1e81b0", 16));
            System.Drawing.Color drawingColor = color;
            Assert.Equal(255, drawingColor.A);
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

            UInt32 iColorCode = Convert.ToUInt32("1e81b0", 16);
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
            Assert.Equal(255, skColor.Alpha);
            Assert.Equal(30, skColor.Red);
            Assert.Equal(129, skColor.Green);
            Assert.Equal(176, skColor.Blue);
        }
    }
}
