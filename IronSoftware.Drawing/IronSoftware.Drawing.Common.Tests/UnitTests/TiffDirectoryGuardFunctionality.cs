using BitMiracle.LibTiff.Classic;
using FluentAssertions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IronSoftware.Drawing.Common.Tests.UnitTests
{
    /// <summary>
    /// CVE-2026-106116 (GHSA-wmxv-xphr-5c9g): a BigTIFF whose IFD entry count exceeds the file size keeps
    /// the ImageSharp 2.x TIFF decoder in a non-progressing loop. ImageSharp 2.x (the netstandard2.0 build,
    /// used by .NET Framework) will not be patched, so AnyBitmap must reject these files before decoding.
    /// The net8.0 build runs the same tests against the patched ImageSharp 4.x.
    /// </summary>
    public class TiffDirectoryGuardFunctionality : TestsBase
    {
        private static readonly TimeSpan FailFastLimit = TimeSpan.FromSeconds(5);

        public TiffDirectoryGuardFunctionality(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// 104-byte little-endian BigTIFF: the first IFD declares 0x0FFFFFFFFFFFFFFF entries.
        /// </summary>
        private static byte[] CreateBigTiffWithOversizedIfdCount()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write((byte)'I');
            writer.Write((byte)'I');
            writer.Write((ushort)43);   // BigTIFF version
            writer.Write((ushort)8);    // offset byte size
            writer.Write((ushort)0);
            writer.Write(16UL);         // first IFD offset
            writer.Write(0x0FFF_FFFF_FFFF_FFFFUL);
            writer.Write(new byte[64]);
            writer.Flush();
            return ms.ToArray();
        }

        private static void ShouldFailFast(Action action)
        {
            Task<Exception> task = Task.Run(() =>
            {
                try
                {
                    action();
                    return null;
                }
                catch (Exception e)
                {
                    return e;
                }
            });

            task.Wait(FailFastLimit).Should().BeTrue("a malformed BigTIFF must not hang the decoder");
            task.Result.Should().NotBeNull("a malformed BigTIFF must be rejected");
        }

        [FactWithAutomaticDisplayName]
        public void FromBytes_BigTiffWithOversizedIfdCount_FailsFast()
        {
            byte[] bytes = CreateBigTiffWithOversizedIfdCount();
            ShouldFailFast(() => _ = AnyBitmap.FromBytes(bytes).Width);
        }

        [FactWithAutomaticDisplayName]
        public void Constructor_BigTiffWithOversizedIfdCount_FailsFast()
        {
            byte[] bytes = CreateBigTiffWithOversizedIfdCount();
            ShouldFailFast(() => _ = new AnyBitmap(bytes).Width);
        }

        [FactWithAutomaticDisplayName]
        public void FromStream_BigTiffWithOversizedIfdCount_FailsFast()
        {
            byte[] bytes = CreateBigTiffWithOversizedIfdCount();
            ShouldFailFast(() =>
            {
                using var stream = new MemoryStream(bytes);
                _ = AnyBitmap.FromStream(stream).Width;
            });
        }

        [FactWithAutomaticDisplayName]
        public void FromFile_BigTiffWithOversizedIfdCount_FailsFast()
        {
            string path = Path.Combine(Path.GetTempPath(), $"cve-2026-106116-{Guid.NewGuid():N}.tif");
            File.WriteAllBytes(path, CreateBigTiffWithOversizedIfdCount());
            try
            {
                ShouldFailFast(() => _ = AnyBitmap.FromFile(path).Width);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [FactWithAutomaticDisplayName]
        public void RotateFlip_BigTiffWithOversizedIfdCount_FailsFast()
        {
            byte[] bytes = CreateBigTiffWithOversizedIfdCount();
            ShouldFailFast(() => AnyBitmap.RotateFlip(AnyBitmap.FromBytes(bytes), AnyBitmap.RotateMode.Rotate90, AnyBitmap.FlipMode.None));
        }

        [FactWithAutomaticDisplayName]
        public void Redact_BigTiffWithOversizedIfdCount_FailsFast()
        {
            byte[] bytes = CreateBigTiffWithOversizedIfdCount();
            ShouldFailFast(() => AnyBitmap.Redact(AnyBitmap.FromBytes(bytes), new Rectangle(0, 0, 1, 1), Color.Black));
        }

        [FactWithAutomaticDisplayName]
        public void CreateMultiFrameTiff_BigTiffWithOversizedIfdCount_FailsFast()
        {
            string path = Path.Combine(Path.GetTempPath(), $"cve-2026-106116-{Guid.NewGuid():N}.tif");
            File.WriteAllBytes(path, CreateBigTiffWithOversizedIfdCount());
            try
            {
                ShouldFailFast(() => AnyBitmap.CreateMultiFrameTiff(new[] { path }));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [FactWithAutomaticDisplayName]
        public void ImageSharpLoader_BigTiffWithOversizedIfdCount_FailsFast()
        {
            byte[] bytes = CreateBigTiffWithOversizedIfdCount();
            ShouldFailFast(() => ImageSharpLoader.Load(bytes));
            ShouldFailFast(() => ImageSharpLoader.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bytes));
            ShouldFailFast(() => ImageSharpLoader.Load(new MemoryStream(bytes)));
            ShouldFailFast(() => ImageSharpLoader.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(new MemoryStream(bytes)));
        }

        [FactWithAutomaticDisplayName]
        public void ImageSharpLoader_ValidImage_MatchesImageLoad()
        {
            byte[] bytes = File.ReadAllBytes(GetRelativeFilePath("van-gogh-starry-night-vincent-van-gogh.jpg"));

            using var expected = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bytes);
            using var fromBytes = ImageSharpLoader.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bytes);
            using var fromStream = ImageSharpLoader.Load(new MemoryStream(bytes));

            fromBytes.Width.Should().Be(expected.Width);
            fromBytes.Height.Should().Be(expected.Height);
            fromBytes[10, 10].Should().Be(expected[10, 10]);
            fromStream.Width.Should().Be(expected.Width);
            fromStream.Height.Should().Be(expected.Height);
        }

        [FactWithAutomaticDisplayName]
        public void FromBytes_ValidBigTiff_StillLoads()
        {
            const int width = 7;
            const int height = 5;
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                // "w8" makes LibTiff write a BigTIFF.
                using (Tiff tiff = Tiff.ClientOpen("in-memory", "w8", ms, new TiffStream()))
                {
                    tiff.SetField(TiffTag.IMAGEWIDTH, width);
                    tiff.SetField(TiffTag.IMAGELENGTH, height);
                    tiff.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                    tiff.SetField(TiffTag.BITSPERSAMPLE, 8);
                    tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                    tiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    tiff.SetField(TiffTag.ROWSPERSTRIP, height);
                    tiff.SetField(TiffTag.COMPRESSION, Compression.NONE);

                    byte[] row = new byte[width * 3];
                    for (int y = 0; y < height; y++)
                    {
                        tiff.WriteScanline(row, y);
                    }

                    tiff.WriteDirectory();
                }

                bytes = ms.ToArray();
            }

            bytes[2].Should().Be(43, "the fixture must be a BigTIFF");

            var bitmap = AnyBitmap.FromBytes(bytes);
            bitmap.Width.Should().Be(width);
            bitmap.Height.Should().Be(height);

            var rotated = AnyBitmap.RotateFlip(bitmap, AnyBitmap.RotateMode.Rotate90, AnyBitmap.FlipMode.None);
            rotated.Width.Should().Be(height);
            rotated.Height.Should().Be(width);
        }
    }
}
