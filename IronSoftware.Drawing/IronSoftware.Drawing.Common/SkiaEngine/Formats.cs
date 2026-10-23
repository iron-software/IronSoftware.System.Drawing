#if NETFRAMEWORK
using System;
using System.Text;

namespace IronSoftware.Drawing.SkiaEngine
{
    internal interface IImageFormat
    {
        string Name { get; }
        string DefaultMimeType { get; }
    }

    // Names and MIME types match ImageSharp's formats.
    internal sealed class PngFormat : IImageFormat { public static readonly PngFormat Instance = new(); public string Name => "PNG"; public string DefaultMimeType => "image/png"; }
    internal sealed class JpegFormat : IImageFormat { public static readonly JpegFormat Instance = new(); public string Name => "JPEG"; public string DefaultMimeType => "image/jpeg"; }
    internal sealed class GifFormat : IImageFormat { public static readonly GifFormat Instance = new(); public string Name => "GIF"; public string DefaultMimeType => "image/gif"; }
    internal sealed class BmpFormat : IImageFormat { public static readonly BmpFormat Instance = new(); public string Name => "BMP"; public string DefaultMimeType => "image/bmp"; }
    internal sealed class TiffFormat : IImageFormat { public static readonly TiffFormat Instance = new(); public string Name => "TIFF"; public string DefaultMimeType => "image/tiff"; }
    internal sealed class WebpFormat : IImageFormat { public static readonly WebpFormat Instance = new(); public string Name => "Webp"; public string DefaultMimeType => "image/webp"; }

    internal static class ImageFormats
    {
        public static IImageFormat Detect(byte[] data)
        {
            if (data == null || data.Length < 8)
            {
                return null;
            }

            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47 && data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A)
                return PngFormat.Instance;
            if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
                return JpegFormat.Instance;
            if (data[0] == 'G' && data[1] == 'I' && data[2] == 'F' && data[3] == '8' && (data[4] == '7' || data[4] == '9') && data[5] == 'a')
                return GifFormat.Instance;
            if (data[0] == 'B' && data[1] == 'M' && data.Length >= 18)
            {
                // Like ImageSharp, also require a known DIB header size.
                int headerSize = BitConverter.ToInt32(data, 14);
                if (headerSize is 12 or 16 or 40 or 52 or 56 or 64 or 108 or 124)
                    return BmpFormat.Instance;
            }

            if ((data[0] == 0x49 && data[1] == 0x49 && (data[2] == 0x2A || data[2] == 0x2B) && data[3] == 0x00)
                || (data[0] == 0x4D && data[1] == 0x4D && data[2] == 0x00 && (data[3] == 0x2A || data[3] == 0x2B)))
                return TiffFormat.Instance;
            if (data.Length >= 12 && data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F' && data[8] == 'W' && data[9] == 'E' && data[10] == 'B' && data[11] == 'P')
                return WebpFormat.Instance;

            return null;
        }
    }

    /// <summary>Header information SkiaSharp does not expose: source pixel layout and resolution.</summary>
    internal sealed class HeaderInfo
    {
        public int PngBitDepth;
        public int PngColorType = -1;
        public bool PngHasTransparency;
        public int JpegComponents;
        public double? HorizontalResolution;
        public double? VerticalResolution;
        public PixelResolutionUnit ResolutionUnits = PixelResolutionUnit.PixelsPerInch;

        public static HeaderInfo Read(byte[] data, IImageFormat format)
        {
            var info = new HeaderInfo();
            try
            {
                switch (format)
                {
                    case PngFormat: info.ReadPng(data); break;
                    case JpegFormat: info.ReadJpeg(data); break;
                    case BmpFormat: info.ReadBmp(data); break;
                }
            }
            catch (Exception)
            {
                // Header metadata is best effort; decoding itself is done by SkiaSharp.
            }

            return info;
        }

        private static int ReadInt32BE(byte[] d, int o) => (d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3];
        private static int ReadUInt16BE(byte[] d, int o) => (d[o] << 8) | d[o + 1];

        private void ReadPng(byte[] d)
        {
            int offset = 8;
            while (offset + 8 <= d.Length)
            {
                int length = ReadInt32BE(d, offset);
                string type = Encoding.ASCII.GetString(d, offset + 4, 4);
                int body = offset + 8;
                if (length < 0 || body + length > d.Length) break;

                switch (type)
                {
                    case "IHDR":
                        PngBitDepth = d[body + 8];
                        PngColorType = d[body + 9];
                        break;
                    case "tRNS":
                        PngHasTransparency = true;
                        break;
                    case "pHYs" when length >= 9:
                        int x = ReadInt32BE(d, body);
                        int y = ReadInt32BE(d, body + 4);
                        if (x > 0 && y > 0)
                        {
                            HorizontalResolution = x;
                            VerticalResolution = y;
                            ResolutionUnits = d[body + 8] == 1 ? PixelResolutionUnit.PixelsPerMeter : PixelResolutionUnit.AspectRatio;
                        }
                        break;
                }

                if (type == "IDAT" || type == "IEND") break;
                offset = body + length + 4; // + CRC
            }
        }

        private void ReadJpeg(byte[] d)
        {
            double? exifX = null, exifY = null;
            int exifUnit = 2;
            bool jfifSet = false;
            int offset = 2;
            while (offset + 4 <= d.Length)
            {
                if (d[offset] != 0xFF) { offset++; continue; }
                byte marker = d[offset + 1];
                if (marker == 0xD8 || marker == 0x01 || (marker >= 0xD0 && marker <= 0xD7)) { offset += 2; continue; }
                if (marker == 0xD9 || marker == 0xDA) break; // end of image / start of scan
                int length = ReadUInt16BE(d, offset + 2);
                int body = offset + 4;
                if (length < 2 || offset + 2 + length > d.Length) break;

                if (marker == 0xE0 && length >= 16 && Encoding.ASCII.GetString(d, body, 4) == "JFIF")
                {
                    int units = d[body + 7];
                    int xDensity = ReadUInt16BE(d, body + 8);
                    int yDensity = ReadUInt16BE(d, body + 10);
                    if (xDensity > 0 && yDensity > 0)
                    {
                        HorizontalResolution = xDensity;
                        VerticalResolution = yDensity;
                        ResolutionUnits = units switch
                        {
                            1 => PixelResolutionUnit.PixelsPerInch,
                            2 => PixelResolutionUnit.PixelsPerCentimeter,
                            _ => PixelResolutionUnit.AspectRatio
                        };
                        jfifSet = true;
                    }
                }
                else if (marker == 0xE1 && length >= 16 && Encoding.ASCII.GetString(d, body, 4) == "Exif")
                {
                    ReadExifResolution(d, body + 6, offset + 2 + length, ref exifX, ref exifY, ref exifUnit);
                }
                else if (marker is 0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or 0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF)
                {
                    JpegComponents = d[body + 5];
                }

                offset += 2 + length;
            }

            if (!jfifSet && exifX > 0 && exifY > 0)
            {
                HorizontalResolution = exifX;
                VerticalResolution = exifY;
                ResolutionUnits = exifUnit == 3 ? PixelResolutionUnit.PixelsPerCentimeter
                    : exifUnit == 1 ? PixelResolutionUnit.AspectRatio
                    : PixelResolutionUnit.PixelsPerInch;
            }
        }

        private static void ReadExifResolution(byte[] d, int tiffStart, int end, ref double? x, ref double? y, ref int unit)
        {
            bool little = d[tiffStart] == 'I';
            int U16(int o) => little ? d[o] | (d[o + 1] << 8) : (d[o] << 8) | d[o + 1];
            int U32(int o) => little ? d[o] | (d[o + 1] << 8) | (d[o + 2] << 16) | (d[o + 3] << 24) : (d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3];

            int ifd = tiffStart + U32(tiffStart + 4);
            if (ifd + 2 > end) return;
            int count = U16(ifd);
            for (int i = 0; i < count; i++)
            {
                int entry = ifd + 2 + (i * 12);
                if (entry + 12 > end) return;
                int tag = U16(entry);
                if (tag == 0x011A || tag == 0x011B)
                {
                    int valueOffset = tiffStart + U32(entry + 8);
                    if (valueOffset + 8 > end) continue;
                    double numerator = (uint)U32(valueOffset);
                    double denominator = (uint)U32(valueOffset + 4);
                    if (denominator == 0) continue;
                    if (tag == 0x011A) x = numerator / denominator; else y = numerator / denominator;
                }
                else if (tag == 0x0128)
                {
                    unit = U16(entry + 8);
                }
            }
        }

        private void ReadBmp(byte[] d)
        {
            if (d.Length < 46 || BitConverter.ToInt32(d, 14) < 40) return;
            int x = BitConverter.ToInt32(d, 38);
            int y = BitConverter.ToInt32(d, 42);
            if (x > 0 && y > 0)
            {
                HorizontalResolution = x;
                VerticalResolution = y;
                ResolutionUnits = PixelResolutionUnit.PixelsPerMeter;
            }
        }
    }
}
#endif
