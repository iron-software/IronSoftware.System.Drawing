using FluentAssertions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Image = SixLabors.ImageSharp.Image;

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

            var bitmap = AnyBitmap.FromFile(imagePath);
            bitmap.IsImageLoaded().Should().BeFalse();

            bitmap.SaveAs("result.bmp");

            bitmap.IsImageLoaded().Should().BeTrue();
            //should still be the original bytes
            bitmap.Length.Should().Be((int)new FileInfo(imagePath).Length);

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

            var bitmap = AnyBitmap.FromBytes(bytes);
            bitmap.IsImageLoaded().Should().BeFalse();

            _ = bitmap.TrySaveAs("result.bmp");

            bitmap.IsImageLoaded().Should().BeTrue();
            //should still be the original bytes
            bitmap.Length.Should().Be(bytes.Length);

            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(bytes);
            _ = bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Stream()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            byte[] bytes = File.ReadAllBytes(imagePath);
            Stream ms = new MemoryStream(bytes);

            var bitmap = AnyBitmap.FromStream(ms);
            bitmap.IsImageLoaded().Should().BeFalse();

            _ = bitmap.TrySaveAs("result.bmp");

            bitmap.IsImageLoaded().Should().BeTrue();
            //should still be the original bytes
            bitmap.Length.Should().Be(bytes.Length);

            AssertImageAreEqual(imagePath, "result.bmp");

            ms.Position = 0;
            bitmap = new AnyBitmap(ms);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_MemoryStream()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            byte[] bytes = File.ReadAllBytes(imagePath);
            var ms = new MemoryStream(bytes);

            var bitmap = AnyBitmap.FromStream(ms);
            bitmap.IsImageLoaded().Should().BeFalse();
           
            _ = bitmap.TrySaveAs("result.bmp");

            bitmap.IsImageLoaded().Should().BeTrue();
            //should still be the original bytes
            bitmap.Length.Should().Be(bytes.Length);

            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(ms);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");


        }

        [FactWithAutomaticDisplayName]
        public async void Create_AnyBitmap_by_Uri_Async()
        {
            var uri = new Uri("https://ironsoftware.com/img/ironsoftware_hero_section/bg-hero-part.png");

            AnyBitmap bitmap = await AnyBitmap.FromUriAsync(uri);
            _ = bitmap.TrySaveAs("result.bmp");
            AssertImageExist("result.bmp", true);

            bitmap = new AnyBitmap(uri);
            _ = bitmap.TrySaveAs("result.bmp");
            AssertImageExist("result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void Create_SVG_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("Example_barcode.svg");
            var bitmap = AnyBitmap.FromFile(imagePath);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Try_Save_Bitmap_with_Format()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = new AnyBitmap(imagePath);

            anyBitmap.SaveAs("result-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-png.png"));
            CleanResultFile("result-png.png");

            anyBitmap.SaveAs("result-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-png-loss.png"));
            CleanResultFile("result-png-loss.png");

            _ = anyBitmap.TrySaveAs("result-try-png.png", AnyBitmap.ImageFormat.Png);
            Assert.True(File.Exists("result-try-png.png"));
            CleanResultFile("result-try-png.png");

            _ = anyBitmap.TrySaveAs("result-try-png-loss.png", AnyBitmap.ImageFormat.Png, 50);
            Assert.True(File.Exists("result-try-png-loss.png"));
            CleanResultFile("result-try-png-loss.png");
        }

        [FactWithAutomaticDisplayName]
        public void Export_file()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = new AnyBitmap(imagePath);

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

        [IgnoreOnUnixFact]
        public void CastBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var bitmap = new System.Drawing.Bitmap(imagePath);
            AnyBitmap anyBitmap = bitmap;

            bitmap.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [IgnoreOnUnixFact]
        public void CastBitmap_from_AnyBitmap()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            System.Drawing.Bitmap bitmap = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [IgnoreOnUnixFact]
        public void CastImage_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var bitmap = System.Drawing.Image.FromFile(imagePath);
            AnyBitmap anyBitmap = bitmap;

            bitmap.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [IgnoreOnUnixFact]
        public void CastImage_from_AnyBitmap()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            System.Drawing.Image bitmap = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Bytes()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            byte[] expected = File.ReadAllBytes(imagePath);

            byte[] result = anyBitmap.GetBytes();
            Assert.Equal(expected, result);

            byte[] resultExport = anyBitmap.ExportBytes();
            Assert.Equal(expected, resultExport);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_set_Pixel()
        {
            string imagePath = GetRelativeFilePath("checkmark.jpg");

            using Image<Rgb24> formatRgb24 = Image.Load<Rgb24>(imagePath);
            using Image<Abgr32> formatAbgr32 = Image.Load<Abgr32>(imagePath);
            using Image<Argb32> formatArgb32 = Image.Load<Argb32>(imagePath);
            using Image<Bgr24> formatBgr24 = Image.Load<Bgr24>(imagePath);
            using Image<Bgra32> formatBgra32 = Image.Load<Bgra32>(imagePath);

            Image[] images = { formatRgb24, formatAbgr32, formatArgb32, formatBgr24, formatBgra32 };

            foreach (Image image in images)
            {
                AnyBitmap bitmap = (AnyBitmap)image;

                // Get the current pixel color - should be white
                var pixelBefore = bitmap.GetPixel(0, 0);

                // Check current pixel color is not black
                Assert.NotEqual(pixelBefore, Color.Black);

                // Set the pixel color to black
                bitmap.SetPixel(0, 0, Color.Black);

                // Check the pixel color has changed
                Assert.Equal(bitmap.GetPixel(0, 0), Color.Black);

#if NETFRAMEWORK
                //windows only
                // SetPixel makes the image dirty so it should update AnyBitmap.Binary value

                System.Drawing.Bitmap temp1 = bitmap;
                AnyBitmap temp2 = (AnyBitmap)temp1;
                Assert.Equal(temp1.GetPixel(0, 0).ToArgb(), System.Drawing.Color.Black.ToArgb());
                Assert.Equal(temp2.GetPixel(0, 0), Color.Black);
#endif
            }
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Stream()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            var expected = new MemoryStream(File.ReadAllBytes(imagePath));

            MemoryStream result = anyBitmap.GetStream();
            AssertStreamAreEqual(expected, result);

            result = anyBitmap.ToStream();
            AssertStreamAreEqual(expected, result);

            Func<Stream> funcStream = anyBitmap.ToStreamFn();
            AssertStreamAreEqual(expected, funcStream);

            using var resultExport = new MemoryStream();
            anyBitmap.ExportStream(resultExport);
            AssertStreamAreEqual(expected, resultExport);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_get_Hashcode()
        {
            string imagePath = GetRelativeFilePath("Mona-Lisa-oil-wood-panel-Leonardo-da.webp");
            var anyBitmap = AnyBitmap.FromFile(imagePath);

            byte[] bytes = File.ReadAllBytes(imagePath);
            Assert.Equal(bytes, anyBitmap.GetBytes());

            int expected = anyBitmap.GetBytes().GetHashCode();
            int result = anyBitmap.GetHashCode();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_should_ToString()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);

            byte[] bytes = File.ReadAllBytes(imagePath);
            string expected = Convert.ToBase64String(bytes, 0, bytes.Length);

            string result = anyBitmap.ToString();
            Assert.Equal(expected, result);
        }

        [FactWithAutomaticDisplayName]
        public void AnyBitmap_GetPixel_should_not_throw_OutOfMemoryException()
        {
            //Should not throw exception
            //previously GetPixel always called CloneAs() which resulted to OutOfMemoryException
            string imagePath = GetRelativeFilePath("google_large_1500dpi.bmp");
            using Image<Rgb24> formatRgb24 = Image.Load<Rgb24>(imagePath);
            using Image<Abgr32> formatAbgr32 = Image.Load<Abgr32>(imagePath);
            using Image<Argb32> formatArgb32 = Image.Load<Argb32>(imagePath);
            using Image<Bgr24> formatBgr24 = Image.Load<Bgr24>(imagePath);
            using Image<Bgra32> formatBgra32 = Image.Load<Bgra32>(imagePath);

            Image[] images = { formatRgb24, formatAbgr32, formatArgb32, formatBgr24, formatBgra32 };

            foreach (Image image in images)
            {
                AnyBitmap bitmap = (AnyBitmap)image;

                int hash = 0;
                for (int y = 0; y < bitmap.Height; y += 8)
                {
                    for (int x = 0; x < bitmap.Width; x += 8)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        hash += pixel.ToArgb();
                    }
                }
            }
        }

        [FactWithAutomaticDisplayName]
        public void Clone_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            AnyBitmap clonedAnyBitmap = anyBitmap.Clone();

            anyBitmap.SaveAs("expected.png");
            clonedAnyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            using Image image = anyBitmap;
            image.Mutate(img => img.Crop(new Rectangle(0, 0, 100, 100)));
            AnyBitmap clonedWithRect = anyBitmap.Clone(new Rectangle(0, 0, 100, 100));

            image.SaveAsPng("expected.png");
            clonedWithRect.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var skBitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            AnyBitmap anyBitmap = skBitmap;

            SaveSkiaBitmap(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_from_AnyBitmap()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SkiaSharp.SKBitmap skBitmap = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaBitmap(skBitmap, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("Sample-Tiff-File-download-for-Testing.tiff"));
            skBitmap = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaBitmap(skBitmap, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKImage_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var skImage = SkiaSharp.SKImage.FromBitmap(SkiaSharp.SKBitmap.Decode(imagePath));
            AnyBitmap anyBitmap = skImage;

            SaveSkiaImage(skImage, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKImage_from_AnyBitmap()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            SkiaSharp.SKImage skImage = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaImage(skImage, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);

            anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("Sample-Tiff-File-download-for-Testing.tiff"));
            skImage = anyBitmap;

            anyBitmap.SaveAs("expected.png");
            SaveSkiaImage(skImage, "result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLabors_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
            var imgSharp = Image.Load<Rgba32>(imagePath);
            AnyBitmap anyBitmap = imgSharp;

            imgSharp.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("mountainclimbers.jpg")]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg")]
        [InlineData("animated_qr.gif")]
        [InlineData("Sample-Tiff-File-download-for-Testing.tiff")]
        public void CastSixLabors_from_AnyBitmap(string filename)
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath(filename));
            Image imgSharp = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            imgSharp.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("mountainclimbers.jpg")]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg")]
        [InlineData("animated_qr.gif")]
        [InlineData("Sample-Tiff-File-download-for-Testing.tiff")]
        public void CastSixLabors_from_AnyBitmap_Rgb24(string filename)
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath(filename));
            Image<Rgb24> imgSharp = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            imgSharp.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("mountainclimbers.jpg")]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg")]
        [InlineData("animated_qr.gif")]
        [InlineData("Sample-Tiff-File-download-for-Testing.tiff")]
        public void CastSixLabors_from_AnyBitmap_Rgba32(string filename)
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath(filename));
            Image<Rgba32> imgSharp = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            imgSharp.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastBitmap_to_AnyBitmap_using_FromBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath);
            AnyBitmap anyBitmap = AnyBitmap.FromBitmap(bitmap);

            bitmap.Save("expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void Load_Tiff_Image()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("IRON-274-39065.tif"));
            Assert.Equal(2, anyBitmap.FrameCount);

            var multiPage = AnyBitmap.FromFile(GetRelativeFilePath("animated_qr.gif"));
            Assert.Equal(4, multiPage.FrameCount);
            Assert.Equal(4, multiPage.GetAllFrames.Count());
            multiPage.GetAllFrames.First().SaveAs("first.png");
            multiPage.GetAllFrames.Last().SaveAs("last.png");
            AssertImageAreEqual(GetRelativeFilePath("first-animated-qr.png"), "first.png");
            AssertImageAreEqual(GetRelativeFilePath("last-animated-qr.png"), "last.png");

            byte[] bytes = File.ReadAllBytes(GetRelativeFilePath("IRON-274-39065.tif"));
            anyBitmap = AnyBitmap.FromBytes(bytes);
            Assert.Equal(2, anyBitmap.FrameCount);

            byte[] multiPageBytes = File.ReadAllBytes(GetRelativeFilePath("animated_qr.gif"));
            multiPage = AnyBitmap.FromBytes(multiPageBytes);
            Assert.Equal(4, multiPage.FrameCount);
            Assert.Equal(4, multiPage.GetAllFrames.Count());
            multiPage.GetAllFrames.First().SaveAs("first.png");
            multiPage.GetAllFrames.Last().SaveAs("last.png");
            AssertImageAreEqual(GetRelativeFilePath("first-animated-qr.png"), "first.png");
            AssertImageAreEqual(GetRelativeFilePath("last-animated-qr.png"), "last.png");
        }

        [FactWithAutomaticDisplayName]
        public void Try_UnLoad_Tiff_Image()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("multiframe.tiff"));
            Assert.Equal(2, anyBitmap.FrameCount);
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Tiff()
        {
            var bitmaps = new List<AnyBitmap>()
            {
                AnyBitmap.FromFile(GetRelativeFilePath("first-animated-qr.png")),
                AnyBitmap.FromFile(GetRelativeFilePath("last-animated-qr.png"))
            };

            var anyBitmap = AnyBitmap.CreateMultiFrameTiff(bitmaps);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            AssertImageAreEqual(GetRelativeFilePath("first-animated-qr.png"), "first.png");
            AssertImageAreEqual(GetRelativeFilePath("last-animated-qr.png"), "last.png");
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Tiff_Paths()
        {
            string outputImagePath = "create-tiff-output.tif";
            var imagePaths = new List<string>()
            {
                GetRelativeFilePath("first-animated-qr.png"),
                GetRelativeFilePath("last-animated-qr.png")
            };
            long maxInputFileSize = imagePaths.Select(path => new FileInfo(path).Length).Max();

            var anyBitmap = AnyBitmap.CreateMultiFrameTiff(imagePaths);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            AssertImageAreEqual(GetRelativeFilePath("first-animated-qr.png"), "first.png");
            AssertImageAreEqual(GetRelativeFilePath("last-animated-qr.png"), "last.png");

            anyBitmap.SaveAs(outputImagePath);

            long outputFileSize = new FileInfo(outputImagePath).Length;
            outputFileSize.Should().BeLessThanOrEqualTo(maxInputFileSize, $"Output file size ({outputFileSize}) exceeds the maximum input file size ({maxInputFileSize}).");

            File.Delete(outputImagePath);
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Gif()
        {
            var bitmaps = new List<AnyBitmap>()
            {
                AnyBitmap.FromFile(GetRelativeFilePath("first-animated-qr.png")),
                AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"))
            };

            var anyBitmap = AnyBitmap.CreateMultiFrameGif(bitmaps);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            var first = Image.Load(GetRelativeFilePath("first-animated-qr.png"));
            first.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new Size(anyBitmap.GetAllFrames.ElementAt(0).Width, anyBitmap.GetAllFrames.ElementAt(0).Height),
                Mode = ResizeMode.BoxPad
            }));
            first.Save("first-expected.jpg");
            AssertImageAreEqual("first-expected.jpg", "first.png", true);

            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            var last = Image.Load(GetRelativeFilePath("mountainclimbers.jpg"));
            last.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new Size(anyBitmap.GetAllFrames.ElementAt(1).Width, anyBitmap.GetAllFrames.ElementAt(1).Height),
                Mode = ResizeMode.BoxPad
            }));
            last.Save("last-expected.jpg");
            AssertImageAreEqual("last-expected.jpg", "last.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Gif_paths()
        {
            var imagePaths = new List<string>()
            {
                GetRelativeFilePath("first-animated-qr.png"),
                GetRelativeFilePath("mountainclimbers.jpg")
            };

            var anyBitmap = AnyBitmap.CreateMultiFrameGif(imagePaths);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            var first = Image.Load(GetRelativeFilePath("first-animated-qr.png"));
            first.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new Size(anyBitmap.GetAllFrames.ElementAt(0).Width, anyBitmap.GetAllFrames.ElementAt(0).Height),
                Mode = ResizeMode.BoxPad
            }));
            first.Save("first-expected.jpg");
            AssertImageAreEqual("first-expected.jpg", "first.png", true);

            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            var last = Image.Load(GetRelativeFilePath("mountainclimbers.jpg"));
            last.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new Size(anyBitmap.GetAllFrames.ElementAt(1).Width, anyBitmap.GetAllFrames.ElementAt(1).Height),
                Mode = ResizeMode.BoxPad
            }));
            last.Save("last-expected.jpg");
            AssertImageAreEqual("last-expected.jpg", "last.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void Should_Return_BitsPerPixel()
        {
            var bitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            Assert.Equal(24, bitmap.BitsPerPixel);

            bitmap = Image.Load<Rgba32>(GetRelativeFilePath("mountainclimbers.jpg"));
            Assert.Equal(32, bitmap.BitsPerPixel);
        }

        [TheoryWithAutomaticDisplayName()]
        [InlineData("mountainclimbers.jpg", "image/jpeg", AnyBitmap.ImageFormat.Jpeg)]
        [InlineData("watermark.deployment.png", "image/png", AnyBitmap.ImageFormat.Png)]
        [InlineData("animated_qr.gif", "image/gif", AnyBitmap.ImageFormat.Gif)]
        [InlineData("Mona-Lisa-oil-wood-panel-Leonardo-da.webp", "image/webp", AnyBitmap.ImageFormat.Webp)]
        [InlineData("multiframe.tiff", "image/tiff", AnyBitmap.ImageFormat.Tiff)]
        public void Should_Return_MimeType(string fileName, string expectedMimeType, AnyBitmap.ImageFormat expectedImageFormat)
        {
            string imagePath = GetRelativeFilePath(fileName);
            var bitmap = AnyBitmap.FromFile(imagePath);
            Assert.Equal(expectedMimeType, bitmap.MimeType);
            Assert.Equal(expectedImageFormat, bitmap.GetImageFormat());
        }

        [FactWithAutomaticDisplayName]
        public void Should_Return_Scan0()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var bitmap = AnyBitmap.FromFile(imagePath);
            Assert.NotEqual(IntPtr.Zero, bitmap.Scan0);
        }

        [IgnoreOnUnixFact]
        public void Should_Return_Stride()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            var bitmap = new System.Drawing.Bitmap(imagePath);
            BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            Assert.Equal(data.Stride, anyBitmap.Stride);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 0, 0)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 500, 0)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 0, 300)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 500, 100)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 599, 150)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 350, 450)]
        public void Should_Return_Pixel(string filename, int x, int y)
        {
            string imagePath = GetRelativeFilePath(filename);
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            var bitmap = Image.Load<Rgb24>(imagePath);

            _ = anyBitmap.Width.Should().Be(bitmap.Width);
            _ = anyBitmap.Height.Should().Be(bitmap.Height);

            Color anyBitmapPixel = anyBitmap.GetPixel(x, y);
            Rgb24 bitmapPixel = bitmap[x, y];
            _ = anyBitmapPixel.R.Should().Be(bitmapPixel.R);
            _ = anyBitmapPixel.G.Should().Be(bitmapPixel.G);
            _ = anyBitmapPixel.B.Should().Be(bitmapPixel.B);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 100, 100)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 1200, 800)]
        [InlineData("mountainclimbers.jpg", 700, 600)]
        [InlineData("mountainclimbers.jpg", 50, 30)]
        [InlineData("support-team-member-1.webp", 10, 10)]
        public void Should_Resize_Image(string fileName, int width, int height)
        {
            string imagePath = GetRelativeFilePath(fileName);
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            var resizeAnyBitmap = new AnyBitmap(anyBitmap, width, height);
            _ = resizeAnyBitmap.Width.Should().Be(width);
            _ = resizeAnyBitmap.Height.Should().Be(height);
        }

        [FactWithAutomaticDisplayName]
        public void Should_RotateFlip()
        {
            string imagePath = GetRelativeFilePath("checkmark.jpg");

            // Check rotate
            var bitmap = AnyBitmap.FromFile(imagePath);
            bitmap = AnyBitmap.RotateFlip(bitmap, AnyBitmap.RotateMode.Rotate180, AnyBitmap.FlipMode.Horizontal);
            bitmap.SaveAs("result_rotate.bmp");
            Assert.Equal(52, bitmap.Width);
            Assert.Equal(52, bitmap.Height);
            AssertImageAreEqual(GetRelativeFilePath("checkmark90.jpg"), "result_rotate.bmp");

            // Check flip
            bitmap = AnyBitmap.FromFile(imagePath);
            bitmap = AnyBitmap.RotateFlip(bitmap, AnyBitmap.RotateMode.None, AnyBitmap.FlipMode.Horizontal);
            bitmap.SaveAs("result_flip.bmp");
            Assert.Equal(52, bitmap.Width);
            Assert.Equal(52, bitmap.Height);
            AssertImageAreEqual(GetRelativeFilePath("checkmarkFlip.jpg"), "result_flip.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Redact_ShouldRedactRegionWithColor()
        {
            // Arrange
            using var memoryStream = new MemoryStream();
            using var image = new Image<Rgba32>(Configuration.Default, 100, 100, Color.White);
            image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
            {
                BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32,
                SupportTransparency = true
            });

            var anyBitmap = new AnyBitmap(memoryStream.ToArray());
            var rectangle = new Rectangle(10, 10, 50, 50);
            Color color = Color.Black;

            // Act
            var result = AnyBitmap.Redact(anyBitmap, rectangle, color);

            // Assert
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = result.GetPixel(x, y);
                    if (rectangle.Contains(x, y))
                    {
                        Assert.Equal(color, pixel);
                    }
                    else
                    {
                        Assert.Equal(Color.White, pixel);
                    }
                }
            }
        }

        [FactWithAutomaticDisplayName]
        public void TestGetRGBBuffer()
        {
            string imagePath = GetRelativeFilePath("checkmark.jpg");
            using var bitmap = new AnyBitmap(imagePath);
            var expectedSize = bitmap.Width * bitmap.Height * 3; // 3 bytes per pixel (RGB)

            byte[] buffer = bitmap.GetRGBBuffer();

            Assert.Equal(expectedSize, buffer.Length);

            // Verify the first pixel's RGB values
            var firstPixel = bitmap.GetPixel(0, 0);
            Assert.Equal(firstPixel.R, buffer[0]);
            Assert.Equal(firstPixel.G, buffer[1]);
            Assert.Equal(firstPixel.B, buffer[2]);
        }

        //[FactWithAutomaticDisplayName]
        public void TestGetRGBABuffer()
        {
            string imagePath = GetRelativeFilePath("checkmark.jpg");
            using var bitmap = new AnyBitmap(imagePath);
            var expectedSize = bitmap.Width * bitmap.Height * 4; // 4 bytes per pixel (RGB)

            byte[] buffer = bitmap.GetRGBABuffer();

            Assert.Equal(expectedSize, buffer.Length);

            // Verify the first pixel's RGB values
            var firstPixel = bitmap.GetPixel(0, 0);
            Assert.Equal(firstPixel.R, buffer[0]);
            Assert.Equal(firstPixel.G, buffer[1]);
            Assert.Equal(firstPixel.B, buffer[2]);
            Assert.Equal(firstPixel.A, buffer[3]);
        }

        [FactWithAutomaticDisplayName]
        public void Test_LoadFromRGBBuffer()
        {
            // Arrange
            int width = 2;
            int height = 2;
            byte[] buffer = new byte[]
            {
                255, 0, 0, // red
                0, 255, 0, // green
                0, 0, 255, // blue
                255, 255, 255, // white
            };

            // Act
            AnyBitmap result = AnyBitmap.LoadAnyBitmapFromRGBBuffer(buffer, width, height);

            // Assert
            byte[] resultData = result.GetRGBBuffer();
            Assert.Equal(buffer, resultData);
        }

        [FactWithAutomaticDisplayName]
        public void TestLoadAnyBitmapFromRGBBuffer()
        {
            string imagePath = GetRelativeFilePath("checkmark.jpg");

            using var bitmap = SixLabors.ImageSharp.Image.Load<Rgb24>(imagePath);
            var width = bitmap.Width;
            var height = bitmap.Height;

            var buffer = GetRGBBuffer(imagePath);

            AnyBitmap result = AnyBitmap.LoadAnyBitmapFromRGBBuffer(buffer, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var expectedColor = bitmap[x, y];
                    var actualColor = result.GetPixel(x, y);

                    Assert.Equal(expectedColor.R, actualColor.R);
                    Assert.Equal(expectedColor.G, actualColor.G);
                    Assert.Equal(expectedColor.B, actualColor.B);
                }
            }
        }

        [TheoryWithAutomaticDisplayName()]
        [InlineData("DW-26 Bitmap96Input.bmp", 96, 96)]
        [InlineData("DW-26 Bitmap300Input.bmp", 300, 300)]
        [InlineData("DW-26 Jpg300Input.jpg", 300, 300)]
        [InlineData("DW-26 Jpg72Input.jpg", 72, 72)]
        [InlineData("DW-26 Png300Input.png", 300, 300)]
        [InlineData("DW-26 Png96Input.png", 96,96)]
        [InlineData("DW-26 SinglePageTif72Input.tiff", 72, 72)]
        [InlineData("DW-26 SinglePageTif300Input.tif", 300, 300)]
        [InlineData("DW-26 MultiPageTif120Input.tiff", 120, 120)]
        [InlineData("DW-26 MultiPageTif200Input.tif", 200, 200)]
        public void AnyBitmapShouldReturnCorrectResolutions(string fileName, double expectedHorizontalResolution, double expectedVerticalResolution)
        {
            string imagePath = GetRelativeFilePath(fileName);
            var bitmap = AnyBitmap.FromFile(imagePath);
            var frames = bitmap.GetAllFrames;
            for (int i = 0; i < bitmap.FrameCount; i++)
            {
                Assert.Equal(expectedHorizontalResolution, frames.ElementAt(i).HorizontalResolution.Value, 1d);
                Assert.Equal(expectedVerticalResolution, frames.ElementAt(i).VerticalResolution.Value, 1d);
            }
        }

#if !NETFRAMEWORK
        [FactWithAutomaticDisplayName]
        public void CastMaui_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            byte[] bytes = File.ReadAllBytes(imagePath);
            var image = (Microsoft.Maui.Graphics.Platform.PlatformImage)Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(new MemoryStream(bytes));
            AnyBitmap anyBitmap = image;

            SaveMauiImages(image, "expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastMaui_from_AnyBitmap()
        {
            var anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            Microsoft.Maui.Graphics.Platform.PlatformImage image = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            SaveMauiImages(image, "result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }
#endif

        [FactWithAutomaticDisplayName]
        public void Should_Read_Tiff_With_Zero_Width_or_Height()
        {
            string imagePath = GetRelativeFilePath("partial_valid.tif");
            var anyBitmap = AnyBitmap.FromFile(imagePath);
            anyBitmap.FrameCount.Should().BeGreaterThan(0);
        }

        [FactWithAutomaticDisplayName]
        public void Create_New_Image_Instance()
        {
            string blankBitmapPath = "blank_bitmap.bmp";
            var bitmap = new AnyBitmap(8, 8);
            bitmap.SaveAs(blankBitmapPath);

            AnyBitmap blankBitmap = AnyBitmap.FromFile(blankBitmapPath);

            blankBitmap.Width.Should().Be(8);
            blankBitmap.Height.Should().Be(8);
        }

        [FactWithAutomaticDisplayName]
        public void Create_New_Image_With_Background_Instance()
        {
            string blankBitmapPath = "blank_bitmap.bmp";
            var bitmap = new AnyBitmap(8, 8, Color.DarkRed);
            bitmap.SaveAs(blankBitmapPath);

            AnyBitmap blankBitmap = AnyBitmap.FromFile(blankBitmapPath);

            blankBitmap.Width.Should().Be(8);
            blankBitmap.Height.Should().Be(8);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    blankBitmap.GetPixel(i, j).Should().Be(Color.DarkRed);
                }
            }
        }

        [FactWithAutomaticDisplayName]
        public void ExtractAlphaData_With32bppImage_ReturnsAlphaChannel()
        {
            // Arrange
            string imagePath = GetRelativeFilePath("32_bit_transparent.png");
            var bitmap = new AnyBitmap(imagePath);

            // Act
            var result = bitmap.ExtractAlphaData();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result[0]);
            Assert.Equal(5, result[49282]);
            Assert.Equal(108, result[49292]);
            Assert.Equal(211, result[49300]);
            Assert.Equal(0, result[47999]);
        }

        [FactWithAutomaticDisplayName]
        public void ExtractAlphaData_WithUnsupportedBppImage_ThrowsException()
        {
            // Arrange
            string imagePath = GetRelativeFilePath("24_bit.png");
            var bitmap = new AnyBitmap(imagePath);

            // Act & Assert
            var exception = Assert.Throws<NotSupportedException>(() => bitmap.ExtractAlphaData());
            Assert.Equal($"Extracting alpha data is not supported for {bitmap.BitsPerPixel} bpp images.", exception.Message);
        }

        [FactWithAutomaticDisplayName]
        public void LoadImage_TiffImage_ShouldLoadWithoutThumbnail()
        {
            // Arrange
            string imagePath = GetRelativeFilePath("example.tif");

            // Act
            var bitmap = new AnyBitmap(imagePath);

            // Assert
            bitmap.FrameCount.Should().Be(1);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("24_bit.png")]
        [InlineData("checkmark.jpg")]
        [InlineData("DW-26 Jpg72Input.jpg")]
        [InlineData("DW-26 Jpg300Input.jpg")]
        [InlineData("mountainclimbers.jpg")]
        public void LoadImage_SetPreserveOriginalFormat_ShouldReturnCorrectBitPerPixel(string imageFileName)
        {
            // Arrange
            string imagePath = GetRelativeFilePath(imageFileName);

            // Act
            var preserve = new AnyBitmap(imagePath, true);
            var notPreserve = new AnyBitmap(imagePath, false);

            // Assert
            Assert.Equal(24, preserve.BitsPerPixel);
            Assert.Equal(32, notPreserve.BitsPerPixel);
        }

#if !NET7_0
        [FactWithAutomaticDisplayName]
        public void CastAnyBitmap_from_SixLabors()
        {
            //This test throw System.OutOfMemoryException in x86

            var image = Image.Load(GetRelativeFilePath("RenderedFromChrome.bmp"));

            var anyBitmap = (AnyBitmap)image;

            image.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertLargeImageAreEqual("expected.bmp", "result.bmp", true);
        }
#endif


                [IgnoreOnAzureDevopsX86Fact]
                public void Load_TiffImage_ShouldNotIncreaseFileSize()
                {
                    // Arrange
        #if NET6_0_OR_GREATER
                    double thresholdPercent = 0.15;
        #else
                    double thresholdPercent = 1.5;
        #endif
                    string imagePath = GetRelativeFilePath("test_dw_10.tif");
                    string outputImagePath = "output.tif";

                    // Act
                    var bitmap = new AnyBitmap(imagePath);
                    bitmap.SaveAs(outputImagePath);
                    var originalFileSize = new FileInfo(imagePath).Length;
                    var maxAllowedFileSize = (long)(originalFileSize * (1 + thresholdPercent));
                    var outputFileSize = new FileInfo(outputImagePath).Length;

                    // Assert
                    outputFileSize.Should().BeLessThanOrEqualTo(maxAllowedFileSize);

                    // Clean up
                    File.Delete(outputImagePath);
                }

        [Theory]
        [InlineData("DW-26 MultiPageTif120Input.tiff")]
        [InlineData("google_large_1500dpi.bmp")]
        public void DW_34_ShouldNotThrowOutOfMemory(string filename)
        {
            string imagePath = GetRelativeFilePath(filename);

            List<AnyBitmap> images = new List<AnyBitmap>();
            for (int i = 0; i < 25; i++)
            {
                var bitmap = new AnyBitmap(imagePath);
                images.Add(bitmap);
                bitmap.IsImageLoaded().Should().BeFalse();
            }

            images.ForEach(bitmap => bitmap.Dispose());
        }

        //[Fact]
        //public void LoadTiff()
        //{
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    for (int i = 0; i < 25; i++)
        //    {
        //        var bitmap = new AnyBitmap("C:\\repo\\IronInternalBenchmarks\\IronOcrBenchmark\\Images\\001_20221121000002_S2123457_EL37.tiff");
        //        //var c = bitmap.GetPixel(10,10);
        //        foreach (var item in bitmap.GetAllFrames)
        //        {
        //            item.GetRGBBuffer();
        //            item.ExtractAlphaData();
        //        }


        //    }
        //    stopWatch.Stop();
        //    // Get the elapsed time as a TimeSpan value.
        //    TimeSpan ts = stopWatch.Elapsed;
        //    ts.Should().Be(TimeSpan.FromHours(1));
        //}

      //  [FactWithAutomaticDisplayName]
        public void AnyBitmap_ExportGif_Should_Works()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);

            using var resultExport = new MemoryStream();
            anyBitmap.ExportStream(resultExport, AnyBitmap.ImageFormat.Gif);
            resultExport.Length.Should().NotBe(0);
            Image.DetectFormat(resultExport.ToArray()).Should().Be(SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance);
        }

       // [FactWithAutomaticDisplayName]
        public void AnyBitmap_ExportTiff_Should_Works()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            var anyBitmap = AnyBitmap.FromFile(imagePath);

            using var resultExport = new MemoryStream();
            anyBitmap.ExportStream(resultExport, AnyBitmap.ImageFormat.Tiff);
            resultExport.Length.Should().NotBe(0);
            Image.DetectFormat(resultExport.ToArray()).Should().Be(SixLabors.ImageSharp.Formats.Tiff.TiffFormat.Instance);
        }

    }
}
