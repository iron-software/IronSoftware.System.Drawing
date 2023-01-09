using FluentAssertions;
using Microsoft.Maui.Graphics;
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
using Rectangle = SixLabors.ImageSharp.Rectangle;

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
            Stream ms = new MemoryStream(bytes);

            AnyBitmap bitmap = AnyBitmap.FromStream(ms);
            bitmap.TrySaveAs("result.bmp");
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
            MemoryStream ms = new MemoryStream(bytes);

            AnyBitmap bitmap = AnyBitmap.FromStream(ms);
            bitmap.TrySaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");

            bitmap = new AnyBitmap(ms);
            bitmap.SaveAs("result.bmp");
            AssertImageAreEqual(imagePath, "result.bmp");
        }

        [FactWithAutomaticDisplayName]
        public void Create_AnyBitmap_by_Uri()
        {
            Uri uri = new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ea/Van_Gogh_-_Starry_Night_-_Google_Art_Project.jpg/1200px-Van_Gogh_-_Starry_Night_-_Google_Art_Project.jpg");

            AnyBitmap bitmap = AnyBitmap.FromUri(uri);
            bitmap.TrySaveAs("result.bmp");
            AssertImageExist("result.bmp", true);

            bitmap = new AnyBitmap(uri);
            bitmap.TrySaveAs("result.bmp");
            AssertImageExist("result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void Create_SVG_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("Example_barcode.svg");
            AnyBitmap bitmap = AnyBitmap.FromFile(imagePath);
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
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath);
            AnyBitmap anyBitmap = bitmap;

            bitmap.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }
        [FactWithAutomaticDisplayName]
        public void CastBitmap_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            System.Drawing.Bitmap bitmap = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastImage_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            System.Drawing.Image bitmap = System.Drawing.Image.FromFile(imagePath);
            AnyBitmap anyBitmap = bitmap;

            bitmap.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastImage_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            System.Drawing.Image bitmap = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            bitmap.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
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

            Func<Stream> funcStream = anyBitmap.ToStreamFn();
            AssertStreamAreEqual(expected, funcStream);

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


            using Image image = anyBitmap;
            image.Mutate(img => img.Crop(new Rectangle(0, 0, 100, 100)));
            AnyBitmap clonedWithRect = anyBitmap.Clone(new CropRectangle(0, 0, 100, 100));

            image.SaveAsPng("expected.png");
            clonedWithRect.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_to_AnyBitmap()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            SkiaSharp.SKBitmap skBitmap = SkiaSharp.SKBitmap.Decode(imagePath);
            AnyBitmap anyBitmap = skBitmap;

            SaveSkiaBitmap(skBitmap, "expected.png");
            anyBitmap.SaveAs("result.png");

            AssertImageAreEqual("expected.png", "result.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSKBitmap_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
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
            SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> imgSharp = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imagePath);
            AnyBitmap anyBitmap = imgSharp;

            imgSharp.Save("expected.bmp");
            anyBitmap.SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }
        [FactWithAutomaticDisplayName]
        public void CastSixLaborsList_to_AnyBitmapList()
        {
            string imagePath = GetRelativeFilePath("mountainclimbers.jpg");
            SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> imgSharp = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imagePath);
            List<SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>> imgSharpList = new List<Image<Rgba32>>()
                {imgSharp};
            // TODO 1/9/23: support this casting
            List<AnyBitmap> anyBitmapList = imgSharpList.Cast<AnyBitmap>().ToList();
            // should we also support this?
            //List<AnyBitmap> anyBitmapList = imgSharpList;

            imgSharp.Save("expected.bmp");
            anyBitmapList.First().SaveAs("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void CastSixLabors_from_AnyBitmap()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"));
            SixLabors.ImageSharp.Image imgSharp = anyBitmap;

            anyBitmap.SaveAs("expected.bmp");
            imgSharp.Save("result.bmp");

            AssertImageAreEqual("expected.bmp", "result.bmp", true);
        }

        [FactWithAutomaticDisplayName]
        public void Load_Tiff_Image()
        {
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("IRON-274-39065.tif"));
            Assert.Equal(2, anyBitmap.FrameCount);

            AnyBitmap multiPage = AnyBitmap.FromFile(GetRelativeFilePath("animated_qr.gif"));
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
            AnyBitmap anyBitmap = AnyBitmap.FromFile(GetRelativeFilePath("multiframe.tiff"));
            Assert.Equal(2, anyBitmap.FrameCount);
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Tiff()
        {
            List<AnyBitmap> bitmaps = new List<AnyBitmap>()
            {
                AnyBitmap.FromFile(GetRelativeFilePath("first-animated-qr.png")),
                AnyBitmap.FromFile(GetRelativeFilePath("last-animated-qr.png"))
            };

            AnyBitmap anyBitmap = AnyBitmap.CreateMultiFrameTiff(bitmaps);
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
            List<string> imagePaths = new List<string>()
            {
                GetRelativeFilePath("first-animated-qr.png"),
                GetRelativeFilePath("last-animated-qr.png")
            };

            AnyBitmap anyBitmap = AnyBitmap.CreateMultiFrameTiff(imagePaths);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            AssertImageAreEqual(GetRelativeFilePath("first-animated-qr.png"), "first.png");
            AssertImageAreEqual(GetRelativeFilePath("last-animated-qr.png"), "last.png");
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Gif()
        {
            List<AnyBitmap> bitmaps = new List<AnyBitmap>()
            {
                AnyBitmap.FromFile(GetRelativeFilePath("first-animated-qr.png")),
                AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"))
            };

            AnyBitmap anyBitmap = AnyBitmap.CreateMultiFrameGif(bitmaps);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            Image first = Image.Load(GetRelativeFilePath("first-animated-qr.png"));
            first.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(anyBitmap.GetAllFrames.ElementAt(0).Width, anyBitmap.GetAllFrames.ElementAt(0).Height),
                Mode = SixLabors.ImageSharp.Processing.ResizeMode.BoxPad
            }));
            first.Save("first-expected.jpg");
            AssertImageAreEqual("first-expected.jpg", "first.png", true);

            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            Image last = Image.Load(GetRelativeFilePath("mountainclimbers.jpg"));
            last.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(anyBitmap.GetAllFrames.ElementAt(1).Width, anyBitmap.GetAllFrames.ElementAt(1).Height),
                Mode = SixLabors.ImageSharp.Processing.ResizeMode.BoxPad
            }));
            last.Save("last-expected.jpg");
            AssertImageAreEqual("last-expected.jpg", "last.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void Create_Multi_page_Gif_paths()
        {
            List<string> imagePaths = new List<string>()
            {
                GetRelativeFilePath("first-animated-qr.png"),
                GetRelativeFilePath("mountainclimbers.jpg")
            };

            AnyBitmap anyBitmap = AnyBitmap.CreateMultiFrameGif(imagePaths);
            Assert.Equal(2, anyBitmap.FrameCount);
            Assert.Equal(2, anyBitmap.GetAllFrames.Count());
            anyBitmap.GetAllFrames.ElementAt(0).SaveAs("first.png");
            Image first = Image.Load(GetRelativeFilePath("first-animated-qr.png"));
            first.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(anyBitmap.GetAllFrames.ElementAt(0).Width, anyBitmap.GetAllFrames.ElementAt(0).Height),
                Mode = SixLabors.ImageSharp.Processing.ResizeMode.BoxPad
            }));
            first.Save("first-expected.jpg");
            AssertImageAreEqual("first-expected.jpg", "first.png", true);

            anyBitmap.GetAllFrames.ElementAt(1).SaveAs("last.png");
            Image last = Image.Load(GetRelativeFilePath("mountainclimbers.jpg"));
            last.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(anyBitmap.GetAllFrames.ElementAt(1).Width, anyBitmap.GetAllFrames.ElementAt(1).Height),
                Mode = SixLabors.ImageSharp.Processing.ResizeMode.BoxPad
            }));
            last.Save("last-expected.jpg");
            AssertImageAreEqual("last-expected.jpg", "last.png", true);
        }

        [FactWithAutomaticDisplayName]
        public void Should_Return_BitsPerPixel()
        {
            AnyBitmap bitmap = AnyBitmap.FromFile(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));
            Assert.Equal(24, bitmap.BitsPerPixel);

            bitmap = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(GetRelativeFilePath("mountainclimbers.jpg"));
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
            AnyBitmap bitmap = AnyBitmap.FromFile(imagePath);
            Assert.Equal(expectedMimeType, bitmap.MimeType);
            Assert.Equal(expectedImageFormat, bitmap.GetImageFormat());
        }

        [FactWithAutomaticDisplayName]
        public void Should_Return_Scan0()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap bitmap = AnyBitmap.FromFile(imagePath);
            Assert.NotEqual(IntPtr.Zero, bitmap.Scan0);
        }

        [FactWithAutomaticDisplayName]
        public void Should_Return_Stride()
        {
            string imagePath = GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg");
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath);
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
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            SixLabors.ImageSharp.Image<Rgb24> bitmap = SixLabors.ImageSharp.Image.Load<Rgb24>(imagePath);

            anyBitmap.Width.Should().Be(bitmap.Width);
            anyBitmap.Height.Should().Be(bitmap.Height);

            Color anyBitmapPixel = anyBitmap.GetPixel(x, y);
            SixLabors.ImageSharp.PixelFormats.Rgb24 bitmapPixel = bitmap[x, y];
            anyBitmapPixel.R.Should().Be(bitmapPixel.R);
            anyBitmapPixel.G.Should().Be(bitmapPixel.G);
            anyBitmapPixel.B.Should().Be(bitmapPixel.B);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 100, 100)]
        [InlineData("van-gogh-starry-night-vincent-van-gogh.jpg", 1200, 800)]
        [InlineData("mountainclimbers.jpg", 700, 600)]
        [InlineData("mountainclimbers.jpg", 50, 30)]
        public void Should_Resize_Image(string fileName, int width, int height)
        {
            string imagePath = GetRelativeFilePath(fileName);
            AnyBitmap anyBitmap = AnyBitmap.FromFile(imagePath);
            AnyBitmap resizeAnyBitmap = new AnyBitmap(anyBitmap, width, height);
            resizeAnyBitmap.Width.Should().Be(width);
            resizeAnyBitmap.Height.Should().Be(height);
        }

#if !NETFRAMEWORK
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
