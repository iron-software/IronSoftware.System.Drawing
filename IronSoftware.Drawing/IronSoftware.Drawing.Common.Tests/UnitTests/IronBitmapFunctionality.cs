using System;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class IronBitmapFunctionality : Compare
    {
        public IronBitmapFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Resize_AnyBitmap_Scale()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            AnyBitmap croppedBitmap = anyBitmap.Resize(0.5f);

            Assert.Equal(300, croppedBitmap.Width);
            Assert.Equal(239, croppedBitmap.Height);

            croppedBitmap.SaveAs("result-resized.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-resized.jpeg"), "result-resized.jpeg");
        }

        [FactWithAutomaticDisplayName]
        public void Resize_AnyBitmap_Width_and_Height()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            AnyBitmap croppedBitmap = anyBitmap.Resize(300, 250);

            Assert.Equal(300, croppedBitmap.Width);
            Assert.Equal(250, croppedBitmap.Height);

            croppedBitmap.SaveAs("result-resized.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-resized-by-size.jpeg"), "result-resized.jpeg");
        }

        [FactWithAutomaticDisplayName]
        public void Crop_AnyBitmap()
        {
            CropRectangle cropArea = new CropRectangle(850, 10, 1000, 200);

            string imagePath = GetRelativeFilePath("IronBitmap", "test-cropped.jpeg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            AnyBitmap croppedBitmap = anyBitmap.CropImage(cropArea);

            Assert.Equal(904, croppedBitmap.Width);
            Assert.Equal(200, croppedBitmap.Height);

            croppedBitmap.SaveAs("result-cropped.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-cropped.jpeg"), "result-cropped.jpeg");

            imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            anyBitmap = new AnyBitmap(imagePath);
            croppedBitmap = anyBitmap.CropImage(300, 500);

            Assert.Equal(300, croppedBitmap.Width);
            Assert.Equal(500, croppedBitmap.Height);

            croppedBitmap.SaveAs("result-cropped.jpeg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-cropped-by-size.jpeg"), "result-cropped.jpeg");
        }

        [FactWithAutomaticDisplayName]
        public void Trim_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("IronBitmap", "white-border.png");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            AnyBitmap trimmedBitmap = anyBitmap.Trim();

            trimmedBitmap.SaveAs("result-trimmed.png");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-trimmed.png"), "result-trimmed.png");
        }

        [FactWithAutomaticDisplayName]
        public void Rotate_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);
            AnyBitmap rotatedBitmap = anyBitmap.RotateImage(90);
            rotatedBitmap.SaveAs("result-rotated.jpg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-rotated-90.jpg"), "result-rotated.jpg");

            imagePath = GetRelativeFilePath("rotate_image.png");
            anyBitmap = new AnyBitmap(imagePath);
            rotatedBitmap = anyBitmap.RotateImage(-45);
            rotatedBitmap.SaveAs("result-rotated.jpg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-rotated-45.jpg"), "result-rotated.jpg");

#if NETCOREAPP2_1
            System.Drawing.Bitmap bitmap;
            var ex = Assert.Throws<PlatformNotSupportedException>(() => System.Math.Ceiling(IronBitmap.DetermineSkewAngle(anyBitmap)));
            Assert.Equal("System.Drawing is not supported on this platform.", ex.Message);

            ex = Assert.Throws<PlatformNotSupportedException>(() => rotatedBitmap = anyBitmap.RotateImage());
            Assert.Equal("System.Drawing is not supported on this platform.", ex.Message);
#else
            Assert.Equal(45, System.Math.Ceiling(IronBitmap.DetermineSkewAngle(anyBitmap)));
            rotatedBitmap = anyBitmap.RotateImage();
            rotatedBitmap.SaveAs("result-rotated.jpg");
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-rotated-45.jpg"), "result-rotated.jpg");
#endif
        }

        [FactWithAutomaticDisplayName]
        public void Add_Border_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            int borderWidth = Math.Max(25, anyBitmap.Width / 20);

            AnyBitmap borderedBitmap = anyBitmap.AddBorder(Color.White, borderWidth);
            borderedBitmap.SaveAs("result-bordered.jpg");
            Assert.Equal(borderWidth * 2 + anyBitmap.Width, borderedBitmap.Width);
            Assert.Equal(borderWidth * 2 + anyBitmap.Height, borderedBitmap.Height);
            AssertImageAreEqual(GetRelativeFilePath("IronBitmap", "expected-bordered.jpg"), "result-bordered.jpg");
        }
    }
}
