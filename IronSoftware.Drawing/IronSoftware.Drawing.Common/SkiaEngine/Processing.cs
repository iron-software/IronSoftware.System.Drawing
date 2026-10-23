#if NETFRAMEWORK
using System;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace IronSoftware.Drawing.SkiaEngine
{
    internal enum RotateMode
    {
        None = 0,
        Rotate90 = 90,
        Rotate180 = 180,
        Rotate270 = 270
    }

    internal enum FlipMode
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }

    internal enum ResizeMode
    {
        Crop,
        Pad,
        BoxPad,
        Max,
        Min,
        Stretch,
        Manual
    }

    internal enum AnchorPositionMode
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    internal sealed class ResizeOptions
    {
        public Size Size { get; set; }
        public ResizeMode Mode { get; set; } = ResizeMode.Crop;
        public Color PadColor { get; set; } = Color.Transparent;
        public AnchorPositionMode Position { get; set; } = AnchorPositionMode.Center;
    }

    internal sealed class SolidBrush
    {
        public SolidBrush(Color color) => Color = color;
        public Color Color { get; }
    }

    internal interface IImageProcessingContext
    {
        IImageProcessingContext Crop(Rectangle rectangle);
        IImageProcessingContext Resize(int width, int height);
        IImageProcessingContext Resize(ResizeOptions options);
        IImageProcessingContext RotateFlip(RotateMode rotateMode, FlipMode flipMode);
        IImageProcessingContext AutoOrient();
        IImageProcessingContext BackgroundColor(Color color);
        IImageProcessingContext Fill(Color color);
        IImageProcessingContext Fill(SolidBrush brush, Rectangle rectangle);
    }

    internal sealed class ImageProcessingContext : IImageProcessingContext
    {
        private readonly Image _image;
        internal ImageProcessingContext(Image image) => _image = image;

        public IImageProcessingContext Crop(Rectangle rectangle) { _image.ApplyCrop(rectangle); return this; }

        public IImageProcessingContext Resize(int width, int height) { _image.ApplyResize(width, height); return this; }

        public IImageProcessingContext Resize(ResizeOptions options)
        {
            if (options.Mode == ResizeMode.BoxPad && options.Position == AnchorPositionMode.Center)
            {
                _image.ApplyBoxPad(options.Size.Width, options.Size.Height, options.PadColor);
            }
            else if (options.Mode == ResizeMode.Stretch)
            {
                _image.ApplyResize(options.Size.Width, options.Size.Height);
            }
            else
            {
                throw new NotSupportedException($"Resize mode {options.Mode} with anchor {options.Position} is not supported.");
            }

            return this;
        }

        public IImageProcessingContext RotateFlip(RotateMode rotateMode, FlipMode flipMode) { _image.ApplyRotateFlip(rotateMode, flipMode); return this; }

        public IImageProcessingContext AutoOrient()
        {
            // EXIF orientation values 1-8 (same handling as ImageSharp's AutoOrient).
            switch (_image.Metadata.ExifOrientation)
            {
                case 2: _image.ApplyRotateFlip(RotateMode.None, FlipMode.Horizontal); break;
                case 3: _image.ApplyRotateFlip(RotateMode.Rotate180, FlipMode.None); break;
                case 4: _image.ApplyRotateFlip(RotateMode.None, FlipMode.Vertical); break;
                case 5: _image.ApplyRotateFlip(RotateMode.Rotate90, FlipMode.Horizontal); break;
                case 6: _image.ApplyRotateFlip(RotateMode.Rotate90, FlipMode.None); break;
                case 7: _image.ApplyRotateFlip(RotateMode.Rotate270, FlipMode.Horizontal); break;
                case 8: _image.ApplyRotateFlip(RotateMode.Rotate270, FlipMode.None); break;
            }

            _image.Metadata.ExifOrientation = 1;
            return this;
        }

        public IImageProcessingContext BackgroundColor(Color color) { _image.ApplyBackgroundColor(color); return this; }

        public IImageProcessingContext Fill(Color color) { _image.ApplyFill(color, null); return this; }

        public IImageProcessingContext Fill(SolidBrush brush, Rectangle rectangle) { _image.ApplyFill(brush.Color, rectangle); return this; }
    }

    /// <summary>SkiaSharp-backed pixel operations.</summary>
    internal static class SkiaOps
    {
        // ImageSharp's default resampler is bicubic (Catmull-Rom); resampling happens in premultiplied space.
        private static readonly SKSamplingOptions Bicubic = new(SKCubicResampler.CatmullRom);

        public static Rgba32[] Resize(Rgba32[] pixels, int width, int height, int newWidth, int newHeight)
        {
            var result = new Rgba32[newWidth * newHeight];
            ResizeCore(pixels, result, width, height, newWidth, newHeight, SKColorType.Rgba8888, 4);
            return result;
        }

        public static Rgba64[] Resize(Rgba64[] pixels, int width, int height, int newWidth, int newHeight)
        {
            var result = new Rgba64[newWidth * newHeight];
            ResizeCore(pixels, result, width, height, newWidth, newHeight, SKColorType.Rgba16161616, 8);
            return result;
        }

        private static void ResizeCore<T>(T[] source, T[] destination, int width, int height, int newWidth, int newHeight, SKColorType colorType, int bytesPerPixel)
            where T : unmanaged
        {
            var sourceHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
            var destinationHandle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                var sourceInfo = new SKImageInfo(width, height, colorType, SKAlphaType.Unpremul);
                var destinationInfo = new SKImageInfo(newWidth, newHeight, colorType, SKAlphaType.Unpremul);
                using SKImage image = SKImage.FromPixels(sourceInfo, sourceHandle.AddrOfPinnedObject(), width * bytesPerPixel);
                using var surface = SKSurface.Create(new SKImageInfo(newWidth, newHeight, colorType, SKAlphaType.Premul));
                if (image == null || surface == null)
                {
                    throw new InvalidOperationException("SkiaSharp could not allocate the resize buffers.");
                }

                using var paint = new SKPaint { BlendMode = SKBlendMode.Src };
                surface.Canvas.DrawImage(image, new SKRect(0, 0, width, height), new SKRect(0, 0, newWidth, newHeight), Bicubic, paint);
                if (!surface.ReadPixels(destinationInfo, destinationHandle.AddrOfPinnedObject(), newWidth * bytesPerPixel, 0, 0))
                {
                    throw new InvalidOperationException("SkiaSharp could not read the resized pixels.");
                }
            }
            finally
            {
                sourceHandle.Free();
                destinationHandle.Free();
            }
        }
    }
}
#endif
