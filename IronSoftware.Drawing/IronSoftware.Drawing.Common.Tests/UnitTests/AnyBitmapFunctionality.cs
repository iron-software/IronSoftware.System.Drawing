using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
#if NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER
using SixLabors.ImageSharp;
#endif

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public class AnyBitmapFunctionality : Compare
    {
        public AnyBitmapFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Filename()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");

            AnyBitmap bitmap = AnyBitmap.FromFile(imagePath);
            bitmap.SaveAs("result.bmp");
            Assert.Equal(671, bitmap.Width);
            Assert.Equal(1000, bitmap.Height);
            Assert.Equal(74684, bitmap.Length);
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(imagePath);
            bitmap.SaveAs("result.bmp");
            Assert.Equal(671, bitmap.Width);
            Assert.Equal(1000, bitmap.Height);
            Assert.Equal(74684, bitmap.Length);
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Byte()
        {            
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            byte[] bytes = File.ReadAllBytes(imagePath);

            AnyBitmap bitmap = AnyBitmap.FromBytes(bytes);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(bytes);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Stream()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            byte[] bytes = File.ReadAllBytes(imagePath);
            MemoryStream ms = new MemoryStream(bytes);

            AnyBitmap bitmap = AnyBitmap.FromStream(ms);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(ms);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Try_Save_Bitmap_with_Format()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            anyBitmap.SaveAs("result-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-png.png"));
            CleanResultFile("result-png.png");

            anyBitmap.SaveAs("result-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-png-loss.png"));
            CleanResultFile("result-png-loss.png");

            anyBitmap.TrySaveAs("result-try-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-try-png.png"));
            CleanResultFile("result-try-png.png");

            anyBitmap.TrySaveAs("result-try-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-try-png-loss.png"));
            CleanResultFile("result-try-png-loss.png");
        }

        [FactWithAutomaticDisplayName]
        public void Export_file()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = new AnyBitmap(imagePath);

            anyBitmap.ExportFile("result.png");
            Assert.True(File.Exists("result.png"));
            CleanResultFile("result.png");

            anyBitmap.ExportFile("result-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-png.png"));
            CleanResultFile("result-png.png");

            anyBitmap.ExportFile("result-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-png-loss.png"));
            CleanResultFile("result-png-loss.png");
        }

        [FactWithAutomaticDisplayName]
        public void CastBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
#if NETCOREAPP2_1
            System.Drawing.Bitmap bitmap;
            var ex = Assert.Throws<PlatformNotSupportedException>(() => bitmap = new System.Drawing.Bitmap(imagePath));
            Assert.Equal("System.Drawing is not supported on this platform.", ex.Message);
#else
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath);
            AnyBitmap anyBitmap = bitmap;

            bitmap.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
#endif
        }

        [FactWithAutomaticDisplayName]
        public void CastBitmap_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
#if NETCOREAPP2_1
            System.Drawing.Bitmap bitmap;
            var ex = Assert.Throws<PlatformNotSupportedException>(() => bitmap = anyBitmap);
            if (IsUnix())
            {
                Assert.Equal($"Microsoft has chosen to no longer support System.Drawing.Common on Linux or MacOS. To solve this please use another Bitmap type such as {typeof(System.Drawing.Bitmap).ToString()}, SkiaSharp or ImageSharp.\n\nhttps://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only", ex.Message);
            }
            else
            {
                Assert.Equal("System.Drawing is not supported on this platform.", ex.Message);
            }
#else
            System.Drawing.Bitmap bitmap = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
#endif
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_equal_Bitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
#if NETCOREAPP2_1
            System.Drawing.Bitmap bitmap;
            var ex = Assert.Throws<PlatformNotSupportedException>(() => bitmap = new System.Drawing.Bitmap(imagePath));
            Assert.Equal("System.Drawing is not supported on this platform.", ex.Message);
#else
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath);
            AnyBitmap compareAnyBitmap = bitmap;

            if (!IsUnix())
            { 
                Assert.True(anyBitmap.Equals(compareAnyBitmap));
            }

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");
            AssertImageAreEqual("expected.bmp", "result.bmp", true);
#endif
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Bytes()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            byte[] expected = File.ReadAllBytes(imagePath);

            byte[] result = anyBitmap.GetBytes();
            Assert.Equal(expected, result);

            byte[] resultExport = anyBitmap.ExportBytes();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Stream()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            MemoryStream expected = new MemoryStream(File.ReadAllBytes(imagePath));

            MemoryStream result = anyBitmap.GetStream();
            AssertStreamAreEqual(expected, result);

            result = anyBitmap.ToStream();
            AssertStreamAreEqual(expected, result);

            using var resultExport = new System.IO.MemoryStream();
            anyBitmap.ExportStream(resultExport);
            AssertStreamAreEqual(expected, (MemoryStream)resultExport);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Hashcode()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);

            byte[] bytes = System.IO.File.ReadAllBytes(imagePath);
            Assert.Equal(bytes, anyBitmap.GetBytes());

            int expected = anyBitmap.GetBytes().GetHashCode();
            int result = anyBitmap.GetHashCode();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_ToString()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);

            byte[] bytes = File.ReadAllBytes(imagePath);
            string expected = Convert.ToBase64String(bytes, 0, bytes.Length);

            string result = anyBitmap.ToString();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void Clone_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            AnyBitmap clonedAnyBitmap = anyBitmap.Clone();

            anyBitmap.SaveAs("expected.png");
            clonedAnyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKBitmap skBitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            AnyBitmap anyBitmap = skBitmap;

            SaveSkiaImage(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SkiaSharp.SKBitmap skBitmap = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaImage(skBitmap, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKImage_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKImage skImage = SkiaSharp.SKImage.FromBitmap(SkiaSharp.SKBitmap.Decode(imagePath));
            AnyBitmap anyBitmap = skImage;

            SaveSkiaImage(skImage, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKImage_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SkiaSharp.SKImage skImage = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaImage(skImage, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

#if NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER

        [FactWithAutomaticDisplayName]
        public void CastSixLabors_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SixLabors.ImageSharp.Image imgSharp = SixLabors.ImageSharp.Image.Load(imagePath);
            AnyBitmap anyBitmap = imgSharp;

            imgSharp.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLabors_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SixLabors.ImageSharp.Image imgSharp = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            imgSharp.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            byte[] bytes = File.ReadAllBytes(imagePath);
            Microsoft.Maui.Graphics.Platform.PlatformImage image = (Microsoft.Maui.Graphics.Platform.PlatformImage)Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(new MemoryStream(bytes));
            AnyBitmap anyBitmap = image;

            SaveMauiImages(image, "expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            Microsoft.Maui.Graphics.Platform.PlatformImage image = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            SaveMauiImages(image, "result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

#endif

    }
}
