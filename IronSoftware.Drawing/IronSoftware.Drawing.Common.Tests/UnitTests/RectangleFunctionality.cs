using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class RectangleFunctionality : TestsBase
    {
        public RectangleFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_new_Rectangle()
        {
            Rectangle Rectangle = new Rectangle();
            Assert.NotNull(Rectangle);
            Assert.Equal(0, Rectangle.Width);
            Assert.Equal(0, Rectangle.Height);
            Assert.Equal(0, Rectangle.X);
            Assert.Equal(0, Rectangle.Y);

            Rectangle.Width = 100;
            Rectangle.Height = 100;
            Rectangle.X = 25;
            Rectangle.Y = 50;
            Assert.Equal(100, Rectangle.Width);
            Assert.Equal(100, Rectangle.Height);
            Assert.Equal(25, Rectangle.X);
            Assert.Equal(50, Rectangle.Y);

            Rectangle = new Rectangle(5, 5, 50, 50);
            Assert.NotNull(Rectangle);
            Assert.Equal(50, Rectangle.Width);
            Assert.Equal(50, Rectangle.Height);
            Assert.Equal(5, Rectangle.X);
            Assert.Equal(5, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastRectangle_to_Rectangle()
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(10, 10, 150, 150);
            Rectangle Rectangle = rectangle;
            Assert.NotNull(Rectangle);
            Assert.Equal(150, Rectangle.Width);
            Assert.Equal(150, Rectangle.Height);
            Assert.Equal(10, Rectangle.X);
            Assert.Equal(10, Rectangle.Y);

            rectangle = new System.Drawing.Rectangle(new System.Drawing.Point(15, 15), new System.Drawing.Size(75, 75));
            Rectangle = rectangle;
            Assert.NotNull(Rectangle);
            Assert.Equal(75, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(15, Rectangle.X);
            Assert.Equal(15, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastRectangle_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(5, 5, 50, 50);

            System.Drawing.Rectangle rectangle = Rectangle;

            Assert.NotNull(Rectangle);
            Assert.Equal(50, rectangle.Width);
            Assert.Equal(50, rectangle.Height);
            Assert.Equal(5, rectangle.X);
            Assert.Equal(5, rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_Rectangle_to_Rectangle()
        {
            SixLabors.ImageSharp.Rectangle rectangle = new SixLabors.ImageSharp.Rectangle(10, 10, 150, 150);
            Rectangle Rectangle = rectangle;
            Assert.NotNull(Rectangle);
            Assert.Equal(150, Rectangle.Width);
            Assert.Equal(150, Rectangle.Height);
            Assert.Equal(10, Rectangle.X);
            Assert.Equal(10, Rectangle.Y);

            rectangle = new SixLabors.ImageSharp.Rectangle(new SixLabors.ImageSharp.Point(15, 15), new SixLabors.ImageSharp.Size(75, 75));
            Rectangle = rectangle;
            Assert.NotNull(Rectangle);
            Assert.Equal(75, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(15, Rectangle.X);
            Assert.Equal(15, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_Rectangle_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(5, 5, 50, 50);

            SixLabors.ImageSharp.Rectangle rectangle = Rectangle;

            Assert.NotNull(Rectangle);
            Assert.Equal(50, rectangle.Width);
            Assert.Equal(50, rectangle.Height);
            Assert.Equal(5, rectangle.X);
            Assert.Equal(5, rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_RectangleF_to_Rectangle()
        {
            SixLabors.ImageSharp.RectangleF rectangle = new SixLabors.ImageSharp.RectangleF(10, 10, 150, 150);
            Rectangle Rectangle = rectangle;
            Assert.NotNull(Rectangle);
            Assert.Equal(150, Rectangle.Width);
            Assert.Equal(150, Rectangle.Height);
            Assert.Equal(10, Rectangle.X);
            Assert.Equal(10, Rectangle.Y);

            rectangle = new SixLabors.ImageSharp.Rectangle(new SixLabors.ImageSharp.Point(15, 15), new SixLabors.ImageSharp.Size(75, 75));
            Rectangle = rectangle;
            Assert.NotNull(Rectangle);
            Assert.Equal(75, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(15, Rectangle.X);
            Assert.Equal(15, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_RectangleF_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(5, 5, 50, 50);

            SixLabors.ImageSharp.RectangleF rectangle = Rectangle;

            Assert.NotNull(Rectangle);
            Assert.Equal(50, rectangle.Width);
            Assert.Equal(50, rectangle.Height);
            Assert.Equal(5, rectangle.X);
            Assert.Equal(5, rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRect_to_Rectangle()
        {
            SkiaSharp.SKRect rect = new SkiaSharp.SKRect(50, 100, 150, 25);
            Rectangle Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(100, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(50, Rectangle.X);
            Assert.Equal(25, Rectangle.Y);
            
            rect = new SkiaSharp.SKRect(150, 25, 50, 100);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(100, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(50, Rectangle.X);
            Assert.Equal(25, Rectangle.Y);

            rect = SkiaSharp.SKRect.Create(200, 200);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(200, Rectangle.Width);
            Assert.Equal(200, Rectangle.Height);
            Assert.Equal(0, Rectangle.X);
            Assert.Equal(0, Rectangle.Y);

            rect = SkiaSharp.SKRect.Create(15, 15, 200, 200);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(200, Rectangle.Width);
            Assert.Equal(200, Rectangle.Height);
            Assert.Equal(15, Rectangle.X);
            Assert.Equal(15, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRect_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(50, 25, 50, 50);
            SkiaSharp.SKRect rect = Rectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            Rectangle = new Rectangle(15, 15, 200, 200);
            rect = Rectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRectI_to_Rectangle()
        {
            SkiaSharp.SKRectI rect = new SkiaSharp.SKRectI(50, 100, 150, 25);
            Rectangle Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(100, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(50, Rectangle.X);
            Assert.Equal(25, Rectangle.Y);

            rect = new SkiaSharp.SKRectI(150, 25, 50, 100);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(100, Rectangle.Width);
            Assert.Equal(75, Rectangle.Height);
            Assert.Equal(50, Rectangle.X);
            Assert.Equal(25, Rectangle.Y);

            rect = SkiaSharp.SKRectI.Create(200, 200);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(200, Rectangle.Width);
            Assert.Equal(200, Rectangle.Height);
            Assert.Equal(0, Rectangle.X);
            Assert.Equal(0, Rectangle.Y);

            rect = SkiaSharp.SKRectI.Create(15, 15, 200, 200);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(200, Rectangle.Width);
            Assert.Equal(200, Rectangle.Height);
            Assert.Equal(15, Rectangle.X);
            Assert.Equal(15, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRectI_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(50, 25, 50, 50);
            SkiaSharp.SKRectI rect = Rectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            Rectangle = new Rectangle(15, 15, 200, 200);
            rect = Rectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }

        [FactWithAutomaticDisplayName]
        public void ConvertMeasurement()
        {
            Rectangle pxCropRect = new Rectangle(15, 25, 150, 175);
            Rectangle mmCropRect = pxCropRect.ConvertTo(MeasurementUnits.Millimeters, 96);
            Assert.Equal(3, mmCropRect.X);
            Assert.Equal(6, mmCropRect.Y);
            Assert.Equal(39, mmCropRect.Width);
            Assert.Equal(46, mmCropRect.Height);
        }

#if !NETFRAMEWORK

        [FactWithAutomaticDisplayName]
        public void CastMaui_Rect_to_Rectangle()
        {
            Microsoft.Maui.Graphics.Rect rect = new Microsoft.Maui.Graphics.Rect(50, 100, 150, 25);
            Rectangle Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(150, Rectangle.Width);
            Assert.Equal(25, Rectangle.Height);
            Assert.Equal(50, Rectangle.X);
            Assert.Equal(100, Rectangle.Y);

            rect = new Microsoft.Maui.Graphics.Rect(150, 25, 50, 100);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(50, Rectangle.Width);
            Assert.Equal(100, Rectangle.Height);
            Assert.Equal(150, Rectangle.X);
            Assert.Equal(25, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_Rect_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(50, 25, 50, 50);
            Microsoft.Maui.Graphics.Rect rect = Rectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            Rectangle = new Rectangle(15, 15, 200, 200);
            rect = Rectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }
        [FactWithAutomaticDisplayName]
        public void CastMaui_RectF_to_Rectangle()
        {
            Microsoft.Maui.Graphics.RectF rect = new Microsoft.Maui.Graphics.RectF(50, 100, 150, 25);
            Rectangle Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(150, Rectangle.Width);
            Assert.Equal(25, Rectangle.Height);
            Assert.Equal(50, Rectangle.X);
            Assert.Equal(100, Rectangle.Y);

            rect = new Microsoft.Maui.Graphics.RectF(150, 25, 50, 100);
            Rectangle = rect;
            Assert.NotNull(Rectangle);
            Assert.Equal(50, Rectangle.Width);
            Assert.Equal(100, Rectangle.Height);
            Assert.Equal(150, Rectangle.X);
            Assert.Equal(25, Rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_RectF_from_Rectangle()
        {
            Rectangle Rectangle = new Rectangle(50, 25, 50, 50);
            Microsoft.Maui.Graphics.RectF rect = Rectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            Rectangle = new Rectangle(15, 15, 200, 200);
            rect = Rectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }
#endif
    }
}
