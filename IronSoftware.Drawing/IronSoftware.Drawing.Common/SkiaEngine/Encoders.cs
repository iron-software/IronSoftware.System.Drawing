#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace IronSoftware.Drawing.SkiaEngine
{
    internal interface IImageEncoder
    {
        void Encode(Image image, Stream stream);
    }

    internal enum JpegColorType
    {
        YCbCrRatio420 = 0,
        YCbCrRatio444 = 1,
        Luminance = 2,
        Rgb = 3,
    }

    internal enum BmpBitsPerPixel
    {
        Pixel24 = 24,
        Pixel32 = 32,
    }

    // Values match the TIFF specification (and ImageSharp's TiffCompression).
    internal enum TiffCompression
    {
        Invalid = 0,
        None = 1,
        Ccitt1D = 2,
        CcittGroup3Fax = 3,
        CcittGroup4Fax = 4,
        Lzw = 5,
        OldJpeg = 6,
        Jpeg = 7,
        Deflate = 8,
        ItuTRecT82 = 9,
        ItuTRecT43 = 10,
        NeXT = 32766,
        PackBits = 32773,
        ThunderScan = 32809,
        OldDeflate = 32946,
    }

    internal static class ResolutionConverter
    {
        public static double ToPixelsPerMeter(double value, PixelResolutionUnit unit) => unit switch
        {
            PixelResolutionUnit.PixelsPerInch => value / 0.0254,
            PixelResolutionUnit.PixelsPerCentimeter => value * 100,
            _ => value,
        };

        public static double ToPixelsPerInch(double value, PixelResolutionUnit unit) => unit switch
        {
            PixelResolutionUnit.PixelsPerCentimeter => value * 2.54,
            PixelResolutionUnit.PixelsPerMeter => value * 0.0254,
            _ => value,
        };
    }

    internal sealed class JpegEncoder : IImageEncoder
    {
        public int? Quality { get; set; }
        public JpegColorType? ColorType { get; set; }

        public void Encode(Image image, Stream stream)
        {
            // ImageSharp's JpegColorType.Rgb / YCbCrRatio444 encode without chroma subsampling.
            bool fullChroma = ColorType == JpegColorType.Rgb || ColorType == JpegColorType.YCbCrRatio444;
            var options = new SKJpegEncoderOptions(
                Math.Max(1, Math.Min(100, Quality ?? 75)),
                fullChroma ? SKJpegEncoderDownsample.Downsample444 : SKJpegEncoderDownsample.Downsample420,
                SKJpegEncoderAlphaOption.Ignore);

            byte[] encoded = SkiaEncoding.Encode(image, pixmap => pixmap.Encode(options));
            encoded = JfifDensity.Apply(encoded, image.Metadata);
            stream.Write(encoded, 0, encoded.Length);
        }
    }

    internal sealed class WebpEncoder : IImageEncoder
    {
        public int Quality { get; set; } = 75;

        public void Encode(Image image, Stream stream)
        {
            var options = new SKWebpEncoderOptions(SKWebpEncoderCompression.Lossy, Math.Max(0, Math.Min(100, Quality)));

            byte[] encoded = SkiaEncoding.Encode(image, pixmap => pixmap.Encode(options));
            stream.Write(encoded, 0, encoded.Length);
        }
    }

    internal sealed class TiffEncoder : IImageEncoder
    {
        public TiffCompression? Compression { get; set; }

        // AnyBitmap writes TIFF through BitMiracle.LibTiff (InternalSaveAsMultiPageTiff); this encoder
        // only identifies the requested format.
        public void Encode(Image image, Stream stream) =>
            throw new NotSupportedException("TIFF is written by AnyBitmap through BitMiracle.LibTiff.");
    }

    internal static class SkiaEncoding
    {
        public static byte[] Encode(Image image, Func<SKPixmap, SKData> encode)
        {
            Rgba32[] pixels = image.GetFrameRgba32(0);
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                var info = new SKImageInfo(image.Width, image.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
                using var pixmap = new SKPixmap(info, handle.AddrOfPinnedObject(), image.Width * 4);
                using SKData data = encode(pixmap) ?? throw new NotSupportedException("SkiaSharp could not encode the image.");
                return data.ToArray();
            }
            finally
            {
                handle.Free();
            }
        }
    }

    /// <summary>Writes the image resolution into the JPEG JFIF APP0 segment (SkiaSharp writes a fixed one).</summary>
    internal static class JfifDensity
    {
        public static byte[] Apply(byte[] jpeg, ImageMetadata metadata)
        {
            byte units;
            double x = metadata.HorizontalResolution, y = metadata.VerticalResolution;
            switch (metadata.ResolutionUnits)
            {
                case PixelResolutionUnit.AspectRatio: units = 0; break;
                case PixelResolutionUnit.PixelsPerCentimeter: units = 2; break;
                case PixelResolutionUnit.PixelsPerMeter: units = 1; x = ResolutionConverter.ToPixelsPerInch(x, PixelResolutionUnit.PixelsPerMeter); y = ResolutionConverter.ToPixelsPerInch(y, PixelResolutionUnit.PixelsPerMeter); break;
                default: units = 1; break;
            }

            ushort xd = (ushort)Math.Max(1, Math.Min(ushort.MaxValue, Math.Round(x)));
            ushort yd = (ushort)Math.Max(1, Math.Min(ushort.MaxValue, Math.Round(y)));

            if (jpeg.Length > 20 && jpeg[2] == 0xFF && jpeg[3] == 0xE0 && jpeg[6] == 'J' && jpeg[7] == 'F' && jpeg[8] == 'I' && jpeg[9] == 'F')
            {
                jpeg[13] = units;
                jpeg[14] = (byte)(xd >> 8); jpeg[15] = (byte)xd;
                jpeg[16] = (byte)(yd >> 8); jpeg[17] = (byte)yd;
                return jpeg;
            }

            byte[] app0 = { 0xFF, 0xE0, 0x00, 0x10, (byte)'J', (byte)'F', (byte)'I', (byte)'F', 0x00, 0x01, 0x01, units, (byte)(xd >> 8), (byte)xd, (byte)(yd >> 8), (byte)yd, 0x00, 0x00 };
            var result = new byte[jpeg.Length + app0.Length];
            Array.Copy(jpeg, 0, result, 0, 2);
            Array.Copy(app0, 0, result, 2, app0.Length);
            Array.Copy(jpeg, 2, result, 2 + app0.Length, jpeg.Length - 2);
            return result;
        }
    }

    /// <summary>BMP writer (32bpp BGRA with an alpha bitfield header, or 24bpp).</summary>
    internal sealed class BmpEncoder : IImageEncoder
    {
        public BmpBitsPerPixel? BitsPerPixel { get; set; }
        public bool SupportTransparency { get; set; }

        public void Encode(Image image, Stream stream)
        {
            int width = image.Width, height = image.Height;
            bool is32 = BitsPerPixel != BmpBitsPerPixel.Pixel24;
            bool alphaHeader = is32 && SupportTransparency;
            int infoHeaderSize = alphaHeader ? 108 : 40;
            int bytesPerPixel = is32 ? 4 : 3;
            int stride = ((width * bytesPerPixel) + 3) & ~3;
            int imageSize = stride * height;
            int offset = 14 + infoHeaderSize;

            int xPpm = (int)Math.Round(ResolutionConverter.ToPixelsPerMeter(image.Metadata.HorizontalResolution, image.Metadata.ResolutionUnits));
            int yPpm = (int)Math.Round(ResolutionConverter.ToPixelsPerMeter(image.Metadata.VerticalResolution, image.Metadata.ResolutionUnits));

            using var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true);
            writer.Write((byte)'B'); writer.Write((byte)'M');
            writer.Write(offset + imageSize);
            writer.Write(0);
            writer.Write(offset);

            writer.Write(infoHeaderSize);
            writer.Write(width);
            writer.Write(height); // positive: bottom-up rows
            writer.Write((short)1);
            writer.Write((short)(bytesPerPixel * 8));
            writer.Write(alphaHeader ? 3 : 0); // BI_BITFIELDS : BI_RGB
            writer.Write(imageSize);
            writer.Write(xPpm);
            writer.Write(yPpm);
            writer.Write(0);
            writer.Write(0);
            if (alphaHeader)
            {
                writer.Write(0x00FF0000); // red mask
                writer.Write(0x0000FF00); // green mask
                writer.Write(0x000000FF); // blue mask
                writer.Write(unchecked((int)0xFF000000)); // alpha mask
                writer.Write(0x73524742); // 'sRGB'
                writer.Write(new byte[36]); // endpoints
                writer.Write(new byte[12]); // gamma
            }

            Rgba32[] pixels = image.GetFrameRgba32(0);
            var row = new byte[stride];
            for (int y = height - 1; y >= 0; y--)
            {
                int source = y * width;
                for (int x = 0; x < width; x++)
                {
                    Rgba32 p = pixels[source + x];
                    int o = x * bytesPerPixel;
                    row[o] = p.B;
                    row[o + 1] = p.G;
                    row[o + 2] = p.R;
                    if (is32) row[o + 3] = p.A;
                }

                writer.Write(row);
            }
        }
    }

    /// <summary>PNG writer that keeps the image's pixel layout (gray/RGB/RGBA, 8 or 16 bits), like ImageSharp.</summary>
    internal sealed class PngEncoder : IImageEncoder
    {
        private static readonly uint[] CrcTable = BuildCrcTable();

        public void Encode(Image image, Stream stream)
        {
            int width = image.Width, height = image.Height;
            (byte colorType, int channels, bool sixteen) = image switch
            {
                Image<L8> => ((byte)0, 1, false),
                Image<L16> => ((byte)0, 1, true),
                Image<La16> => ((byte)4, 2, false),
                Image<La32> => ((byte)4, 2, true),
                Image<Rgb24> or Image<Bgr24> => ((byte)2, 3, false),
                Image<Rgb48> => ((byte)2, 3, true),
                Image<Rgba64> => ((byte)6, 4, true),
                _ => ((byte)6, 4, false),
            };

            int bitDepth = sixteen ? 16 : 8;
            int bytesPerPixel = channels * (sixteen ? 2 : 1);
            int stride = width * bytesPerPixel;
            byte[] palette = null, transparency = null;
            byte[] raw;

            // Like ImageSharp, keep a decoded PNG's palette or low-bit grayscale encoding.
            int sourceType = image.Metadata.PngColorType, sourceDepth = image.Metadata.PngBitDepth;
            if (sourceType == 3 && !sixteen && sourceDepth is 1 or 2 or 4 or 8)
            {
                colorType = 3;
                bitDepth = sourceDepth;
                raw = BuildIndexedRows(image.GetFrameRgba32(0), width, height, bitDepth, out palette, out transparency);
                stride = (width * bitDepth + 7) / 8;
                bytesPerPixel = 1;
            }
            else if (sourceType == 0 && colorType == 0 && !sixteen && sourceDepth is 1 or 2 or 4)
            {
                bitDepth = sourceDepth;
                raw = BuildLowBitGrayRows(image.GetFrameRgba32(0), width, height, bitDepth);
                stride = (width * bitDepth + 7) / 8;
                bytesPerPixel = 1;
            }
            else
            {
                raw = BuildRows(image, channels, sixteen, stride);
            }

            stream.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, 0, 8);

            var ihdr = new byte[13];
            WriteInt32BE(ihdr, 0, width);
            WriteInt32BE(ihdr, 4, height);
            ihdr[8] = (byte)bitDepth;
            ihdr[9] = colorType;
            WriteChunk(stream, "IHDR", ihdr);
            if (palette != null)
            {
                WriteChunk(stream, "PLTE", palette);
                if (transparency != null)
                {
                    WriteChunk(stream, "tRNS", transparency);
                }
            }

            var phys = new byte[9];
            bool aspect = image.Metadata.ResolutionUnits == PixelResolutionUnit.AspectRatio;
            WriteInt32BE(phys, 0, (int)Math.Round(aspect ? image.Metadata.HorizontalResolution : ResolutionConverter.ToPixelsPerMeter(image.Metadata.HorizontalResolution, image.Metadata.ResolutionUnits)));
            WriteInt32BE(phys, 4, (int)Math.Round(aspect ? image.Metadata.VerticalResolution : ResolutionConverter.ToPixelsPerMeter(image.Metadata.VerticalResolution, image.Metadata.ResolutionUnits)));
            phys[8] = (byte)(aspect ? 0 : 1);
            WriteChunk(stream, "pHYs", phys);

            WriteChunk(stream, "IDAT", Zlib(Filter(raw, height, stride, bytesPerPixel)));
            WriteChunk(stream, "IEND", Array.Empty<byte>());
        }

        private static byte[] BuildIndexedRows(Rgba32[] pixels, int width, int height, int bitDepth, out byte[] palette, out byte[] transparency)
        {
            bool transparent = Array.Exists(pixels, p => p.A < 128);
            int maxColors = (1 << bitDepth) - (transparent ? 1 : 0);
            byte[] indices = new OctreeQuantizer(pixels, Math.Max(1, maxColors)).Map(pixels, out Rgba32[] colors, out int transparentIndex);

            palette = new byte[colors.Length * 3];
            for (int i = 0; i < colors.Length; i++)
            {
                palette[i * 3] = colors[i].R;
                palette[(i * 3) + 1] = colors[i].G;
                palette[(i * 3) + 2] = colors[i].B;
            }

            transparency = null;
            if (transparentIndex >= 0)
            {
                transparency = new byte[transparentIndex + 1];
                for (int i = 0; i < transparency.Length; i++) transparency[i] = 255;
                transparency[transparentIndex] = 0;
            }

            return PackRows(indices, width, height, bitDepth);
        }

        private static byte[] BuildLowBitGrayRows(Rgba32[] pixels, int width, int height, int bitDepth)
        {
            int max = (1 << bitDepth) - 1;
            var samples = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                samples[i] = (byte)(((PixelMath.Luminance8(pixels[i].R, pixels[i].G, pixels[i].B) * max) + 127) / 255);
            }

            return PackRows(samples, width, height, bitDepth);
        }

        /// <summary>Packs one sample per pixel into rows of <paramref name="bitDepth"/>-bit samples, most significant bits first.</summary>
        private static byte[] PackRows(byte[] samples, int width, int height, int bitDepth)
        {
            int stride = (width * bitDepth + 7) / 8;
            if (bitDepth == 8)
            {
                return samples;
            }

            var packed = new byte[stride * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int bit = x * bitDepth;
                    packed[(y * stride) + (bit >> 3)] |= (byte)(samples[(y * width) + x] << (8 - bitDepth - (bit & 7)));
                }
            }

            return packed;
        }

        private static byte[] BuildRows(Image image, int channels, bool sixteen, int stride)
        {
            int width = image.Width, height = image.Height;
            var raw = new byte[stride * height];
            if (sixteen)
            {
                Rgba64[] pixels = image.GetFrameRgba64(0);
                int o = 0;
                foreach (Rgba64 p in pixels)
                {
                    if (channels <= 2) { WriteUInt16BE(raw, o, PixelMath.Luminance16(p.R, p.G, p.B)); o += 2; }
                    else { WriteUInt16BE(raw, o, p.R); WriteUInt16BE(raw, o + 2, p.G); WriteUInt16BE(raw, o + 4, p.B); o += 6; }
                    if (channels is 2 or 4) { WriteUInt16BE(raw, o, p.A); o += 2; }
                }
            }
            else
            {
                Rgba32[] pixels = image.GetFrameRgba32(0);
                int o = 0;
                foreach (Rgba32 p in pixels)
                {
                    if (channels <= 2) { raw[o++] = PixelMath.Luminance8(p.R, p.G, p.B); }
                    else { raw[o++] = p.R; raw[o++] = p.G; raw[o++] = p.B; }
                    if (channels is 2 or 4) { raw[o++] = p.A; }
                }
            }

            return raw;
        }

        /// <summary>Adaptive filtering: per row, the filter with the smallest sum of absolute values.</summary>
        private static byte[] Filter(byte[] raw, int height, int stride, int bpp)
        {
            var output = new byte[(stride + 1) * height];
            var candidate = new byte[5][];
            for (int i = 0; i < 5; i++) candidate[i] = new byte[stride];

            for (int y = 0; y < height; y++)
            {
                int row = y * stride;
                int prior = row - stride;
                for (int x = 0; x < stride; x++)
                {
                    int a = x >= bpp ? raw[row + x - bpp] : 0;
                    int b = y > 0 ? raw[prior + x] : 0;
                    int c = x >= bpp && y > 0 ? raw[prior + x - bpp] : 0;
                    int value = raw[row + x];
                    candidate[0][x] = (byte)value;
                    candidate[1][x] = (byte)(value - a);
                    candidate[2][x] = (byte)(value - b);
                    candidate[3][x] = (byte)(value - ((a + b) >> 1));
                    candidate[4][x] = (byte)(value - Paeth(a, b, c));
                }

                int best = 0;
                long bestSum = long.MaxValue;
                for (int f = 0; f < 5; f++)
                {
                    long sum = 0;
                    byte[] data = candidate[f];
                    for (int x = 0; x < stride; x++) sum += data[x] < 128 ? data[x] : 256 - data[x];
                    if (sum < bestSum) { bestSum = sum; best = f; }
                }

                int o = y * (stride + 1);
                output[o] = (byte)best;
                Array.Copy(candidate[best], 0, output, o + 1, stride);
            }

            return output;
        }

        private static int Paeth(int a, int b, int c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
            return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
        }

        private static byte[] Zlib(byte[] data)
        {
            using var output = new MemoryStream();
            output.WriteByte(0x78);
            output.WriteByte(0xDA);
            using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
            {
                deflate.Write(data, 0, data.Length);
            }

            uint a = 1, b = 0;
            foreach (byte value in data)
            {
                a = (a + value) % 65521;
                b = (b + a) % 65521;
            }

            uint adler = (b << 16) | a;
            output.WriteByte((byte)(adler >> 24));
            output.WriteByte((byte)(adler >> 16));
            output.WriteByte((byte)(adler >> 8));
            output.WriteByte((byte)adler);
            return output.ToArray();
        }

        private static void WriteChunk(Stream stream, string type, byte[] data)
        {
            var header = new byte[8];
            WriteInt32BE(header, 0, data.Length);
            for (int i = 0; i < 4; i++) header[4 + i] = (byte)type[i];
            stream.Write(header, 0, 8);
            stream.Write(data, 0, data.Length);

            uint crc = 0xFFFFFFFF;
            for (int i = 4; i < 8; i++) crc = CrcTable[(crc ^ header[i]) & 0xFF] ^ (crc >> 8);
            foreach (byte value in data) crc = CrcTable[(crc ^ value) & 0xFF] ^ (crc >> 8);
            crc ^= 0xFFFFFFFF;
            var crcBytes = new byte[4];
            WriteInt32BE(crcBytes, 0, unchecked((int)crc));
            stream.Write(crcBytes, 0, 4);
        }

        private static uint[] BuildCrcTable()
        {
            var table = new uint[256];
            for (uint n = 0; n < 256; n++)
            {
                uint c = n;
                for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
                table[n] = c;
            }

            return table;
        }

        private static void WriteInt32BE(byte[] d, int o, int v) { d[o] = (byte)(v >> 24); d[o + 1] = (byte)(v >> 16); d[o + 2] = (byte)(v >> 8); d[o + 3] = (byte)v; }
        private static void WriteUInt16BE(byte[] d, int o, ushort v) { d[o] = (byte)(v >> 8); d[o + 1] = (byte)v; }
    }

    /// <summary>GIF89a writer: every frame, octree-quantised to a local palette, with transparency and looping.</summary>
    internal sealed class GifEncoder : IImageEncoder
    {
        public void Encode(Image image, Stream stream)
        {
            int width = image.Width, height = image.Height;
            using var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("GIF89a"));
            writer.Write((ushort)width);
            writer.Write((ushort)height);
            writer.Write((byte)0x70); // no global colour table, 8-bit colour resolution
            writer.Write((byte)0);    // background colour index
            writer.Write((byte)0);    // pixel aspect ratio

            int frameCount = image.FrameCount;
            if (frameCount > 1)
            {
                writer.Write(new byte[] { 0x21, 0xFF, 0x0B });
                writer.Write(System.Text.Encoding.ASCII.GetBytes("NETSCAPE2.0"));
                writer.Write(new byte[] { 0x03, 0x01, 0x00, 0x00, 0x00 }); // loop forever
            }

            for (int f = 0; f < frameCount; f++)
            {
                Rgba32[] pixels = image.GetFrameRgba32(f);
                var quantizer = new OctreeQuantizer(pixels, maxColors: 255);
                byte[] indices = quantizer.Map(pixels, out Rgba32[] palette, out int transparentIndex);

                int paletteBits = 1;
                while ((1 << paletteBits) < palette.Length) paletteBits++;
                int tableSize = 1 << paletteBits;

                int delay = image.Metadata.FrameDelays != null && f < image.Metadata.FrameDelays.Count ? image.Metadata.FrameDelays[f] : 0;
                writer.Write(new byte[] { 0x21, 0xF9, 0x04 });
                writer.Write((byte)((transparentIndex >= 0 ? 0x01 : 0x00) | (frameCount > 1 ? 0x08 : 0x00))); // disposal: restore to background
                writer.Write((ushort)delay);
                writer.Write((byte)Math.Max(0, transparentIndex));
                writer.Write((byte)0);

                writer.Write((byte)0x2C);
                writer.Write((ushort)0);
                writer.Write((ushort)0);
                writer.Write((ushort)width);
                writer.Write((ushort)height);
                writer.Write((byte)(0x80 | (paletteBits - 1))); // local colour table

                for (int i = 0; i < tableSize; i++)
                {
                    Rgba32 c = i < palette.Length ? palette[i] : default;
                    writer.Write(c.R); writer.Write(c.G); writer.Write(c.B);
                }

                int minimumCodeSize = Math.Max(2, paletteBits);
                writer.Write((byte)minimumCodeSize);
                LzwEncode(writer, indices, minimumCodeSize);
            }

            writer.Write((byte)0x3B);
        }

        private static void LzwEncode(BinaryWriter writer, byte[] indices, int minimumCodeSize)
        {
            int clearCode = 1 << minimumCodeSize;
            int endCode = clearCode + 1;
            int codeSize = minimumCodeSize + 1;
            int nextCode = endCode + 1;
            var table = new Dictionary<int, int>();

            var block = new byte[255];
            int blockLength = 0;
            int bitBuffer = 0, bitCount = 0;

            void Emit(int code)
            {
                bitBuffer |= code << bitCount;
                bitCount += codeSize;
                while (bitCount >= 8)
                {
                    block[blockLength++] = (byte)bitBuffer;
                    bitBuffer >>= 8;
                    bitCount -= 8;
                    if (blockLength == 255) { writer.Write((byte)255); writer.Write(block, 0, 255); blockLength = 0; }
                }
            }

            Emit(clearCode);
            if (indices.Length > 0)
            {
                int prefix = indices[0];
                for (int i = 1; i < indices.Length; i++)
                {
                    int key = (prefix << 8) | indices[i];
                    if (table.TryGetValue(key, out int code))
                    {
                        prefix = code;
                        continue;
                    }

                    Emit(prefix);

                    // Grow the code size after emitting, based on the entries added so far (as in the
                    // classic 'compress' encoder); decoders add each entry one code later.
                    if (nextCode > (1 << codeSize) - 1 && codeSize < 12) codeSize++;

                    if (nextCode < 4096)
                    {
                        table[key] = nextCode++;
                    }
                    else
                    {
                        Emit(clearCode);
                        table.Clear();
                        codeSize = minimumCodeSize + 1;
                        nextCode = endCode + 1;
                    }

                    prefix = indices[i];
                }

                Emit(prefix);
                if (nextCode > (1 << codeSize) - 1 && codeSize < 12) codeSize++;
            }

            Emit(endCode);
            if (bitCount > 0) { block[blockLength++] = (byte)bitBuffer; if (blockLength == 255) { writer.Write((byte)255); writer.Write(block, 0, 255); blockLength = 0; } }
            if (blockLength > 0) { writer.Write((byte)blockLength); writer.Write(block, 0, blockLength); }
            writer.Write((byte)0); // block terminator
        }
    }

    /// <summary>Octree colour quantiser; pixels with alpha below 128 map to a transparent palette entry.</summary>
    internal sealed class OctreeQuantizer
    {
        private sealed class Node
        {
            public readonly Node[] Children = new Node[8];
            public bool IsLeaf;
            public long R, G, B;
            public int Count;
            public int PaletteIndex;
            public Node NextReducible;
        }

        private readonly Node _root = new();
        private readonly Node[] _reducible = new Node[8];
        private int _leafCount;
        private readonly int _maxColors;

        public OctreeQuantizer(Rgba32[] pixels, int maxColors)
        {
            _maxColors = maxColors;
            foreach (Rgba32 p in pixels)
            {
                if (p.A < 128) continue;
                Add(_root, p, 0);
                while (_leafCount > _maxColors) Reduce();
            }
        }

        public byte[] Map(Rgba32[] pixels, out Rgba32[] palette, out int transparentIndex)
        {
            var colors = new List<Rgba32>();
            BuildPalette(_root, colors);
            bool hasTransparency = Array.Exists(pixels, p => p.A < 128);
            transparentIndex = -1;
            if (hasTransparency || colors.Count == 0)
            {
                transparentIndex = colors.Count;
                colors.Add(new Rgba32(0, 0, 0, 0));
            }

            palette = colors.ToArray();
            var indices = new byte[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                indices[i] = pixels[i].A < 128 ? (byte)transparentIndex : (byte)Lookup(pixels[i]);
            }

            return indices;
        }

        private static int Index(Rgba32 p, int level)
        {
            int shift = 7 - level;
            return (((p.R >> shift) & 1) << 2) | (((p.G >> shift) & 1) << 1) | ((p.B >> shift) & 1);
        }

        private void Add(Node node, Rgba32 p, int level)
        {
            if (node.IsLeaf || level == 8)
            {
                if (!node.IsLeaf) { node.IsLeaf = true; _leafCount++; }
                node.R += p.R; node.G += p.G; node.B += p.B; node.Count++;
                return;
            }

            int index = Index(p, level);
            Node child = node.Children[index];
            if (child == null)
            {
                child = new Node();
                node.Children[index] = child;
                if (level + 1 < 8)
                {
                    child.NextReducible = _reducible[level + 1];
                    _reducible[level + 1] = child;
                }
            }

            Add(child, p, level + 1);
        }

        private void Reduce()
        {
            int level = 7;
            while (level > 0 && _reducible[level] == null) level--;
            Node node = _reducible[level];
            if (node == null) return;
            _reducible[level] = node.NextReducible;

            int removed = 0;
            for (int i = 0; i < 8; i++)
            {
                Node child = node.Children[i];
                if (child == null) continue;
                node.R += child.R; node.G += child.G; node.B += child.B; node.Count += child.Count;
                if (child.IsLeaf) removed++;
                node.Children[i] = null;
            }

            node.IsLeaf = true;
            _leafCount -= removed - 1;
        }

        private static void BuildPalette(Node node, List<Rgba32> colors)
        {
            if (node.IsLeaf)
            {
                node.PaletteIndex = colors.Count;
                int count = Math.Max(1, node.Count);
                colors.Add(new Rgba32((byte)(node.R / count), (byte)(node.G / count), (byte)(node.B / count)));
                return;
            }

            foreach (Node child in node.Children)
            {
                if (child != null) BuildPalette(child, colors);
            }
        }

        private int Lookup(Rgba32 p)
        {
            Node node = _root;
            for (int level = 0; level < 8 && !node.IsLeaf; level++)
            {
                Node next = node.Children[Index(p, level)];
                if (next == null)
                {
                    // Colour not in the tree (only possible for semi-transparent pixels): use any leaf below.
                    foreach (Node child in node.Children)
                    {
                        if (child != null) { next = child; break; }
                    }
                }

                node = next;
            }

            while (!node.IsLeaf)
            {
                foreach (Node child in node.Children)
                {
                    if (child != null) { node = child; break; }
                }
            }

            return node.PaletteIndex;
        }
    }
}
#endif
