using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class CropRectangleFunctionality : TestsBase
    {
        public CropRectangleFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_new_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle();
            Assert.NotNull(cropRectangle);
            Assert.Equal(0, cropRectangle.Width);
            Assert.Equal(0, cropRectangle.Height);
            Assert.Equal(0, cropRectangle.X);
            Assert.Equal(0, cropRectangle.Y);

            cropRectangle.Width = 100;
            cropRectangle.Height = 100;
            cropRectangle.X = 25;
            cropRectangle.Y = 50;
            Assert.Equal(100, cropRectangle.Width);
            Assert.Equal(100, cropRectangle.Height);
            Assert.Equal(25, cropRectangle.X);
            Assert.Equal(50, cropRectangle.Y);

            cropRectangle = new CropRectangle(5, 5, 50, 50);
            Assert.NotNull(cropRectangle);
            Assert.Equal(50, cropRectangle.Width);
            Assert.Equal(50, cropRectangle.Height);
            Assert.Equal(5, cropRectangle.X);
            Assert.Equal(5, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastRectangle_to_CropRectangle()
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(10, 10, 150, 150);
            CropRectangle cropRectangle = rectangle;
            Assert.NotNull(cropRectangle);
            Assert.Equal(150, cropRectangle.Width);
            Assert.Equal(150, cropRectangle.Height);
            Assert.Equal(10, cropRectangle.X);
            Assert.Equal(10, cropRectangle.Y);

            rectangle = new System.Drawing.Rectangle(new System.Drawing.Point(15, 15), new System.Drawing.Size(75, 75));
            cropRectangle = rectangle;
            Assert.NotNull(cropRectangle);
            Assert.Equal(75, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(15, cropRectangle.X);
            Assert.Equal(15, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastRectangle_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(5, 5, 50, 50);

            System.Drawing.Rectangle rectangle = cropRectangle;

            Assert.NotNull(cropRectangle);
            Assert.Equal(50, rectangle.Width);
            Assert.Equal(50, rectangle.Height);
            Assert.Equal(5, rectangle.X);
            Assert.Equal(5, rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_Rectangle_to_CropRectangle()
        {
            SixLabors.ImageSharp.Rectangle rectangle = new SixLabors.ImageSharp.Rectangle(10, 10, 150, 150);
            CropRectangle cropRectangle = rectangle;
            Assert.NotNull(cropRectangle);
            Assert.Equal(150, cropRectangle.Width);
            Assert.Equal(150, cropRectangle.Height);
            Assert.Equal(10, cropRectangle.X);
            Assert.Equal(10, cropRectangle.Y);

            rectangle = new SixLabors.ImageSharp.Rectangle(new SixLabors.ImageSharp.Point(15, 15), new SixLabors.ImageSharp.Size(75, 75));
            cropRectangle = rectangle;
            Assert.NotNull(cropRectangle);
            Assert.Equal(75, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(15, cropRectangle.X);
            Assert.Equal(15, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_Rectangle_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(5, 5, 50, 50);

            SixLabors.ImageSharp.Rectangle rectangle = cropRectangle;

            Assert.NotNull(cropRectangle);
            Assert.Equal(50, rectangle.Width);
            Assert.Equal(50, rectangle.Height);
            Assert.Equal(5, rectangle.X);
            Assert.Equal(5, rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_RectangleF_to_CropRectangle()
        {
            SixLabors.ImageSharp.RectangleF rectangle = new SixLabors.ImageSharp.RectangleF(10, 10, 150, 150);
            CropRectangle cropRectangle = rectangle;
            Assert.NotNull(cropRectangle);
            Assert.Equal(150, cropRectangle.Width);
            Assert.Equal(150, cropRectangle.Height);
            Assert.Equal(10, cropRectangle.X);
            Assert.Equal(10, cropRectangle.Y);

            rectangle = new SixLabors.ImageSharp.Rectangle(new SixLabors.ImageSharp.Point(15, 15), new SixLabors.ImageSharp.Size(75, 75));
            cropRectangle = rectangle;
            Assert.NotNull(cropRectangle);
            Assert.Equal(75, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(15, cropRectangle.X);
            Assert.Equal(15, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastImageSharp_RectangleF_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(5, 5, 50, 50);

            SixLabors.ImageSharp.RectangleF rectangle = cropRectangle;

            Assert.NotNull(cropRectangle);
            Assert.Equal(50, rectangle.Width);
            Assert.Equal(50, rectangle.Height);
            Assert.Equal(5, rectangle.X);
            Assert.Equal(5, rectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRect_to_CropRectangle()
        {
            SkiaSharp.SKRect rect = new SkiaSharp.SKRect(50, 100, 150, 25);
            CropRectangle cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(100, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(50, cropRectangle.X);
            Assert.Equal(25, cropRectangle.Y);
            
            rect = new SkiaSharp.SKRect(150, 25, 50, 100);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(100, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(50, cropRectangle.X);
            Assert.Equal(25, cropRectangle.Y);

            rect = SkiaSharp.SKRect.Create(200, 200);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(200, cropRectangle.Width);
            Assert.Equal(200, cropRectangle.Height);
            Assert.Equal(0, cropRectangle.X);
            Assert.Equal(0, cropRectangle.Y);

            rect = SkiaSharp.SKRect.Create(15, 15, 200, 200);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(200, cropRectangle.Width);
            Assert.Equal(200, cropRectangle.Height);
            Assert.Equal(15, cropRectangle.X);
            Assert.Equal(15, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRect_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(50, 25, 50, 50);
            SkiaSharp.SKRect rect = cropRectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            cropRectangle = new CropRectangle(15, 15, 200, 200);
            rect = cropRectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRectI_to_CropRectangle()
        {
            SkiaSharp.SKRectI rect = new SkiaSharp.SKRectI(50, 100, 150, 25);
            CropRectangle cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(100, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(50, cropRectangle.X);
            Assert.Equal(25, cropRectangle.Y);

            rect = new SkiaSharp.SKRectI(150, 25, 50, 100);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(100, cropRectangle.Width);
            Assert.Equal(75, cropRectangle.Height);
            Assert.Equal(50, cropRectangle.X);
            Assert.Equal(25, cropRectangle.Y);

            rect = SkiaSharp.SKRectI.Create(200, 200);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(200, cropRectangle.Width);
            Assert.Equal(200, cropRectangle.Height);
            Assert.Equal(0, cropRectangle.X);
            Assert.Equal(0, cropRectangle.Y);

            rect = SkiaSharp.SKRectI.Create(15, 15, 200, 200);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(200, cropRectangle.Width);
            Assert.Equal(200, cropRectangle.Height);
            Assert.Equal(15, cropRectangle.X);
            Assert.Equal(15, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKRectI_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(50, 25, 50, 50);
            SkiaSharp.SKRectI rect = cropRectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            cropRectangle = new CropRectangle(15, 15, 200, 200);
            rect = cropRectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }

#if !NET472

        [FactWithAutomaticDisplayName]
        public void CastMaui_Rect_to_CropRectangle()
        {
            Microsoft.Maui.Graphics.Rect rect = new Microsoft.Maui.Graphics.Rect(50, 100, 150, 25);
            CropRectangle cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(150, cropRectangle.Width);
            Assert.Equal(25, cropRectangle.Height);
            Assert.Equal(50, cropRectangle.X);
            Assert.Equal(100, cropRectangle.Y);

            rect = new Microsoft.Maui.Graphics.Rect(150, 25, 50, 100);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(50, cropRectangle.Width);
            Assert.Equal(100, cropRectangle.Height);
            Assert.Equal(150, cropRectangle.X);
            Assert.Equal(25, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_Rect_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(50, 25, 50, 50);
            Microsoft.Maui.Graphics.Rect rect = cropRectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            cropRectangle = new CropRectangle(15, 15, 200, 200);
            rect = cropRectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }
        [FactWithAutomaticDisplayName]
        public void CastMaui_RectF_to_CropRectangle()
        {
            Microsoft.Maui.Graphics.RectF rect = new Microsoft.Maui.Graphics.RectF(50, 100, 150, 25);
            CropRectangle cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(150, cropRectangle.Width);
            Assert.Equal(25, cropRectangle.Height);
            Assert.Equal(50, cropRectangle.X);
            Assert.Equal(100, cropRectangle.Y);

            rect = new Microsoft.Maui.Graphics.RectF(150, 25, 50, 100);
            cropRectangle = rect;
            Assert.NotNull(cropRectangle);
            Assert.Equal(50, cropRectangle.Width);
            Assert.Equal(100, cropRectangle.Height);
            Assert.Equal(150, cropRectangle.X);
            Assert.Equal(25, cropRectangle.Y);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_RectF_from_CropRectangle()
        {
            CropRectangle cropRectangle = new CropRectangle(50, 25, 50, 50);
            Microsoft.Maui.Graphics.RectF rect = cropRectangle;
            Assert.Equal(50, rect.Left);
            Assert.Equal(25, rect.Top);
            Assert.Equal(100, rect.Right);
            Assert.Equal(75, rect.Bottom);

            cropRectangle = new CropRectangle(15, 15, 200, 200);
            rect = cropRectangle;
            Assert.Equal(15, rect.Left);
            Assert.Equal(15, rect.Top);
            Assert.Equal(215, rect.Right);
            Assert.Equal(215, rect.Bottom);
        }
#endif
    }
}
