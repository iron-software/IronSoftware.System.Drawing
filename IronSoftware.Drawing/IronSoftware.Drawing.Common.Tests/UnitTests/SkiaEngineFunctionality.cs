#if NET48
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    /// <summary>
    /// .NET Framework runs on the SkiaSharp engine (no ImageSharp). These tests cover the behaviour that the
    /// ImageSharp-cast tests check on other targets, using only the public AnyBitmap API.
    /// </summary>
    public class SkiaEngineFunctionality : Compare
    {
        public SkiaEngineFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        private static AnyBitmap MakeBitmap(int width, int height)
        {
            var bitmap = new AnyBitmap(width, height, Color.White);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(255, x * 255 / width, y * 255 / height, 128));
                }
            }

            return bitmap;
        }

        [FactWithAutomaticDisplayName]
        public void SetPixel_GetPixel_RoundTrip()
        {
            using var bitmap = AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"));
            bitmap.GetPixel(0, 0).Should().NotBe(Color.Black);

            bitmap.SetPixel(0, 0, Color.Black);
            bitmap.SetPixel(5, 7, Color.FromArgb(255, 10, 20, 30));

            bitmap.GetPixel(0, 0).Should().Be(Color.Black);
            bitmap.GetPixel(5, 7).Should().Be(Color.FromArgb(255, 10, 20, 30));
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData(AnyBitmap.ImageFormat.Png)]
        [InlineData(AnyBitmap.ImageFormat.Bmp)]
        [InlineData(AnyBitmap.ImageFormat.Tiff)]
        public void LosslessFormats_RoundTripPixels(AnyBitmap.ImageFormat format)
        {
            using var source = MakeBitmap(40, 30);
            using var decoded = AnyBitmap.FromBytes(source.ExportBytes(format));

            decoded.Width.Should().Be(40);
            decoded.Height.Should().Be(30);
            foreach (var (x, y) in new[] { (0, 0), (39, 0), (20, 15), (0, 29), (39, 29) })
            {
                decoded.GetPixel(x, y).Should().Be(source.GetPixel(x, y), $"{format} pixel ({x},{y})");
            }
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData(AnyBitmap.ImageFormat.Jpeg, "image/jpeg")]
        [InlineData(AnyBitmap.ImageFormat.Webp, "image/webp")]
        [InlineData(AnyBitmap.ImageFormat.Gif, "image/gif")]
        public void LossyFormats_RoundTripCloseToSource(AnyBitmap.ImageFormat format, string mimeType)
        {
            using var source = new AnyBitmap(32, 32, Color.FromArgb(255, 200, 40, 90));
            using var decoded = AnyBitmap.FromBytes(source.ExportBytes(format, 90));

            decoded.MimeType.Should().Be(mimeType);
            Color pixel = decoded.GetPixel(16, 16);
            ((int)pixel.R).Should().BeCloseTo(200, 6);
            ((int)pixel.G).Should().BeCloseTo(40, 6);
            ((int)pixel.B).Should().BeCloseTo(90, 6);
        }

        [FactWithAutomaticDisplayName]
        public void Png_PreservesTransparency()
        {
            using var source = new AnyBitmap(10, 10, Color.Transparent);
            source.SetPixel(3, 3, Color.FromArgb(128, 255, 0, 0));

            using var decoded = AnyBitmap.FromBytes(source.ExportBytes(AnyBitmap.ImageFormat.Png));

            decoded.GetPixel(0, 0).A.Should().Be(0);
            decoded.GetPixel(3, 3).Should().Be(Color.FromArgb(128, 255, 0, 0));
        }

        [FactWithAutomaticDisplayName]
        public void Clone_Rectangle_CropsPixels()
        {
            using var source = MakeBitmap(50, 40);
            using var cropped = source.Clone(new Rectangle(10, 5, 20, 15));

            cropped.Width.Should().Be(20);
            cropped.Height.Should().Be(15);
            cropped.GetPixel(0, 0).Should().Be(source.GetPixel(10, 5));
            cropped.GetPixel(19, 14).Should().Be(source.GetPixel(29, 19));
        }

        [FactWithAutomaticDisplayName]
        public void Redact_FillsOnlyTheRegion()
        {
            using var source = new AnyBitmap(100, 100, Color.White);
            using var redacted = source.Redact(new Rectangle(10, 10, 30, 20), Color.Red);

            redacted.GetPixel(10, 10).Should().Be(Color.Red);
            redacted.GetPixel(39, 29).Should().Be(Color.Red);
            redacted.GetPixel(9, 10).Should().Be(Color.White);
            redacted.GetPixel(40, 30).Should().Be(Color.White);
        }

        [FactWithAutomaticDisplayName]
        public void RotateFlip_MovesPixels()
        {
            using var source = MakeBitmap(20, 10);
            using var rotated = source.RotateFlip(AnyBitmap.RotateMode.Rotate90, AnyBitmap.FlipMode.None);

            rotated.Width.Should().Be(10);
            rotated.Height.Should().Be(20);
            // Rotating 90 degrees clockwise moves the top-left pixel to the top-right corner.
            rotated.GetPixel(9, 0).Should().Be(source.GetPixel(0, 0));
        }

        [FactWithAutomaticDisplayName]
        public void Resize_ChangesDimensions()
        {
            using var source = AnyBitmap.FromFile(GetRelativeFilePath("mountainclimbers.jpg"));
            using var resized = new AnyBitmap(source, 64, 48);

            resized.Width.Should().Be(64);
            resized.Height.Should().Be(48);
        }

        [FactWithAutomaticDisplayName]
        public void MultiFrameGif_KeepsEveryFrame()
        {
            var frames = new List<AnyBitmap>
            {
                new AnyBitmap(30, 30, Color.Red),
                new AnyBitmap(30, 30, Color.Lime),
                new AnyBitmap(30, 30, Color.Blue)
            };

            using var gif = AnyBitmap.CreateMultiFrameGif(frames);
            using var decoded = AnyBitmap.FromBytes(gif.ExportBytes(AnyBitmap.ImageFormat.Gif));

            decoded.FrameCount.Should().Be(3);
            var decodedFrames = decoded.GetAllFrames.ToList();
            decodedFrames[0].GetPixel(15, 15).Should().Be(Color.Red);
            decodedFrames[1].GetPixel(15, 15).Should().Be(Color.Lime);
            decodedFrames[2].GetPixel(15, 15).Should().Be(Color.Blue);
        }

        [FactWithAutomaticDisplayName]
        public void MultiFrameTiff_KeepsEveryPage()
        {
            var pages = new List<AnyBitmap> { MakeBitmap(40, 30), MakeBitmap(40, 30) };
            pages[1].SetPixel(0, 0, Color.Black);

            using var tiff = AnyBitmap.CreateMultiFrameTiff(pages);
            using var decoded = AnyBitmap.FromBytes(tiff.ExportBytes(AnyBitmap.ImageFormat.Tiff));

            decoded.FrameCount.Should().Be(2);
            var decodedPages = decoded.GetAllFrames.ToList();
            decodedPages[0].GetPixel(0, 0).Should().Be(pages[0].GetPixel(0, 0));
            decodedPages[1].GetPixel(0, 0).Should().Be(Color.Black);
        }

        [TheoryWithAutomaticDisplayName]
        [InlineData("24_bit.png", 24)]
        [InlineData("mountainclimbers.jpg", 24)]
        [InlineData("32_bit_transparent.png", 32)]
        public void BitsPerPixel_FollowsSource(string fileName, int expected)
        {
            using var bitmap = AnyBitmap.FromFile(GetRelativeFilePath(fileName), true);
            bitmap.BitsPerPixel.Should().Be(expected);
        }
    }
}
#endif
