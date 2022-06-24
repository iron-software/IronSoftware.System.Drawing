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
        public void Crop_AnyBitmap()
        {
            CropRectangle cropArea = new CropRectangle(850, 10, 1000, 200);

            string imagePath = GetRelativeFilePath("test-cropped.jpeg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            AnyBitmap croppedBitmap = anyBitmap.CropImage(cropArea);

            Assert.Equal(904, croppedBitmap.Width);
            Assert.Equal(200, croppedBitmap.Height);
        }
    }
}
