using FluentAssertions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class IronSkiasharpBitmapFunctionality : Compare
    {
        public IronSkiasharpBitmapFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Resize_SKBitmap_Scale()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);

            SkiaSharp.SKBitmap croppedBitmap = bitmap.Resize(0.5f);

            Assert.Equal(300, croppedBitmap.Width);
            Assert.Equal(239, croppedBitmap.Height);

            SaveSkiaBitmap(croppedBitmap, "result-skiasharp-resized.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-resized.jpeg"), "result-skiasharp-resized.jpeg");
        }

        [FactWithAutomaticDisplayName]
        public void Resize_SKBitmap_Width_and_Height()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);

            SkiaSharp.SKBitmap croppedBitmap = bitmap.Resize(300, 250);

            Assert.Equal(300, croppedBitmap.Width);
            Assert.Equal(250, croppedBitmap.Height);

            SaveSkiaBitmap(croppedBitmap, "result-skiasharp-resized.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-resized-by-size.jpeg"), "result-skiasharp-resized.jpeg");
        }

        [FactWithAutomaticDisplayName]
        public void Crop_SKBitmap()
        {
            CropRectangle cropArea = new CropRectangle(850, 10, 1000, 200);

            string imagePath = GetRelativeFilePath("IronBitmap", "test-cropped.jpeg");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);

            SkiaSharp.SKBitmap croppedBitmap = bitmap.CropImage(cropArea);

            Assert.Equal(904, croppedBitmap.Width);
            Assert.Equal(200, croppedBitmap.Height);

            SaveSkiaBitmap(croppedBitmap, "result-skiasharp-cropped.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-cropped.jpeg"), "result-skiasharp-cropped.jpeg");

            imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            bitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            croppedBitmap = bitmap.CropImage(300, 500);

            Assert.Equal(300, croppedBitmap.Width);
            Assert.Equal(500, croppedBitmap.Height);

            SaveSkiaBitmap(croppedBitmap, "result-skiasharp-cropped.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-cropped-by-size.jpeg"), "result-skiasharp-cropped.jpeg");
        }

        [FactWithAutomaticDisplayName]
        public void Trim_SKBitmap()
        {
            string imagePath = GetRelativeFilePath("IronBitmap", "white-border.png");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);

            SkiaSharp.SKBitmap trimmedBitmap = bitmap.Trim();

            SaveSkiaBitmap(trimmedBitmap, "result-skiasharp-trimmed.png");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-trimmed.png"), "result-skiasharp-trimmed.png");
        }

        [FactWithAutomaticDisplayName]
        public void Rotate_SKBitmap()
        {
            string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            SkiaSharp.SKBitmap rotatedBitmap = bitmap.RotateImage(90);
            SaveSkiaBitmap(rotatedBitmap, "result-skiasharp-rotated.jpg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-rotated-90.jpg"), "result-skiasharp-rotated.jpg");

            imagePath = GetRelativeFilePath("rotate_image.png");
            bitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            rotatedBitmap = bitmap.RotateImage(-45);
            SaveSkiaBitmap(rotatedBitmap, "result-skiasharp-rotated.jpg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-rotated-45.jpg"), "result-skiasharp-rotated.jpg");
        }

        [FactWithAutomaticDisplayName]
        public void Add_Border_to_SKBitmap()
        {
            string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);

            int borderWidth = Math.Max(25, bitmap.Width / 20);
            SkiaSharp.SKBitmap borderedBitmap = bitmap.AddBorder(Color.White, borderWidth);
            SaveSkiaBitmap(borderedBitmap, "result-skiasharp-bordered.jpg");
            Assert.Equal(borderWidth * 2 + bitmap.Width, borderedBitmap.Width);
            Assert.Equal(borderWidth * 2 + bitmap.Height, borderedBitmap.Height);
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-bordered.jpg"), "result-skiasharp-bordered.jpg");
        }

        [FactWithAutomaticDisplayName]
        public void Sharpen_SKBitmap()
        {
            string imagePath = GetRelativeFilePath("IRON-332 Input.png");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);

            SkiaSharp.SKBitmap trimmed = bitmap.Trim();

            SkiaSharp.SKBitmap sharpped = trimmed.Sharpen();

            for (int i = 0; i < sharpped.Width; i++)
            {
                for (int j = 0; j < sharpped.Height; j++)
                {
                    Color color = sharpped.GetPixel(i, j);
                    color.GetLuminance().Should().BeOneOf(0, 100);
                }
            }

            SaveSkiaBitmap(sharpped, "sharppen-image.png");

            bitmap.Dispose();
            trimmed.Dispose();
            sharpped.Dispose();
        }

        [FactWithAutomaticDisplayName]
        public void GetDeskew_SKBitmap()
        {
            string imagePath = GetRelativeFilePath(@"IronBitmap/rotated-image.jpg");
            SkiaSharp.SKBitmap bitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            SkiaSharp.SKBitmap result = bitmap.RotateImage(-IronSkiasharpBitmap.GetSkewAngle(bitmap));
            IronSkiasharpBitmap.GetSkewAngle(result).Should().BeApproximately(0, 1);
            result.Dispose();
            bitmap.Dispose();

            imagePath = GetRelativeFilePath(@"IronBitmap/bcnotdetected.gif");
            bitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            result = bitmap.RotateImage(-IronSkiasharpBitmap.GetSkewAngle(bitmap));
            IronSkiasharpBitmap.GetSkewAngle(result).Should().BeApproximately(0, 1);
            result.Dispose();
            bitmap.Dispose();

        }
    }
}
