#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronSoftware.Drawing.SkiaEngine
{
    internal enum PixelResolutionUnit : byte
    {
        AspectRatio = 0,
        PixelsPerInch = 1,
        PixelsPerCentimeter = 2,
        PixelsPerMeter = 3
    }

    internal sealed class ImageMetadata
    {
        // ImageSharp defaults.
        public double HorizontalResolution { get; set; } = 96;
        public double VerticalResolution { get; set; } = 96;
        public PixelResolutionUnit ResolutionUnits { get; set; } = PixelResolutionUnit.PixelsPerInch;

        /// <summary>EXIF orientation (1 = top-left). Consumed by AutoOrient.</summary>
        internal ushort ExifOrientation { get; set; } = 1;

        /// <summary>Per-frame GIF/WebP delay in 1/100 s, kept so re-encoding animations keeps timing.</summary>
        internal List<int> FrameDelays { get; set; }

        public ImageMetadata Clone() => new()
        {
            HorizontalResolution = HorizontalResolution,
            VerticalResolution = VerticalResolution,
            ResolutionUnits = ResolutionUnits,
            ExifOrientation = ExifOrientation,
            FrameDelays = FrameDelays == null ? null : new List<int>(FrameDelays)
        };
    }

    internal sealed class PixelTypeInfo
    {
        public PixelTypeInfo(int bitsPerPixel) => BitsPerPixel = bitsPerPixel;
        public int BitsPerPixel { get; }
    }

    internal delegate void PixelAccessorAction<TPixel>(PixelAccessor<TPixel> accessor) where TPixel : unmanaged, IPixel<TPixel>;

    internal readonly struct PixelAccessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly TPixel[] _data;

        internal PixelAccessor(TPixel[] data, int width, int height)
        {
            _data = data;
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public Span<TPixel> GetRowSpan(int rowIndex) => new(_data, rowIndex * Width, Width);
    }

    /// <summary>A frame of an image, as returned by <see cref="ImageFrameCollection.RootFrame"/>.</summary>
    internal sealed class ImageFrame
    {
        internal ImageFrame(Image owner, int index)
        {
            Owner = owner;
            Index = index;
        }

        internal Image Owner { get; }
        internal int Index { get; }
    }

    internal sealed class ImageFrameCollection
    {
        private readonly Image _owner;
        internal ImageFrameCollection(Image owner) => _owner = owner;

        public int Count => _owner.FrameCount;
        public ImageFrame RootFrame => new(_owner, 0);
        public Image CloneFrame(int index) => _owner.CloneFrameInternal(index);
        public void AddFrame(ImageFrame frame) => _owner.AddFrameInternal(frame.Owner, frame.Index);
    }

    internal abstract class Image : IDisposable
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public ImageMetadata Metadata { get; internal set; } = new();
        public abstract PixelTypeInfo PixelType { get; }
        public ImageFrameCollection Frames => new(this);

        internal abstract int FrameCount { get; }
        internal abstract Image CloneFrameInternal(int index);
        internal abstract void AddFrameInternal(Image source, int frameIndex);
        internal abstract Rgba32[] GetFrameRgba32(int frameIndex);
        internal abstract Rgba64[] GetFrameRgba64(int frameIndex);
        internal abstract Image CloneInternal();

        public abstract Image<TOther> CloneAs<TOther>() where TOther : unmanaged, IPixel<TOther>;

        public Image Clone(Action<IImageProcessingContext> operation)
        {
            Image clone = CloneInternal();
            clone.Mutate(operation);
            return clone;
        }

        public void Mutate(Action<IImageProcessingContext> operation) => operation(new ImageProcessingContext(this));

        public void Save(Stream stream, IImageEncoder encoder) => encoder.Encode(this, stream);

        public void SaveAsGif(Stream stream) => Save(stream, new GifEncoder());

        // Processing primitives, implemented per pixel type by Image<TPixel>.
        internal abstract void ApplyCrop(Rectangle rectangle);
        internal abstract void ApplyResize(int width, int height);
        internal abstract void ApplyBoxPad(int width, int height, Color padColor);
        internal abstract void ApplyRotateFlip(RotateMode rotateMode, FlipMode flipMode);
        internal abstract void ApplyBackgroundColor(Color color);
        internal abstract void ApplyFill(Color color, Rectangle region);

        public virtual void Dispose()
        {
            // Pixel memory is managed; nothing to release.
        }

        public static IImageFormat DetectFormat(byte[] data) => ImageFormats.Detect(data);

        public static Image Load(byte[] data) => ImageDecoder.Decode(data);

        public static Image Load(string path) => ImageDecoder.Decode(File.ReadAllBytes(path));

        public static Image<TPixel> Load<TPixel>(byte[] data) where TPixel : unmanaged, IPixel<TPixel>
        {
            Image image = ImageDecoder.Decode(data);
            return image as Image<TPixel> ?? image.CloneAs<TPixel>();
        }

        public static Image<TPixel> LoadPixelData<TPixel>(ReadOnlySpan<byte> data, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
        {
            var pixels = new TPixel[width * height];
            ReadOnlySpan<TPixel> source = MemoryMarshal.Cast<byte, TPixel>(data);
            if (source.Length < pixels.Length)
            {
                throw new ArgumentException($"The pixel data is too small for a {width}x{height} image.", nameof(data));
            }

            source.Slice(0, pixels.Length).CopyTo(pixels);
            return new Image<TPixel>(width, height, new List<TPixel[]> { pixels }, new ImageMetadata());
        }
    }

    internal sealed class Image<TPixel> : Image where TPixel : unmanaged, IPixel<TPixel>
    {
        private static readonly PixelTypeInfo PixelInfo = new(PixelMath.BitsPerPixel<TPixel>());

        private int _width;
        private int _height;
        private List<TPixel[]> _frames;

        public Image(int width, int height)
            : this(width, height, new List<TPixel[]> { new TPixel[checked(width * height)] }, new ImageMetadata())
        {
        }

        internal Image(int width, int height, List<TPixel[]> frames, ImageMetadata metadata)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), $"Invalid image size {width}x{height}.");
            }

            _width = width;
            _height = height;
            _frames = frames;
            Metadata = metadata ?? new ImageMetadata();
        }

        public override int Width => _width;
        public override int Height => _height;
        public override PixelTypeInfo PixelType => PixelInfo;
        internal override int FrameCount => _frames.Count;
        internal TPixel[] RootPixels => _frames[0];

        public TPixel this[int x, int y]
        {
            get => _frames[0][(y * _width) + x];
            set => _frames[0][(y * _width) + x] = value;
        }

        public void ProcessPixelRows(PixelAccessorAction<TPixel> processPixels) =>
            processPixels(new PixelAccessor<TPixel>(_frames[0], _width, _height));

        public void CopyPixelDataTo(Span<byte> destination) =>
            MemoryMarshal.AsBytes(_frames[0].AsSpan()).CopyTo(destination);

        public override Image<TOther> CloneAs<TOther>()
        {
            var frames = new List<TOther[]>(_frames.Count);
            foreach (TPixel[] frame in _frames)
            {
                frames.Add(Convert<TOther>(frame));
            }

            return new Image<TOther>(_width, _height, frames, Metadata.Clone());
        }

        internal override Image CloneInternal() =>
            new Image<TPixel>(_width, _height, _frames.Select(f => (TPixel[])f.Clone()).ToList(), Metadata.Clone());

        internal override Image CloneFrameInternal(int index) =>
            new Image<TPixel>(_width, _height, new List<TPixel[]> { (TPixel[])_frames[index].Clone() }, Metadata.Clone());

        internal override void AddFrameInternal(Image source, int frameIndex)
        {
            if (source.Width != _width || source.Height != _height)
            {
                throw new ArgumentException("Frame must have the same dimensions as the image.", nameof(source));
            }

            _frames.Add(source is Image<TPixel> same
                ? (TPixel[])same._frames[frameIndex].Clone()
                : FromRgba64(source.GetFrameRgba64(frameIndex)));
        }

        internal override Rgba32[] GetFrameRgba32(int frameIndex)
        {
            TPixel[] frame = _frames[frameIndex];
            if (frame is Rgba32[] rgba)
            {
                return (Rgba32[])rgba.Clone();
            }

            var result = new Rgba32[frame.Length];
            for (int i = 0; i < frame.Length; i++)
            {
                result[i] = frame[i].ToRgba32();
            }

            return result;
        }

        internal override Rgba64[] GetFrameRgba64(int frameIndex)
        {
            TPixel[] frame = _frames[frameIndex];
            var result = new Rgba64[frame.Length];
            for (int i = 0; i < frame.Length; i++)
            {
                result[i] = frame[i].ToRgba64();
            }

            return result;
        }

        private static TOther[] Convert<TOther>(TPixel[] source) where TOther : unmanaged, IPixel<TOther>
        {
            if (source is TOther[] same)
            {
                return (TOther[])same.Clone();
            }

            var result = new TOther[source.Length];
            if (Is16Bit<TPixel>() || Is16Bit<TOther>())
            {
                for (int i = 0; i < source.Length; i++)
                {
                    result[i].FromRgba64(source[i].ToRgba64());
                }
            }
            else
            {
                for (int i = 0; i < source.Length; i++)
                {
                    result[i].FromRgba32(source[i].ToRgba32());
                }
            }

            return result;
        }

        internal static bool Is16Bit<T>() =>
            typeof(T) == typeof(Rgba64) || typeof(T) == typeof(Rgb48) || typeof(T) == typeof(L16) || typeof(T) == typeof(La32);

        private static TPixel[] FromRgba32(Rgba32[] source)
        {
            if (source is TPixel[] same)
            {
                return same;
            }

            var result = new TPixel[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i].FromRgba32(source[i]);
            }

            return result;
        }

        private static TPixel[] FromRgba64(Rgba64[] source)
        {
            if (source is TPixel[] same)
            {
                return same;
            }

            var result = new TPixel[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i].FromRgba64(source[i]);
            }

            return result;
        }

        internal override void ApplyCrop(Rectangle rectangle)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= 0 || rectangle.X < 0 || rectangle.Y < 0
                || rectangle.X + rectangle.Width > _width || rectangle.Y + rectangle.Height > _height)
            {
                throw new ArgumentException("Crop rectangle should be smaller than the source bounds.", nameof(rectangle));
            }

            for (int f = 0; f < _frames.Count; f++)
            {
                TPixel[] source = _frames[f];
                var cropped = new TPixel[rectangle.Width * rectangle.Height];
                for (int y = 0; y < rectangle.Height; y++)
                {
                    Array.Copy(source, ((rectangle.Y + y) * _width) + rectangle.X, cropped, y * rectangle.Width, rectangle.Width);
                }

                _frames[f] = cropped;
            }

            _width = rectangle.Width;
            _height = rectangle.Height;
        }

        internal override void ApplyResize(int width, int height)
        {
            // ImageSharp keeps the aspect ratio when one dimension is 0.
            if (width == 0 && height > 0) width = Math.Max(1, (int)Math.Round(_width * (height / (double)_height)));
            if (height == 0 && width > 0) height = Math.Max(1, (int)Math.Round(_height * (width / (double)_width)));
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Resize dimensions must be greater than 0.");
            }

            if (width == _width && height == _height)
            {
                return;
            }

            bool is16 = Is16Bit<TPixel>();
            for (int f = 0; f < _frames.Count; f++)
            {
                _frames[f] = is16
                    ? FromRgba64(SkiaOps.Resize(GetFrameRgba64(f), _width, _height, width, height))
                    : FromRgba32(SkiaOps.Resize(GetFrameRgba32(f), _width, _height, width, height));
            }

            _width = width;
            _height = height;
        }

        internal override void ApplyBoxPad(int width, int height, Color padColor)
        {
            // ResizeMode.BoxPad: pad without scaling when the source fits; otherwise scale down to fit
            // (keeping the aspect ratio) and pad. The image is centred.
            int scaledWidth = _width;
            int scaledHeight = _height;
            if (_width > width || _height > height)
            {
                double ratio = Math.Min(width / (double)_width, height / (double)_height);
                scaledWidth = Math.Max(1, (int)Math.Round(_width * ratio));
                scaledHeight = Math.Max(1, (int)Math.Round(_height * ratio));
            }

            ApplyResize(scaledWidth, scaledHeight);

            if (scaledWidth == width && scaledHeight == height)
            {
                return;
            }

            int offsetX = (width - scaledWidth) / 2;
            int offsetY = (height - scaledHeight) / 2;
            TPixel pad = default;
            pad.FromRgba32(padColor);

            for (int f = 0; f < _frames.Count; f++)
            {
                TPixel[] source = _frames[f];
                var padded = new TPixel[width * height];
                padded.AsSpan().Fill(pad);
                for (int y = 0; y < scaledHeight; y++)
                {
                    Array.Copy(source, y * scaledWidth, padded, ((offsetY + y) * width) + offsetX, scaledWidth);
                }

                _frames[f] = padded;
            }

            _width = width;
            _height = height;
        }

        internal override void ApplyRotateFlip(RotateMode rotateMode, FlipMode flipMode)
        {
            int degrees = ((int)rotateMode % 360 + 360) % 360;
            if (degrees != 0)
            {
                int newWidth = degrees == 180 ? _width : _height;
                int newHeight = degrees == 180 ? _height : _width;
                for (int f = 0; f < _frames.Count; f++)
                {
                    TPixel[] source = _frames[f];
                    var rotated = new TPixel[source.Length];
                    for (int y = 0; y < _height; y++)
                    {
                        int row = y * _width;
                        for (int x = 0; x < _width; x++)
                        {
                            int destination = degrees switch
                            {
                                90 => (x * newWidth) + (_height - 1 - y),
                                180 => ((_height - 1 - y) * newWidth) + (_width - 1 - x),
                                _ => ((_width - 1 - x) * newWidth) + y, // 270
                            };
                            rotated[destination] = source[row + x];
                        }
                    }

                    _frames[f] = rotated;
                }

                _width = newWidth;
                _height = newHeight;
            }

            if (flipMode == FlipMode.None)
            {
                return;
            }

            foreach (TPixel[] frame in _frames)
            {
                if (flipMode == FlipMode.Horizontal)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Array.Reverse(frame, y * _width, _width);
                    }
                }
                else
                {
                    var rowBuffer = new TPixel[_width];
                    for (int y = 0; y < _height / 2; y++)
                    {
                        int top = y * _width;
                        int bottom = (_height - 1 - y) * _width;
                        Array.Copy(frame, top, rowBuffer, 0, _width);
                        Array.Copy(frame, bottom, frame, top, _width);
                        Array.Copy(rowBuffer, 0, frame, bottom, _width);
                    }
                }
            }
        }

        internal override void ApplyBackgroundColor(Color color)
        {
            // Source-over composition of the image onto the background colour (ImageSharp BackgroundColor).
            for (int f = 0; f < _frames.Count; f++)
            {
                Rgba32[] pixels = GetFrameRgba32(f);
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Blend(pixels[i], color, pixels[i].A / 255f, color.A / 255f, sourceOnTop: true);
                }

                _frames[f] = FromRgba32(pixels);
            }
        }

        internal override void ApplyFill(Color color, Rectangle region)
        {
            int x0 = 0, y0 = 0, x1 = _width, y1 = _height;
            if (region != null)
            {
                Rectangle r = region;
                x0 = Math.Max(0, r.X);
                y0 = Math.Max(0, r.Y);
                x1 = Math.Min(_width, r.X + r.Width);
                y1 = Math.Min(_height, r.Y + r.Height);
                if (x0 >= x1 || y0 >= y1)
                {
                    return;
                }
            }

            float alpha = color.A / 255f;
            TPixel opaque = default;
            opaque.FromRgba32(color);

            foreach (TPixel[] frame in _frames)
            {
                for (int y = y0; y < y1; y++)
                {
                    int row = y * _width;
                    for (int x = x0; x < x1; x++)
                    {
                        if (color.A == 255)
                        {
                            frame[row + x] = opaque;
                        }
                        else
                        {
                            Rgba32 destination = frame[row + x].ToRgba32();
                            frame[row + x].FromRgba32(Blend(color, destination, alpha, destination.A / 255f, sourceOnTop: true));
                        }
                    }
                }
            }
        }

        /// <summary>Porter-Duff source-over: <paramref name="top"/> over <paramref name="bottom"/>.</summary>
        private static Rgba32 Blend(Rgba32 top, Rgba32 bottom, float topAlpha, float bottomAlpha, bool sourceOnTop)
        {
            float outAlpha = topAlpha + (bottomAlpha * (1 - topAlpha));
            if (outAlpha <= 0)
            {
                return new Rgba32(0, 0, 0, 0);
            }

            byte Channel(byte t, byte b) => (byte)Math.Round(((t * topAlpha) + (b * bottomAlpha * (1 - topAlpha))) / outAlpha);
            return new Rgba32(Channel(top.R, bottom.R), Channel(top.G, bottom.G), Channel(top.B, bottom.B), (byte)Math.Round(outAlpha * 255));
        }
    }
}
#endif
