#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BitMiracle.LibTiff.Classic;
using SkiaSharp;

namespace IronSoftware.Drawing.SkiaEngine
{
    internal static class ImageDecoder
    {
        private enum PixelKind { Rgba32, Rgb24, L8, L16, La16, La32, Rgb48, Rgba64 }

        public static Image Decode(byte[] data)
        {
            IImageFormat format = ImageFormats.Detect(data)
                ?? throw new NotSupportedException("Image cannot be loaded. The image format is not recognised.");

            if (format is TiffFormat)
            {
                return TiffDecoder.Decode(data);
            }

            HeaderInfo header = HeaderInfo.Read(data, format);
            using SKData skData = SKData.CreateCopy(data);
            using SKCodec codec = SKCodec.Create(skData)
                ?? throw new NotSupportedException($"Image cannot be loaded. The {format.Name} data could not be decoded.");

            int width = codec.Info.Width;
            int height = codec.Info.Height;
            bool animated = format is GifFormat or WebpFormat;
            int frameCount = animated ? Math.Max(1, codec.FrameCount) : 1;
            SKCodecFrameInfo[] frameInfo = animated && codec.FrameCount > 0 ? codec.FrameInfo : Array.Empty<SKCodecFrameInfo>();

            var metadata = new ImageMetadata { ExifOrientation = (ushort)codec.EncodedOrigin };
            if (metadata.ExifOrientation is < 1 or > 8)
            {
                metadata.ExifOrientation = 1;
            }

            if (header.HorizontalResolution > 0 && header.VerticalResolution > 0)
            {
                metadata.HorizontalResolution = header.HorizontalResolution.Value;
                metadata.VerticalResolution = header.VerticalResolution.Value;
                metadata.ResolutionUnits = header.ResolutionUnits;
            }

            if (frameInfo.Length > 0)
            {
                metadata.FrameDelays = frameInfo.Select(f => Math.Max(0, f.Duration / 10)).ToList();
            }

            PixelKind kind = ChoosePixelKind(format, header);
            bool sixteenBit = kind is PixelKind.L16 or PixelKind.La32 or PixelKind.Rgb48 or PixelKind.Rgba64;

            if (sixteenBit)
            {
                List<Rgba64[]> frames;
                try
                {
                    frames = DecodeFrames<Rgba64>(codec, width, height, frameCount, frameInfo, SKColorType.Rgba16161616);
                }
                catch (InvalidConversionException)
                {
                    // Some codecs cannot convert straight to 16-bit integer RGBA; go through half floats instead.
                    frames = DecodeFrames<ulong>(codec, width, height, frameCount, frameInfo, SKColorType.RgbaF16).Select(HalfToRgba64).ToList();
                }
                return kind switch
                {
                    PixelKind.L16 => Build<L16>(width, height, frames, metadata),
                    PixelKind.La32 => Build<La32>(width, height, frames, metadata),
                    PixelKind.Rgb48 => Build<Rgb48>(width, height, frames, metadata),
                    _ => new Image<Rgba64>(width, height, frames, metadata),
                };
            }

            List<Rgba32[]> frames8 = DecodeFrames<Rgba32>(codec, width, height, frameCount, frameInfo, SKColorType.Rgba8888);
            return kind switch
            {
                PixelKind.Rgb24 => Build<Rgb24>(width, height, frames8, metadata),
                PixelKind.L8 => Build<L8>(width, height, frames8, metadata),
                PixelKind.La16 => Build<La16>(width, height, frames8, metadata),
                _ => new Image<Rgba32>(width, height, frames8, metadata),
            };
        }

        /// <summary>The pixel type ImageSharp's non-generic Image.Load picks for the source.</summary>
        private static PixelKind ChoosePixelKind(IImageFormat format, HeaderInfo header)
        {
            if (format is JpegFormat)
            {
                return header.JpegComponents == 1 ? PixelKind.L8 : PixelKind.Rgb24;
            }

            if (format is not PngFormat || header.PngColorType < 0)
            {
                return PixelKind.Rgba32;
            }

            bool sixteen = header.PngBitDepth == 16;
            bool transparent = header.PngHasTransparency;
            return header.PngColorType switch
            {
                0 => sixteen ? (transparent ? PixelKind.La32 : PixelKind.L16) : (transparent ? PixelKind.La16 : PixelKind.L8),
                2 => sixteen ? (transparent ? PixelKind.Rgba64 : PixelKind.Rgb48) : (transparent ? PixelKind.Rgba32 : PixelKind.Rgb24),
                4 => sixteen ? PixelKind.La32 : PixelKind.La16,
                6 => sixteen ? PixelKind.Rgba64 : PixelKind.Rgba32,
                _ => PixelKind.Rgba32, // palette
            };
        }

        private static List<T[]> DecodeFrames<T>(SKCodec codec, int width, int height, int frameCount, SKCodecFrameInfo[] frameInfo, SKColorType colorType)
            where T : unmanaged
        {
            var info = new SKImageInfo(width, height, colorType, SKAlphaType.Unpremul);
            var frames = new List<T[]>(frameCount);
            for (int i = 0; i < frameCount; i++)
            {
                var pixels = new T[width * height];
                int required = i < frameInfo.Length ? frameInfo[i].RequiredFrame : -1;
                if (required >= 0 && required < frames.Count)
                {
                    Array.Copy(frames[required], pixels, pixels.Length);
                }

                var options = new SKCodecOptions(i, required);
                GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                try
                {
                    SKCodecResult result = codec.GetPixels(info, handle.AddrOfPinnedObject(), options);
                    if (result == SKCodecResult.InvalidConversion)
                    {
                        throw new InvalidConversionException();
                    }

                    if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
                    {
                        throw new NotSupportedException($"Image cannot be loaded. Decoding failed: {result}.");
                    }
                }
                finally
                {
                    handle.Free();
                }

                frames.Add(pixels);
            }

            return frames;
        }

        private sealed class InvalidConversionException : Exception
        {
        }

        private static Rgba64[] HalfToRgba64(ulong[] halves)
        {
            var pixels = new Rgba64[halves.Length];
            for (int i = 0; i < halves.Length; i++)
            {
                ulong h = halves[i];
                pixels[i] = new Rgba64(HalfToUInt16((ushort)h), HalfToUInt16((ushort)(h >> 16)), HalfToUInt16((ushort)(h >> 32)), HalfToUInt16((ushort)(h >> 48)));
            }

            return pixels;
        }

        /// <summary>Converts an IEEE 754 half holding a 0..1 colour value to a 16-bit channel.</summary>
        private static ushort HalfToUInt16(ushort half)
        {
            if ((half & 0x8000) != 0)
            {
                return 0;
            }

            int exponent = (half >> 10) & 0x1F;
            int mantissa = half & 0x3FF;
            double value = exponent == 0
                ? mantissa / 1024.0 * Math.Pow(2, -14)
                : exponent == 31 ? 1.0 : (1 + (mantissa / 1024.0)) * Math.Pow(2, exponent - 15);
            return (ushort)Math.Round(Math.Min(1.0, value) * 65535);
        }

        private static Image<TPixel> Build<TPixel>(int width, int height, List<Rgba32[]> frames, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
        {
            var converted = new List<TPixel[]>(frames.Count);
            foreach (Rgba32[] frame in frames)
            {
                var pixels = new TPixel[frame.Length];
                for (int i = 0; i < frame.Length; i++)
                {
                    pixels[i].FromRgba32(frame[i]);
                }

                converted.Add(pixels);
            }

            return new Image<TPixel>(width, height, converted, metadata);
        }

        private static Image<TPixel> Build<TPixel>(int width, int height, List<Rgba64[]> frames, ImageMetadata metadata) where TPixel : unmanaged, IPixel<TPixel>
        {
            var converted = new List<TPixel[]>(frames.Count);
            foreach (Rgba64[] frame in frames)
            {
                var pixels = new TPixel[frame.Length];
                for (int i = 0; i < frame.Length; i++)
                {
                    pixels[i].FromRgba64(frame[i]);
                }

                converted.Add(pixels);
            }

            return new Image<TPixel>(width, height, converted, metadata);
        }
    }

    /// <summary>TIFF decoding through BitMiracle.LibTiff (all pages, decoded to Rgba32).</summary>
    internal static class TiffDecoder
    {
        private sealed class QuietErrorHandler : TiffErrorHandler
        {
            public override void WarningHandler(Tiff tif, string method, string format, params object[] args) { }
            public override void WarningHandlerExt(Tiff tif, object clientData, string method, string format, params object[] args) { }
        }

        public static Image Decode(byte[] data)
        {
            Tiff.SetErrorHandler(new QuietErrorHandler());
            using var stream = new MemoryStream(data);
            using Tiff tiff = Tiff.ClientOpen("in-memory", "r", stream, new TiffStream())
                ?? throw new NotSupportedException("Image cannot be loaded. The TIFF data could not be opened.");

            Image<Rgba32> result = null;
            do
            {
                FieldValue[] subFileType = tiff.GetField(TiffTag.SUBFILETYPE);
                if (subFileType != null && subFileType.Length > 0 && (FileType)subFileType[0].Value == FileType.REDUCEDIMAGE)
                {
                    continue; // thumbnail
                }

                int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                var raster = new int[width * height];
                if (!tiff.ReadRGBAImageOriented(width, height, raster, Orientation.TOPLEFT))
                {
                    throw new NotSupportedException("Image cannot be loaded. The TIFF page could not be decoded.");
                }

                // LibTiff packs each pixel as ABGR in an int: little-endian bytes R, G, B, A = Rgba32 layout.
                var pixels = new Rgba32[width * height];
                MemoryMarshal.AsBytes(raster.AsSpan()).CopyTo(MemoryMarshal.AsBytes(pixels.AsSpan()));

                if (result == null)
                {
                    var metadata = new ImageMetadata();
                    FieldValue[] x = tiff.GetField(TiffTag.XRESOLUTION);
                    FieldValue[] y = tiff.GetField(TiffTag.YRESOLUTION);
                    FieldValue[] unit = tiff.GetField(TiffTag.RESOLUTIONUNIT);
                    if (x != null && y != null && x[0].ToDouble() > 0 && y[0].ToDouble() > 0)
                    {
                        metadata.HorizontalResolution = x[0].ToDouble();
                        metadata.VerticalResolution = y[0].ToDouble();
                        metadata.ResolutionUnits = unit != null && (ResUnit)unit[0].ToInt() == ResUnit.CENTIMETER
                            ? PixelResolutionUnit.PixelsPerCentimeter
                            : unit != null && (ResUnit)unit[0].ToInt() == ResUnit.NONE ? PixelResolutionUnit.AspectRatio : PixelResolutionUnit.PixelsPerInch;
                    }

                    result = new Image<Rgba32>(width, height, new List<Rgba32[]> { pixels }, metadata);
                }
                else
                {
                    var page = new Image<Rgba32>(width, height, new List<Rgba32[]> { pixels }, new ImageMetadata());
                    if (width != result.Width || height != result.Height)
                    {
                        page.ApplyBoxPad(result.Width, result.Height, Color.Transparent);
                    }

                    result.AddFrameInternal(page, 0);
                }
            }
            while (tiff.ReadDirectory());

            return result ?? throw new NotSupportedException("Image cannot be loaded. The TIFF contains no image pages.");
        }
    }
}
#endif
