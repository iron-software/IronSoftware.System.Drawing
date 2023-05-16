using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    public abstract class Compare : TestsBase
    {
        protected Compare(ITestOutputHelper output) : base(output)
        {
        }

        protected static void AssertImageAreEqual(string expectedImagePath, string resultImagePath, bool isCleanAll = false)
        {
            string assertName = "AssertImage.AreEqual";

            var expected = AnyBitmap.FromFile(expectedImagePath);
            var actual = AnyBitmap.FromFile(resultImagePath);

            if (isCleanAll)
            {
                CleanResultFile(expectedImagePath);
            }

            CleanResultFile(resultImagePath);

            //Test to see if we have the same size of image
            if (expected.Width != actual.Width || expected.Height != actual.Height)
            {
                throw new AssertActualExpectedException($"Expected:<Height {expected.Height}, Width {expected.Width}>.", $"Actual:<Height {actual.Height},Width {actual.Width}>.", $"{assertName} failed.");
            }

            //Convert each image to a byte array
            byte[] btImageExpected = expected.ExportBytes();
            byte[] btImageActual = expected.ExportBytes();

            //Compute a hash for each image
            var shaM = SHA256.Create();
            byte[] hash1 = shaM.ComputeHash(btImageExpected);
            byte[] hash2 = shaM.ComputeHash(btImageActual);

            //Compare the hash values
            for (int i = 0; i < hash1.Length && i < hash2.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    throw new AssertActualExpectedException($"Expected:<hash value {hash1[i]}>.", $"Actual:<hash value {hash2[i]}>.", $"{assertName} failed.");
                }
            }
        }

        protected static void CleanResultFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
        protected static void AssertStreamAreEqual(MemoryStream expected, MemoryStream actual)
        {
            string assertName = "AssertStream.AreEqual";

            if (expected.Length != actual.Length)
            {
                throw new AssertActualExpectedException($"Expected:<Length {expected.Length}>.", $"Actual:<Length {actual.Length}>.", $"{assertName} failed.");
            }

            expected.Position = 0;
            actual.Position = 0;

            byte[] msArray1 = expected.ToArray();
            byte[] msArray2 = actual.ToArray();

            if (!msArray1.SequenceEqual(msArray2))
            {
                throw new AssertActualExpectedException($"Expected: {expected}", $"Actual: {actual}", $"Actual Stream sequence not equal to Expected.");
            }
        }
        protected static void AssertStreamAreEqual(MemoryStream expected, Func<Stream> actual)
        {
            using (var actualStream = (MemoryStream)actual.Invoke())
            {
                AssertStreamAreEqual(expected, actualStream);
            }
        }

        protected static void AssertBytesAreEqual(byte[] expected, byte[] actual)
        {
            string assertName = "AssertBytes.AreEqual";

            if (!expected.SequenceEqual(actual))
            {
                throw new AssertActualExpectedException($"Expected: {expected}", $"Actual: {actual}", $"{assertName} failed.");
            }
        }

        protected static void AssertImageExist(string resultImagePath, bool isCleanAll = false)
        {
            string assertName = "AssertFile.Exist";
            if (File.Exists(resultImagePath))
            {
                if (isCleanAll)
                {
                    CleanResultFile(resultImagePath);
                }
            }
            else
            {
                throw new AssertActualExpectedException($"Expected: File should exist in {resultImagePath}.", $"Actual: File does not exist in {resultImagePath}.", $"{assertName} failed.");
            }
        }

        protected static void SaveSkiaBitmap(SkiaSharp.SKBitmap bitmap, string filename, AnyBitmap.ImageFormat imageFormat = AnyBitmap.ImageFormat.Png)
        {
            var image = SkiaSharp.SKImage.FromBitmap(bitmap);
            SaveSkiaImage(image, filename, imageFormat);
        }

        protected static void SaveSkiaImage(SkiaSharp.SKImage image, string filename, AnyBitmap.ImageFormat imageFormat = AnyBitmap.ImageFormat.Png)
        {
            using Stream stream = File.OpenWrite(filename);
            {
                SkiaSharp.SKData data = image.Encode((SkiaSharp.SKEncodedImageFormat)(int)imageFormat, 100);
                data.SaveTo(stream);
            }
        }

        protected static void SaveMauiImages(Microsoft.Maui.Graphics.IImage image, string filename)
        {
            using var memStream = new MemoryStream();
            image.Save(memStream);
            using FileStream fileStream = File.Create(filename);
            _ = memStream.Seek(0, SeekOrigin.Begin);
            memStream.CopyTo(fileStream);
        }

        protected static bool IsGrayScale(SkiaSharp.SKBitmap image)
        {
            bool res = true;
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    SkiaSharp.SKColor color = image.GetPixel(i, j);

                    if (color.Alpha != 0 && (color.Red != color.Green || color.Green != color.Blue))
                    {
                        res = false;
                        break;
                    }
                }
            }

            return res;
        }

        protected byte[] GetRGBBuffer(string imagePath)
        {
            using var image = SixLabors.ImageSharp.Image.Load<Rgb24>(imagePath); // Load image from file

            int width = image.Width;
            int height = image.Height;

            byte[] rgbBuffer = new byte[width * height * 3]; // 3 bytes per pixel (RGB)

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelRow = accessor.GetRowSpan(y);

                    for (int x = 0; x < accessor.Width; x++)
                    {
                        ref Rgb24 pixel = ref pixelRow[x];

                        int bufferIndex = (y * width + x) * 3;
                        rgbBuffer[bufferIndex] = pixel.R;
                        rgbBuffer[bufferIndex + 1] = pixel.G;
                        rgbBuffer[bufferIndex + 2] = pixel.B;
                    }
                }
            });

            return rgbBuffer;
        }
    }
}
