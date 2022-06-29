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

        protected void AssertImageAreEqual(string expectedImagePath, string resultImagePath, bool isCleanAll = false)
        {
            string assertName = "AssertImage.AreEqual";

            AnyBitmap expected = AnyBitmap.FromFile(expectedImagePath);
            AnyBitmap actual = AnyBitmap.FromFile(resultImagePath);

            if (isCleanAll)
                CleanResultFile(expectedImagePath);
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
                    throw new AssertActualExpectedException($"Expected:<hash value {hash1[i]}>.", $"Actual:<hash value {hash2[i]}>.", $"{assertName} failed.");
            }
        }

        protected void CleanResultFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
        protected void AssertStreamAreEqual(MemoryStream expected, MemoryStream actual)
        {
            string assertName = "AssertStream.AreEqual";

            if (expected.Length != actual.Length)
                throw new AssertActualExpectedException($"Expected:<Length {expected.Length}>.", $"Actual:<Length {actual.Length}>.", $"{assertName} failed.");
            expected.Position = 0;
            actual.Position = 0;

            var msArray1 = expected.ToArray();
            var msArray2 = actual.ToArray();

            if (!msArray1.SequenceEqual(msArray2))
                throw new AssertActualExpectedException($"Expected: {expected}", $"Actual: {actual}", $"Actual Stream sequence not equal to Expected.");
        }

        protected void AssertBytesAreEqual(byte[] expected, byte[] actual)
        {
            string assertName = "AssertBytes.AreEqual";

            if (!expected.SequenceEqual(actual))
                throw new AssertActualExpectedException($"Expected: {expected}", $"Actual: {actual}", $"{assertName} failed.");
        }

        protected void SaveSkiaImage(SkiaSharp.SKBitmap bitmap, string filename, AnyBitmap.ImageFormat imageFormat = AnyBitmap.ImageFormat.Png)
        {
            SkiaSharp.SKImage image = SkiaSharp.SKImage.FromBitmap(bitmap);
            SaveSkiaImage(image, filename, imageFormat);
        }

        protected void SaveSkiaImage(SkiaSharp.SKImage image, string filename, AnyBitmap.ImageFormat imageFormat = AnyBitmap.ImageFormat.Png)
        {
            using Stream stream = System.IO.File.OpenWrite(filename);
            {
                SkiaSharp.SKData data = image.Encode((SkiaSharp.SKEncodedImageFormat)((int)imageFormat), 100);
                data.SaveTo(stream);
            }
        }

#if NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        protected void SaveMauiImages(Microsoft.Maui.Graphics.IImage image, string filename)
        {
            using MemoryStream memStream = new MemoryStream();
            image.Save(memStream);
            using var fileStream = System.IO.File.Create(filename);
            memStream.Seek(0, SeekOrigin.Begin);
            memStream.CopyTo(fileStream);
        }
#endif
    }
}
