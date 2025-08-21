using BitMiracle.LibTiff.Classic;
using Microsoft.Maui.Graphics.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// <para>A universally compatible Bitmap format for .NET 7, .NET 6, .NET 5,
    /// and .NET Core. As well as compatibility with Windows, NanoServer, 
    /// IIS, macOS, Mobile, Xamarin, iOS, Android, Google Cloud, Azure, AWS, 
    /// and Linux.</para>
    /// <para>Works nicely with popular Image and Bitmap formats such as 
    /// System.Drawing.Bitmap, SkiaSharp, SixLabors.ImageSharp, 
    /// Microsoft.Maui.Graphics.</para>
    /// <para>Implicit casting means that using this class to input and output 
    /// Bitmap and image types from public API's gives full compatibility to 
    /// all image type fully supported by Microsoft.</para>
    /// <para>When casting to and from AnyBitmap, 
    /// please remember to dispose your original Bitmap object (e.g. System.Drawing.Bitmap) 
    /// to avoid unnecessary memory allocation.</para>
    /// <para>Unlike System.Drawing.Bitmap this bitmap object is 
    /// self-memory-managing and does not need to be explicitly 'used' 
    /// or 'disposed'.</para>
    /// </summary>
    public partial class AnyBitmap : IDisposable, IAnyImage
    {
        private bool _disposed = false;

        /// <summary>
        /// We use Lazy because in some case we can skip Image.Load (which use a lot of memory). 
        /// e.g. open jpg file and save it to jpg file without changing anything so we don't need to load the image.
        /// </summary>
        private Lazy<IReadOnlyList<Image>> _lazyImage;

        private IReadOnlyList<Image> GetInternalImages()
        {
            return _lazyImage?.Value ?? throw new InvalidOperationException("No image data available");
        }

        private Image GetFirstInternalImage()
        {
            return (_lazyImage?.Value?[0]) ?? throw new InvalidOperationException("No image data available");
        }

        private void ForceLoadLazyImage()
        {
            var _ = _lazyImage?.Value;
        }

        private readonly object _binaryLock = new object();
        private byte[] _binary;

        /// <summary>
        /// This value save the original bytes, we need to update it each time that Image object (inside _lazyImage) is changed <see cref="IsDirty"/>
        /// </summary>
        private byte[] Binary
        {
            get
            {

                if (_binary == null)
                {
                    //In case like <see cref="AnyBitmap(Image)"/> Binary will be assign once the image is loaded
                    ForceLoadLazyImage(); //force load but _binary can still be null depended on how _lazyImage was loaded              
                }

                if (_binary == null || IsDirty)
                {
                    lock (_binaryLock)
                    {
                        if (_binary == null || IsDirty)
                        {
                            //Which mean we need to update _binary to sync with the image
                            using var stream = new MemoryStream();
                            IImageEncoder enc = GetDefaultImageExportEncoder();

                            GetFirstInternalImage().Save(stream, enc);
                            _binary = stream.ToArray();
                            IsDirty = false;
                        }
                    }  
                }

                return _binary;
            }
            set
            {
                _binary = value;
            }
        }

        private int _isDirty;

        /// <summary>
        /// If IsDirty = true means we need to update Binary. Since  Image object (inside _lazyImage) is changed
        /// </summary>
        private bool IsDirty
        {
            // use Interlocked to make sure that it always updated and thread safe.
            get => Thread.VolatileRead(ref _isDirty) == 1;
            set => Interlocked.Exchange(ref _isDirty, value ? 1 : 0);
        }

        private IImageFormat Format => Image.DetectFormat(Binary);
        private TiffCompression TiffCompression { get; set; } = TiffCompression.Lzw;
        private bool PreserveOriginalFormat { get; set; } = true;

        //cache since Image.Width (ImageSharp) is slow
        private int? _width = null;

        /// <summary>
        /// Width of the image.
        /// </summary>
        public int Width => _width ??= GetFirstInternalImage().Width;

        //cache since Image.Height (ImageSharp) is slow
        private int? _height = null;

        /// <summary>
        /// Height of the image.
        /// </summary>
        public int Height => _height ??= GetFirstInternalImage().Height;

        /// <summary>
        /// Number of raw image bytes stored
        /// </summary>
        public int Length
        {
            get
            {
                return Binary == null ? 0 : Binary.Length;
            }
        }

        /// <summary>
        /// Hashing integer based on image raw binary data.
        /// </summary>
        /// <returns>Int</returns>
        public override int GetHashCode()
        {
            return Binary.GetHashCode();
        }

        /// <summary>
        /// A Base64 encoded string representation of the raw image binary data.
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/bitmap-to-string/">
        /// Code Example</a></para>
        /// </summary>
        /// <returns>The bitmap data as a Base64 string.</returns>
        /// <seealso cref="Convert.ToBase64String(byte[])"/>
        public override string ToString()
        {
            return Convert.ToBase64String(Binary ?? Array.Empty<byte>());
        }

        /// <summary>
        /// The raw image data as byte[] (ByteArray)"/>
        /// </summary>
        /// <returns>A byte[] (ByteArray) </returns>
        public byte[] GetBytes()
        {
            return Binary;
        }

        /// <summary>
        /// The raw image data as a <see cref="MemoryStream"/>
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/bitmap-to-stream/">
        /// Code Example</a></para>
        /// </summary>
        /// <returns><see cref="MemoryStream"/></returns>
        public MemoryStream GetStream()
        {
            return new MemoryStream(Binary);
        }

        /// <summary>
        /// Creates an exact duplicate <see cref="AnyBitmap"/>
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/clone-anybitmap/">
        /// Code Example</a></para>
        /// </summary>
        /// <returns></returns>
        public AnyBitmap Clone()
        {
            return new AnyBitmap(Binary, PreserveOriginalFormat);
        }

        /// <summary>
        /// Creates an exact duplicate <see cref="AnyBitmap"/> of the cropped area.
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/clone-anybitmap/">
        /// Code Example</a></para>
        /// </summary>
        /// <param name="rectangle">Defines the portion of this 
        /// <see cref="AnyBitmap"/> to copy.</param>
        /// <returns></returns>
        public AnyBitmap Clone(Rectangle rectangle)
        {
            var cloned = GetInternalImages().Select(img => img.Clone(x => x.Crop(rectangle)));
            return new AnyBitmap(Binary, cloned);
        }

        /// <summary>
        /// Exports the Bitmap as bytes encoded in the 
        /// <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable this feature.</para>
        /// </summary>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality , 100 is highest.</param>
        /// <returns>Transcoded image bytes.</returns>
        public byte[] ExportBytes(
            ImageFormat format = ImageFormat.Default, int lossy = 100)
        {
            using MemoryStream mem = new();
            ExportStream(mem, format, lossy);
            byte[] byteArray = mem.ToArray();

            return byteArray;
        }

        /// <inheritdoc/>
        public byte[] ExportBytesAsJpg()
        {
            return this.ExportBytes(ImageFormat.Jpeg);
        }

        /// <summary>
        /// Exports the Bitmap as a file encoded in the 
        /// <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable the encoding feature.</para>
        /// <para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/export-anybitmap/">
        /// Code Example</a></para>
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Void. Saves a file to disk.</returns>

        public void ExportFile(
            string file,
            ImageFormat format = ImageFormat.Default,
            int lossy = 100)
        {
            SaveAs(file, format, lossy);
        }

        /// <summary>
        /// Exports the Bitmap as a <see cref="MemoryStream"/> encoded in the 
        /// <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable the encoding feature.</para>
        /// <para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/bitmap-to-stream/">
        /// Code Example</a></para>
        /// </summary>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Transcoded image bytes in a <see cref="MemoryStream"/>.</returns>
        public MemoryStream ToStream(
            ImageFormat format = ImageFormat.Default, int lossy = 100)
        {
            MemoryStream stream = new();
            ExportStream(stream, format, lossy);
            return stream;
        }

        /// <summary>
        /// Exports the Bitmap as a Func<see cref="MemoryStream"/>> encoded in 
        /// the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Transcoded image bytes in a Func <see cref="MemoryStream"/>
        /// </returns>
        public Func<Stream> ToStreamFn(ImageFormat format = ImageFormat.Default, int lossy = 100)
        {
            MemoryStream stream = new();
            ExportStream(stream, format, lossy);
            stream.Position = 0;
            return () => stream;
        }

        /// <summary>
        /// Saves the Bitmap to an existing <see cref="Stream"/> encoded in the
        /// <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="stream">An image encoding format.</param>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Void. Saves Transcoded image bytes to you <see cref="Stream"/>.</returns>
        public void ExportStream(
            Stream stream,
            ImageFormat format = ImageFormat.Default,
            int lossy = 100)
        {

            if (lossy is < 0 or > 100)
            {
                lossy = 100;
            }

            //this Check if _lazyImage is not loaded which mean we didn't touch anything and the output format is the same then we should just return the original data

            var isSameFormat = (format is ImageFormat.Default or ImageFormat.RawFormat) || (GetImageFormat() == format);
            var isCompressNeeded = (format is ImageFormat.Default or ImageFormat.Webp or ImageFormat.Jpeg) && lossy != 100;

            if (!IsDirty && isSameFormat && !isCompressNeeded)
            {
                var writer = new BinaryWriter(stream);
                writer.Write(Binary);
                return;
            }

            try
            {
                IImageEncoder enc = GetDefaultImageExportEncoder(format, lossy);
                if (enc is TiffEncoder)
                {
                    InternalSaveAsMultiPageTiff(_lazyImage?.Value, stream);
                }
                else if (enc is GifEncoder)
                {
                    InternalSaveAsMultiPageGif(_lazyImage?.Value, stream);

                }
                else
                {
                    GetFirstInternalImage().Save(stream, enc);
                }

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    $"Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    $"Cannot export stream with SixLabors.ImageSharp, {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the raw image data to a file.
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <seealso cref="TrySaveAs(string)"/>
        public void SaveAs(string file)
        {
            SaveAs(file, GetImageFormat(file));
        }

        /// <summary>
        /// Saves the image data to a file. Allows for the image to be 
        /// transcoded to popular image formats.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality , 100 is highest.</param>
        /// <returns>Void.  Saves Transcoded image bytes to your File.</returns>
        /// <seealso cref="TrySaveAs(string, ImageFormat, int)"/>
        /// <seealso cref="TrySaveAs(string)"/>
        public void SaveAs(string file, ImageFormat format, int lossy = 100)
        {
            using var fileStream = new FileStream(file, FileMode.Create);
            ExportStream(fileStream, format, lossy);
        }

        /// <summary>
        /// Tries to Save the image data to a file. Allows for the image to be
        /// transcoded to popular image formats.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp
        /// to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <param name="format">An image encoding format.</param>
        /// <param name="lossy">JPEG and WebP encoding quality (ignored for all
        /// other values of <see cref="ImageFormat"/>). Higher values return 
        /// larger file sizes. 0 is lowest quality , 100 is highest.</param>
        /// <returns>returns true on success, false on failure.</returns>
        /// <seealso cref="SaveAs(string, ImageFormat, int)"/>
        public bool TrySaveAs(string file, ImageFormat format, int lossy = 100)
        {
            try
            {
                ExportFile(file, format, lossy);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to Save the raw image data to a file.
        /// <returns>returns true on success, false on failure.</returns>
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <seealso cref="SaveAs(string)"/>
        public bool TrySaveAs(string file)
        {
            try
            {
                SaveAs(file);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generic method to convert popular image types to <see cref="AnyBitmap"/>.
        /// <para> Support includes SixLabors.ImageSharp.Image, 
        /// SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, 
        /// System.Drawing.Image and Microsoft.Maui.Graphics formats.</para>
        /// <para>Syntax sugar. Explicit casts already also exist to and from
        /// <see cref="AnyBitmap"/> and all supported types.</para>
        /// </summary>
        /// <typeparam name="T">The Type to cast from. Support includes 
        /// SixLabors.ImageSharp.Image, SkiaSharp.SKImage, SkiaSharp.SKBitmap,
        /// System.Drawing.Bitmap, System.Drawing.Image and 
        /// Microsoft.Maui.Graphics formats.</typeparam>
        /// <param name="otherBitmapFormat">A bitmap or image format from 
        /// another graphics library.</param>
        /// <returns>A <see cref="AnyBitmap"/></returns>
        public static AnyBitmap FromBitmap<T>(T otherBitmapFormat)
        {
            try
            {
                var bitmap = (System.Drawing.Bitmap)Convert.ChangeType(
                    otherBitmapFormat,
                    typeof(System.Drawing.Bitmap));
                return (AnyBitmap)bitmap;
            }
            catch (Exception e)
            {
                throw new InvalidCastException(typeof(T).FullName, e);
            }
        }
        /// <summary>
        /// Generic method to convert <see cref="AnyBitmap"/> to popular image
        /// types.
        /// <para> Support includes SixLabors.ImageSharp.Image, 
        /// SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, 
        /// System.Drawing.Image and Microsoft.Maui.Graphics formats.</para>
        /// <para>Syntax sugar. Explicit casts already also exist to and from 
        /// <see cref="AnyBitmap"/> and all supported types.</para>
        /// </summary>
        /// <typeparam name="T">The Type to cast to. Support includes 
        /// SixLabors.ImageSharp.Image, SkiaSharp.SKImage, SkiaSharp.SKBitmap, 
        /// System.Drawing.Bitmap, System.Drawing.Image and 
        /// Microsoft.Maui.Graphics formats.</typeparam>
        /// <returns>A <see cref="AnyBitmap"/></returns>
        public T ToBitmap<T>()
        {
            try
            {
                var result = (T)Convert.ChangeType(this, typeof(T));
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidCastException(typeof(T).FullName, e);
            }
        }

        /// <summary>
        /// Create a new Bitmap from a a Byte Span.
        /// </summary>
        /// <param name="span">A Byte Span of image data in any common format.</param>
        public static AnyBitmap FromSpan(ReadOnlySpan<byte> span)
        {
            return new AnyBitmap(span, true);
        }

        /// <summary>
        /// Create a new Bitmap from a a byte span.
        /// </summary>
        /// <param name="span">A byte span of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        public static AnyBitmap FromSpan(ReadOnlySpan<byte> span, bool preserveOriginalFormat)
        {
            return new AnyBitmap(span, preserveOriginalFormat);
        }

        /// <summary>
        /// Create a new Bitmap from a a byte array.
        /// </summary>
        /// <param name="bytes">A byte array of image data in any common format.</param>
        public static AnyBitmap FromBytes(byte[] bytes)
        {
            return new AnyBitmap(bytes, true);
        }

        /// <summary>
        /// Create a new Bitmap from a a byte array.
        /// </summary>
        /// <param name="bytes">A byte array of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        public static AnyBitmap FromBytes(byte[] bytes, bool preserveOriginalFormat)
        {
            return new AnyBitmap(bytes, preserveOriginalFormat);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.
        /// Default is true. Set to false to load as Rgba32.</param>
        /// <seealso cref="FromStream(Stream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(MemoryStream stream)
        {
            return new AnyBitmap(stream, true);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param> 
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromStream(Stream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(MemoryStream stream, bool preserveOriginalFormat)
        {
            return new AnyBitmap(stream, preserveOriginalFormat);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(MemoryStream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(Stream stream)
        {
            return new AnyBitmap(stream, true);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromStream(MemoryStream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(Stream stream, bool preserveOriginalFormat)
        {
            return new AnyBitmap(stream, preserveOriginalFormat);
        }

        /// <summary>
        /// Construct a new Bitmap from binary data (byte span).
        /// </summary>
        /// <param name="span">A byte span of image data in any common format.</param>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(ReadOnlySpan<byte> span)
        {
            LoadImage(span, true);
        }

        /// <summary>
        /// Construct a new Bitmap out of binary data with a byte span.
        /// </summary>
        /// <param name="span">A byte span of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(ReadOnlySpan<byte> span, bool preserveOriginalFormat)
        {
            LoadImage(span, preserveOriginalFormat);
        }

        /// <summary>
        /// Construct a new Bitmap from binary data (bytes).
        /// </summary>
        /// <param name="bytes">A ByteArray of image data in any common format.</param>
        /// <seealso cref="FromBytes(byte[])"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(byte[] bytes)
        {
            LoadImage(bytes, true);
        }

        /// <summary>
        /// Construct a new Bitmap out of binary data with a byte array.
        /// </summary>
        /// <param name="bytes">A byte array of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromBytes(byte[], bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(byte[] bytes, bool preserveOriginalFormat)
        {
            LoadImage(bytes, preserveOriginalFormat);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(Stream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(MemoryStream stream)
        {
            LoadImage(stream.ToArray(), true);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromStream(Stream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(MemoryStream stream, bool preserveOriginalFormat = true)
        {
            LoadImage(stream.ToArray(), preserveOriginalFormat);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(MemoryStream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Stream stream)
        {
            LoadImage(stream, true);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromStream(MemoryStream, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Stream stream, bool preserveOriginalFormat)
        {
            LoadImage(stream, preserveOriginalFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original">The <see cref="AnyBitmap"/> from which to 
        /// create the new <see cref="AnyBitmap"/>.</param>
        /// <param name="width">The width of the new AnyBitmap.</param>
        /// <param name="height">The height of the new AnyBitmap.</param>
        public AnyBitmap(AnyBitmap original, int width, int height)
        {
            LoadAndResizeImage(original, width, height);
        }

        /// <summary>
        /// Construct a new Bitmap from a file.
        /// </summary>
        /// <param name="file">A fully qualified file path./</param>
        /// <seealso cref="FromFile(string)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(string file)
        {
            LoadImage(File.ReadAllBytes(file), true);
        }

        /// <summary>
        /// Construct a new Bitmap from a file.
        /// </summary>
        /// <param name="file">A fully qualified file path./</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromFile(string, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(string file, bool preserveOriginalFormat)
        {
            LoadImage(File.ReadAllBytes(file), preserveOriginalFormat);
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri.
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <seealso cref="FromUriAsync(Uri)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Uri uri)
        {
            try
            {
                using Stream stream = LoadUriAsync(uri).GetAwaiter().GetResult();
                LoadImage(stream, true);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri.
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromUriAsync(Uri, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Uri uri, bool preserveOriginalFormat)
        {
            try
            {
                using Stream stream = LoadUriAsync(uri).GetAwaiter().GetResult();
                LoadImage(stream, preserveOriginalFormat);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from width and height.
        /// </summary>
        /// <param name="width">Width of new AnyBitmap</param>
        /// <param name="height">Height of new AnyBitmap</param>
        /// <param name="backgroundColor">Background color of new AnyBitmap</param>
        public AnyBitmap(int width, int height, Color backgroundColor = null)
        {
            _lazyImage = new Lazy<IReadOnlyList<Image>>(() =>
            {
                var image = new Image<Rgba32>(width, height);
                if (backgroundColor != null)
                {
                    image.Mutate(context => context.Fill(backgroundColor));
                }
                return [image];
            });
            ForceLoadLazyImage();
        }

        /// <summary>
        /// Construct an AnyBitmap object from a buffer of RGB pixel data.
        /// </summary>
        /// <param name="buffer">An array of bytes representing the RGB pixel data. This should contain 3 bytes (one each for red, green, and blue) for each pixel in the image.</param>
        /// <param name="width">The width of the image, in pixels.</param>
        /// <param name="height">The height of the image, in pixels.</param>
        /// <returns>An AnyBitmap object that represents the image defined by the provided pixel data, width, and height.</returns>
        internal AnyBitmap(byte[] buffer, int width, int height)
        {
            _lazyImage = new Lazy<IReadOnlyList<Image>>(() =>
            {
                var image = Image.LoadPixelData<Rgb24>(buffer, width, height);
                return [image];
            });
        }

        /// <summary>
        /// Note: This only use for Casting It won't create new object Image
        /// </summary>
        /// <param name="image"></param>
        internal AnyBitmap(Image image) : this([image])
        {
        }

        /// <summary>
        /// Note: This only use for Casting It won't create new object Image
        /// </summary>
        /// <param name="images"></param>
        internal AnyBitmap(IEnumerable<Image> images)
        {
            _lazyImage = new Lazy<IReadOnlyList<Image>>(() =>
            {
                return [.. images];
            });
        }

        /// <summary>
        /// Fastest AnyBitmap ctor
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="images"></param>
        internal AnyBitmap(byte[] bytes, IEnumerable<Image> images)
        {
            Binary = bytes;
            _lazyImage = new Lazy<IReadOnlyList<Image>>(() =>
            {
                return [.. images];
            });
        }


        /// <summary>
        /// Create a new Bitmap from a file.
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <seealso cref="FromFile(string)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromFile(string file)
        {
            if (file.ToLower().EndsWith(".svg"))
            {
                return LoadSVGImage(file, true);
            }
            else
            {
                return new AnyBitmap(file, true);
            }
        }

        /// <summary>
        /// Create a new Bitmap from a file.
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <seealso cref="FromFile(string, bool)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromFile(string file, bool preserveOriginalFormat)
        {
            if (file.ToLower().EndsWith(".svg"))
            {
                return LoadSVGImage(file, preserveOriginalFormat);
            }
            else
            {
                return new AnyBitmap(file, preserveOriginalFormat);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri.
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <returns></returns>
        /// <seealso cref="AnyBitmap"/>
        /// <seealso cref="FromUri(Uri)"/>
        /// <seealso cref="FromUriAsync(Uri)"/>
        public static async Task<AnyBitmap> FromUriAsync(Uri uri)
        {
            try
            {
                using Stream stream = await LoadUriAsync(uri);
                return new AnyBitmap(stream, true);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri.
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <returns></returns>
        /// <seealso cref="AnyBitmap"/>
        /// <seealso cref="FromUri(Uri, bool)"/>
        /// <seealso cref="FromUriAsync(Uri, bool)"/>
        public static async Task<AnyBitmap> FromUriAsync(Uri uri, bool preserveOriginalFormat)
        {
            try
            {
                using Stream stream = await LoadUriAsync(uri);
                return new AnyBitmap(stream, preserveOriginalFormat);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <returns></returns>
        /// <seealso cref="AnyBitmap"/>
        /// <seealso cref="FromUriAsync(Uri)"/>
#if NET6_0_OR_GREATER
        [Obsolete("FromUri(Uri) is obsolete for net60 or greater because it uses WebClient which is obsolete. Consider using FromUriAsync(Uri) method.")]
#endif
        public static AnyBitmap FromUri(Uri uri)
        {
            try
            {
                using WebClient client = new();
                return new AnyBitmap(client.OpenRead(uri));
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri.
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <param name="preserveOriginalFormat">Determine whether to load <see cref="SixLabors.ImageSharp.Image"/> as its original pixel format or Rgba32.</param>
        /// <returns></returns>
        /// <seealso cref="AnyBitmap"/>
        /// <seealso cref="FromUriAsync(Uri, bool)"/>
#if NET6_0_OR_GREATER
        [Obsolete("FromUri(Uri) is obsolete for net60 or greater because it uses WebClient which is obsolete. Consider using FromUriAsync(Uri) method.")]
#endif
        public static AnyBitmap FromUri(Uri uri, bool preserveOriginalFormat)
        {
            try
            {
                using WebClient client = new();
                return new AnyBitmap(client.OpenRead(uri), preserveOriginalFormat);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Creates an AnyBitmap object from a buffer of RGB pixel data.
        /// </summary>
        /// <param name="buffer">An array of bytes representing the RGB pixel data. This should contain 3 bytes (one each for red, green, and blue) for each pixel in the image.</param>
        /// <param name="width">The width of the image, in pixels.</param>
        /// <param name="height">The height of the image, in pixels.</param>
        /// <returns>An AnyBitmap object that represents the image defined by the provided pixel data, width, and height.</returns>
        public static AnyBitmap LoadAnyBitmapFromRGBBuffer(byte[] buffer, int width, int height)
        {
            return new AnyBitmap(buffer, width, height);
        }

        //cache
        private int? _bitsPerPixel = null;
        /// <summary>
        /// Gets colors depth, in number of bits per pixel.
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/get-color-depth/">
        /// Code Example</a></para>
        /// </summary>
        public int BitsPerPixel => _bitsPerPixel ??= GetFirstInternalImage().PixelType.BitsPerPixel;

        //cache
        private int? _frameCount = null;
        /// <summary>
        /// Returns the number of frames in our loaded Image.  Each “frame” is
        /// a page of an image such as  Tiff or Gif.  All other image formats 
        /// return 1.
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/get-number-of-frames-in-anybitmap/">
        /// Code Example</a></para>
        /// </summary>
        /// <seealso cref="GetAllFrames" />
        public int FrameCount
        {
            get
            {
                if (!_frameCount.HasValue)
                {
                    var images = GetInternalImages();
                    _frameCount = images.Count == 1 ? images[0].Frames.Count : images.Count;
                }
                return _frameCount.Value;
            }
        }

        /// <summary>
        /// Returns all of the frames in our loaded Image. Each "frame" 
        /// is a page of an image such as Tiff or Gif. All other image formats 
        /// return an IEnumerable of length 1.
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/get-frame-from-anybitmap/">
        /// Code Example</a></para>
        /// </summary>
        /// <seealso cref="FrameCount" />
        /// <seealso cref="System.Linq" />
        public IEnumerable<AnyBitmap> GetAllFrames
        {
            get
            {
                var images = GetInternalImages();
                if (images.Count == 1)
                {
                    return ImageFrameCollectionToImages(images[0].Frames).Select(x => (AnyBitmap)x);
                }
                else
                {
                    return images.Select(x => (AnyBitmap)x);
                }
            }
        }

        /// <summary>
        /// Creates a multi-frame TIFF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and height.</para>
        /// <para>The image dimension still the same with original dimension 
        /// with black background.</para>
        /// </summary>
        /// <param name="imagePaths">Array of fully qualified file path to merge
        /// into Tiff image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameTiff(IEnumerable<string> imagePaths)
        {
            using var stream = new MemoryStream();
            InternalSaveAsMultiPageTiff(imagePaths.Select(Image.Load), stream);
            return FromStream(stream);
        }

        /// <summary>
        /// Creates a multi-frame TIFF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and 
        /// height.</para>
        /// <para>The image dimension still the same with original dimension 
        /// with black background.</para>
        /// </summary>
        /// <param name="images">Array of <see cref="AnyBitmap"/> to merge into
        /// Tiff image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameTiff(IEnumerable<AnyBitmap> images)
        {
            using var stream = new MemoryStream();
            InternalSaveAsMultiPageTiff(images.Select(x => (Image)x), stream);
            return FromStream(stream);
        }

        /// <summary>
        /// Creates a multi-frame GIF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and 
        /// height.</para>
        /// <para>The image dimension still the same with original dimension
        /// with background transparent.</para>
        /// </summary>
        /// <param name="imagePaths">Array of fully qualified file path to merge
        /// into Gif image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameGif(IEnumerable<string> imagePaths)
        {
            using var stream = new MemoryStream();
            InternalSaveAsMultiPageGif(imagePaths.Select(Image.Load), stream);
            return FromStream(stream);
        }

        /// <summary>
        /// Creates a multi-frame GIF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and 
        /// height.</para>
        /// <para>The image dimension still the same with original dimension 
        /// with background transparent.</para>
        /// </summary>
        /// <param name="images">Array of <see cref="AnyBitmap"/> to merge into
        /// Gif image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameGif(IEnumerable<AnyBitmap> images)
        {
            using var stream = new MemoryStream();
            InternalSaveAsMultiPageGif(images.Select(x => (Image)x), stream);
            return FromStream(stream);
        }

        /// <summary>
        /// Extracts the alpha channel data from an image.
        /// </summary>
        /// <returns>An array of bytes representing the alpha values of the image's pixels.</returns>
        /// <exception cref="NotSupportedException">Thrown when the image's bit depth is not 32 bpp.</exception>
        public byte[] ExtractAlphaData()
        {

            var alpha = new byte[Width * Height];

            switch (GetFirstInternalImage())
            {
                case Image<Rgba32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < accessor.Width; x++)
                            {
                                alpha[y * accessor.Width + x] = pixelRow[x].A;
                            }
                        }
                    });
                    break;
                case Image<Abgr32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Abgr32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < accessor.Width; x++)
                            {
                                alpha[y * accessor.Width + x] = pixelRow[x].A;
                            }
                        }
                    });
                    break;
                case Image<Argb32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Argb32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < accessor.Width; x++)
                            {
                                alpha[y * accessor.Width + x] = pixelRow[x].A;
                            }
                        }
                    });
                    break;
                case Image<Bgra32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Bgra32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < accessor.Width; x++)
                            {
                                alpha[y * accessor.Width + x] = pixelRow[x].A;
                            }
                        }
                    });
                    break;
                case Image<Rgba64> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba64> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < accessor.Width; x++)
                            {
                                alpha[y * accessor.Width + x] = pixelRow[x].ToRgba32().A;
                            }
                        }
                    });
                    break;
                default:
                    throw new NotSupportedException($"Extracting alpha data is not supported for {BitsPerPixel} bpp images.");
            }

            return alpha.ToArray();
        }

        /// <summary>
        /// Specifies how much an <see cref="AnyBitmap"/> is rotated and the 
        /// axis used to flip the image.
        /// </summary>
        /// <param name="rotateMode">Provides enumeration over how the image 
        /// should be rotated.</param>
        /// <param name="flipMode">Provides enumeration over how a image 
        /// should be flipped.</param>
        /// <returns>Transformed image</returns>
        public AnyBitmap RotateFlip(RotateMode rotateMode, FlipMode flipMode)
        {
            return RotateFlip(this, rotateMode, flipMode);
        }

        /// <summary>
        /// Specifies how much an <see cref="AnyBitmap"/> is rotated and the 
        /// axis used to flip the image.
        /// </summary>
        /// <param name="rotateFlipType">Provides enumeration over how the 
        /// image should be rotated.</param>
        /// <returns>Transformed image</returns>
        [Obsolete("The parameter type RotateFlipType is legacy support from " +
            "System.Drawing. Please use RotateMode and FlipMode instead.")]
        public AnyBitmap RotateFlip(RotateFlipType rotateFlipType)
        {
            (RotateMode rotateMode, FlipMode flipMode) = ParseRotateFlipType(rotateFlipType);
            return RotateFlip(this, rotateMode, flipMode);
        }

        /// <summary>
        /// Specifies how much an image is rotated and the axis used to flip 
        /// the image.
        /// </summary>
        /// <param name="bitmap">The <see cref="AnyBitmap"/> to perform the 
        /// transformation on.</param>
        /// <param name="rotateMode">Provides enumeration over how the image 
        /// should be rotated.</param>
        /// <param name="flipMode">Provides enumeration over how a image 
        /// should be flipped.</param>
        /// <returns>Transformed image</returns>
        public static AnyBitmap RotateFlip(
            AnyBitmap bitmap,
            RotateMode rotateMode,
            FlipMode flipMode)
        {
            SixLabors.ImageSharp.Processing.RotateMode rotateModeImgSharp = rotateMode switch
            {
                RotateMode.None => SixLabors.ImageSharp.Processing.RotateMode.None,
                RotateMode.Rotate90 => SixLabors.ImageSharp.Processing.RotateMode.Rotate90,
                RotateMode.Rotate180 => SixLabors.ImageSharp.Processing.RotateMode.Rotate180,
                RotateMode.Rotate270 => SixLabors.ImageSharp.Processing.RotateMode.Rotate270,
                _ => throw new NotImplementedException()
            };

            SixLabors.ImageSharp.Processing.FlipMode flipModeImgSharp = flipMode switch
            {
                FlipMode.None => SixLabors.ImageSharp.Processing.FlipMode.None,
                FlipMode.Horizontal => SixLabors.ImageSharp.Processing.FlipMode.Horizontal,
                FlipMode.Vertical => SixLabors.ImageSharp.Processing.FlipMode.Vertical,
                _ => throw new NotImplementedException()
            };

            Image image = Image.Load(bitmap.Binary);

            image.Mutate(x => x.RotateFlip(rotateModeImgSharp, flipModeImgSharp));

            return new AnyBitmap(image);
        }

        /// <summary>
        /// Creates a new bitmap with the region defined by the specified
        /// rectangle redacted with the specified color.
        /// </summary>
        /// <param name="Rectangle">The rectangle defining the region
        /// to redact.</param>
        /// <param name="color">The color to use for redaction.</param>
        /// <returns>A new bitmap with the specified region redacted.</returns>
        public AnyBitmap Redact(Rectangle Rectangle, Color color)
        {
            return Redact(this, Rectangle, color);
        }

        /// <summary>
        /// Creates a new bitmap with the region defined by the specified
        /// rectangle in the specified bitmap redacted with the specified color.
        /// </summary>
        /// <param name="bitmap">The bitmap to redact.</param>
        /// <param name="Rectangle">The rectangle defining the region
        /// to redact.</param>
        /// <param name="color">The color to use for redaction.</param>
        /// <returns>A new bitmap with the specified region redacted.</returns>
        public static AnyBitmap Redact(
            AnyBitmap bitmap,
            Rectangle Rectangle,
            Color color)
        {

            //this casting will crate new object
            Image image = Image.Load(bitmap.Binary);
            Rectangle rectangle = Rectangle;
            var brush = new SolidBrush(color);
            image.Mutate(ctx => ctx.Fill(brush, rectangle));
       
            return new AnyBitmap(image);
        }

        /// <summary>
        /// Gets the stride width (also called scan width) of the 
        /// <see cref="AnyBitmap"/> object.
        /// </summary>
        public int Stride
        {
            get
            {
                return GetStride();
            }
        }

        /// <summary>
        /// Gets the address of the first pixel data in the 
        /// <see cref="AnyBitmap"/>. This can also be thought of as the first 
        /// scan line in the <see cref="AnyBitmap"/>.
        /// </summary>
        /// <returns>The address of the first 32bpp BGRA pixel data in the 
        /// <see cref="AnyBitmap"/>.</returns>
        public IntPtr Scan0
        {
            get
            {
                return GetFirstPixelData();
            }
        }

        /// <summary>
        /// Returns the 
        /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types">
        /// HTTP MIME types</see>
        /// of the image. 
        /// <para>must be one of the following: image/bmp, image/jpeg, 
        /// image/png, image/gif, image/tiff, image/webp, or image/unknown.</para>
        /// </summary>
        public string MimeType
        {
            get
            {
                return Format?.DefaultMimeType ?? "image/unknown";
            }
        }

        /// <summary>
        /// Image formats which <see cref="AnyBitmap"/> readed.
        /// </summary>
        /// <returns><see cref="ImageFormat"/></returns>
        public ImageFormat GetImageFormat()
        {
            return (Format?.DefaultMimeType) switch
            {
                "image/gif" => ImageFormat.Gif,
                "image/tiff" => ImageFormat.Tiff,
                "image/jpeg" => ImageFormat.Jpeg,
                "image/png" => ImageFormat.Png,
                "image/webp" => ImageFormat.Webp,
                "image/vnd.microsoft.icon" => ImageFormat.Icon,
                _ => ImageFormat.Bmp,
            };
        }

        /// <summary>
        /// Gets the resolution of the image in x-direction.
        /// </summary>
        /// <returns></returns>
        public double? HorizontalResolution
        {
            get
            {
                return GetFirstInternalImage().Metadata.HorizontalResolution;
            }
        }

        /// <summary>
        /// Gets the resolution of the image in y-direction.
        /// </summary>
        /// <returns></returns>
        public double? VerticalResolution
        {
            get
            {
                return GetFirstInternalImage().Metadata.VerticalResolution;
            }
        }

        /// <summary>
        /// Gets the <see cref="Color"/> of the specified pixel in this 
        /// <see cref="AnyBitmap"/>
        /// <para>This always return an Rgba32 color format.</para>
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to retrieve.</param>
        /// <param name="y">The y-coordinate of the pixel to retrieve.</param>
        /// <returns>A <see cref="Color"/> structure that represents the color 
        /// of the specified pixel.</returns>
        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x),
                    "x is less than 0, or greater than or equal to Width.");
            }

            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y),
                    "y is less than 0, or greater than or equal to Height.");
            }

            return GetPixelColor(x, y);
        }

        /// <summary>
        /// Sets the <see cref="Color"/> of the specified pixel in this <see cref="AnyBitmap"/>
        /// <para>Performs an operation that modifies the current object. (mutable)</para>
        /// <para>Set in Rgb24 color format.</para>
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel to retrieve.</param>
        /// <param name="y">The y-coordinate of the pixel to retrieve.</param>
        /// <param name="color">The color to set the pixel.</param>
        /// <returns>void</returns>
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x),
                    "x is less than 0, or greater than or equal to Width.");
            }

            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y),
                    "y is less than 0, or greater than or equal to Height.");
            }
            SetPixelColor(x, y, color);
        }

        /// <summary>
        /// Retrieves the RGB buffer from the image at the specified path.
        /// </summary>
        /// <returns>An array of bytes representing the RGB buffer of the image.</returns>
        /// <remarks>
        /// Each pixel is represented by three bytes in the order: red, green, blue.
        /// The pixels are read from the image row by row, from top to bottom and left to right within each row.
        /// </remarks>
        public byte[] GetRGBBuffer()
        {
            var image = GetFirstInternalImage();
            int width = image.Width;
            int height = image.Height;
            byte[] rgbBuffer = new byte[width * height * 3]; // 3 bytes per pixel (RGB)
            switch (image)
            {
                case Image<Rgba32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Rgba32 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Rgb24> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Rgb24 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Abgr32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Abgr32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Abgr32 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Argb32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Argb32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Argb32 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Bgr24> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Bgr24> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Bgr24 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Bgra32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Bgra32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Bgra32 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Rgb48> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgb48> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                //required casting in 16bit color
                                Color pixel = (Color)pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                case Image<Rgba64> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba64> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                //required casting in 16bit color
                                Color pixel = (Color)pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    break;
                default:
                    var clonedImage = image.CloneAs<Rgb24>();
                    clonedImage.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Rgb24 pixel = pixelRow[x];
                                int index = (y * width + x) * 3;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                            }
                        }
                    });
                    clonedImage.Dispose();
                    break;
            }
            return rgbBuffer;
        }

        /// <summary>
        /// Retrieves the RGBA buffer from the image at the specified path.
        /// </summary>
        /// <returns>An array of bytes representing the RGBA buffer of the image.</returns>
        /// <remarks>
        /// Each pixel is represented by four bytes in the order: red, green, blue, alpha.
        /// The pixels are read from the image row by row, from top to bottom and left to right within each row.
        /// </remarks>
        public byte[] GetRGBABuffer()
        {
            var image = GetFirstInternalImage();
            int width = image.Width;
            int height = image.Height;
            byte[] rgbBuffer = new byte[width * height * 4]; // 3 bytes per pixel (RGB)
            switch (image)
            {
                case Image<Rgba32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Rgba32 pixel = pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = pixel.A;
                            }
                        }
                    });
                    break;
                case Image<Rgb24> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Rgb24 pixel = pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = byte.MaxValue;
                            }
                        }
                    });
                    break;
                case Image<Abgr32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Abgr32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Abgr32 pixel = pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = pixel.A;
                            }
                        }
                    });
                    break;
                case Image<Argb32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Argb32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Argb32 pixel = pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = pixel.A;
                            }
                        }
                    });
                    break;
                case Image<Bgr24> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Bgr24> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Bgr24 pixel = pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = byte.MaxValue;
                            }
                        }
                    });
                    break;
                case Image<Bgra32> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Bgra32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                Bgra32 pixel = pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = pixel.A;
                            }
                        }
                    });
                    break;
                case Image<Rgb48> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgb48> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                //required casting in 16bit color
                                Color pixel = (Color)pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = pixel.A;
                            }
                        }
                    });
                    break;
                case Image<Rgba64> imageAsFormat:
                    imageAsFormat.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba64> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < width; x++)
                            {
                                //required casting in 16bit color
                                Color pixel = (Color)pixelRow[x];
                                int index = (y * width + x) * 4;

                                rgbBuffer[index] = pixel.R;
                                rgbBuffer[index + 1] = pixel.G;
                                rgbBuffer[index + 2] = pixel.B;
                                rgbBuffer[index + 3] = pixel.A;
                            }
                        }
                    });
                    break;
                default:
                    using (var clonedImage = image.CloneAs<Rgba32>())
                    {
                        clonedImage.ProcessPixelRows(accessor =>
                        {
                            for (int y = 0; y < accessor.Height; y++)
                            {
                                Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                                for (int x = 0; x < width; x++)
                                {
                                    Rgba32 pixel = pixelRow[x];
                                    int index = (y * width + x) * 4;

                                    rgbBuffer[index] = pixel.R;
                                    rgbBuffer[index + 1] = pixel.G;
                                    rgbBuffer[index + 2] = pixel.B;
                                    rgbBuffer[index + 3] = pixel.A;
                                }
                            }
                        });
                    }
                    break;
            }
            return rgbBuffer;
        }

        #region Implicit Casting

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Image objects to 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support ImageSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original SixLabors.ImageSharp.Image object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">SixLabors.ImageSharp.Image will automatically 
        /// be casted to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(Image<Rgb24> image)
        {
            try
            {
                return new AnyBitmap(image);

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap from SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Since we store ImageSharp object internal AnyBitmap (lazy) so this casting will return the same ImageSharp object if it loaded.
        /// But if it is gif/tiff we need to make resize all frame to have the same size before we load to ImageSharp object;
        /// </summary>
        private static Image CastToImageSharp(AnyBitmap bitmap)
        {
            try
            {
                if (!bitmap.IsImageLoaded())
                {
                    var format = bitmap.Format;
                    if (format is not TiffFormat && format is not GifFormat)
                    {
                        return Image.Load(bitmap.Binary);
                    }
                }

                //if it is loaded or gif/tiff
                var images = bitmap.GetInternalImages();
                if (images.Count == 1)
                {
                    //not gif/tiff
                    return images[0];
                }
                else
                {
                    //for gif/tiff we need to resize all frame
                    //Tiff can have different frame size but ImageSharp does not support
                    var resultImage = images[0].Clone((_) => { });

                    foreach (var frame in images.Skip(1))
                    {
                        var newFrame = frame.Clone(x =>
                        {
                            x.Resize(new ResizeOptions
                            {
                                Size = new Size(resultImage.Width, resultImage.Height),
                                Mode = ResizeMode.BoxPad, // Pad to fit the target dimensions
                                PadColor = Color.Transparent, // Use transparent padding
                                Position = AnchorPositionMode.Center // Center the image within the frame
                            });
                        });

                        resultImage.Frames.AddFrame(newFrame.Frames.RootFrame);
                    }

                    return resultImage;
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Since we store ImageSharp object internal AnyBitmap (lazy) so this casting will return the same ImageSharp object if it loaded.
        /// But if it is gif/tiff we need to make resize all frame to have the same size before we load to ImageSharp object;
        /// </summary>
        private static Image<T> CastToImageSharp<T>(AnyBitmap bitmap) where T :unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<T>
        {
            try
            {
                if (!bitmap.IsImageLoaded())
                {
                    var format = bitmap.Format;
                    if (format is not TiffFormat && format is not GifFormat)
                    {
                        return Image.Load<T>(bitmap.Binary);
                    }
                   
                }
              
                var images = bitmap.GetInternalImages();
                if (images.Count == 1)
                {
                    if (images[0] is Image<T> correctType)
                    {
                        return correctType;
                    }
                    else
                    {
                        return images[0].CloneAs<T>();
                    }
                }
                else
                {
                    var resultImage = images[0].CloneAs<T>();

                    //for gif/tiff we need to resize all frame
                    //Tiff can have different frame size but ImageSharp does not support
                    foreach (var frame in images.Skip(1))
                    {
                        var newFrame = frame.CloneAs<T>();

                        newFrame.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(resultImage.Width, resultImage.Height),
                            Mode = ResizeMode.BoxPad, // Pad to fit the target dimensions
                            PadColor = Color.Transparent, // Use transparent padding
                            Position = AnchorPositionMode.Center // Center the image within the frame
                        }));
                        resultImage.Frames.AddFrame(newFrame.Frames.RootFrame);
                    }

                    return resultImage;
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Image objects from 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> 
        /// as parameters or return types, you now automatically support 
        /// ImageSharp as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to 
        /// a SixLabors.ImageSharp.Image.</param>
        public static implicit operator Image<Rgb24>(AnyBitmap bitmap)
        {
            return CastToImageSharp<Rgb24>(bitmap);
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Image objects to 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support ImageSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original SixLabors.ImageSharp.Image object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">SixLabors.ImageSharp.Image will automatically be
        /// cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(Image<Rgba32> image)
        {
            try
            {
                return new AnyBitmap(image);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap from SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Image objects from 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support ImageSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to
        /// a SixLabors.ImageSharp.Image.</param>
        public static implicit operator Image<Rgba32>(AnyBitmap bitmap)
        {
            try
            {
                return CastToImageSharp<Rgba32>(bitmap);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Image objects to 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support ImageSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original SixLabors.ImageSharp.Image object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">SixLabors.ImageSharp.Image will automatically
        /// be casted to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(Image image)
        {
            try
            {
                return new AnyBitmap(image);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap from SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Image objects from 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support ImageSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to
        /// a SixLabors.ImageSharp.Image.</param>
        public static implicit operator Image(AnyBitmap bitmap)
        {
            try
            {
                return CastToImageSharp(bitmap);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKImage objects to 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as
        /// parameters or return types, you now automatically support SkiaSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original SkiaSharp.SKImage object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">SkiaSharp.SKImage will automatically be casted to
        /// <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SKImage image)
        {
            try
            {
                return new AnyBitmap(
                    image.Encode(SKEncodedImageFormat.Png, 100)
                    .ToArray());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap from SkiaSharp", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKImage objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// SkiaSharp.SKImage as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to 
        /// a SkiaSharp.SKImage.</param>
        public static implicit operator SKImage(AnyBitmap bitmap)
        {
            try
            {
                SKImage result = null;
                try
                {
                    result = SKImage.FromBitmap(SKBitmap.Decode(bitmap.Binary));
                }
                catch
                {
                    // Exception can be ignored here because the input image may be in TIFF format.
                    // TIFF format images are handled elsewhere after this catch block. Therefore, 
                    // it's expected that decoding may fail here and it's safe to continue execution.
                }

                if (result != null)
                {
                    return result;
                }

                return OpenTiffToSKImage(bitmap);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap to SkiaSharp", e);
            }
        }
        /// <summary>
        /// Implicitly casts SkiaSharp.SKBitmap objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as
        /// parameters or return types, you now automatically support SkiaSharp
        /// as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original SkiaSharp.SKBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">SkiaSharp.SKBitmap will automatically be casted
        /// to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SKBitmap image)
        {
            try
            {
                return new AnyBitmap(
                    image.Encode(SKEncodedImageFormat.Png, 100)
                    .ToArray());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap from SkiaSharp", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKBitmap objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// SkiaSharp.SKBitmap as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is explicitly cast to 
        /// a SkiaSharp.SKBitmap.</param>
        public static implicit operator SKBitmap(AnyBitmap bitmap)
        {
            try
            {
                SKBitmap result = null;
                try
                {
                    result = SKBitmap.Decode(bitmap.Binary);
                }
                catch
                {
                    // Exception can be ignored here because the input image may be in TIFF format.
                    // TIFF format images are handled elsewhere after this catch block. Therefore, 
                    // it's expected that decoding may fail here and it's safe to continue execution.
                }

                if (result != null)
                {
                    return result;
                }

                return OpenTiffToSKBitmap(bitmap);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap to SkiaSharp", e);
            }
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Platform.PlatformImage 
        /// objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// Microsoft.Maui.Graphics as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original Microsoft.Maui.Graphics.Platform.PlatformImage object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">Microsoft.Maui.Graphics.Platform.PlatformImage 
        /// will automatically be casted to <see cref="AnyBitmap"/>.</param>

        public static implicit operator AnyBitmap(PlatformImage image)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream);
                return new AnyBitmap(memoryStream.ToArray());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install Microsoft.Maui.Graphics from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while casting AnyBitmap from Microsoft.Maui.Graphics", e);
            }
        }
        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.Platform.PlatformImage 
        /// objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// Microsoft.Maui.Graphics as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to 
        /// a Microsoft.Maui.Graphics.Platform.PlatformImage.</param>

        public static implicit operator PlatformImage(AnyBitmap bitmap)
        {
            try
            {
                return (PlatformImage)PlatformImage.FromStream(bitmap.GetStream());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install Microsoft.Maui.Graphics from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while casting AnyBitmap to Microsoft.Maui.Graphics", e);
            }
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Bitmap objects to 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// System.Drawing.Common as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original System.Drawing.Bitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">System.Drawing.Bitmap will automatically be casted to <see cref="AnyBitmap"/> </param>
        public static implicit operator AnyBitmap(System.Drawing.Bitmap image)
        {
            byte[] data;
            try
            {
                System.Drawing.Bitmap blank = new(image.Width, image.Height);
                var g = System.Drawing.Graphics.FromImage(blank);
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
                g.Dispose();

                System.Drawing.Bitmap tempImage = new(blank);
                blank.Dispose();

                System.Drawing.Imaging.ImageFormat imageFormat =
                    GetMimeType(image) != "image/unknown"
                    ? image.RawFormat
                    : System.Drawing.Imaging.ImageFormat.Bmp;

                using var memoryStream = new MemoryStream();
                tempImage.Save(memoryStream, imageFormat);
                tempImage.Dispose();

                data = memoryStream.ToArray();
                return new AnyBitmap(data);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException or TypeInitializationException)
                {
#if NETSTANDARD
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }

                throw;
            }
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Bitmap objects from 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// System.Drawing.Common as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object 
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to
        /// a System.Drawing.Bitmap.</param>
        public static implicit operator System.Drawing.Bitmap(AnyBitmap bitmap)
        {
            try
            {
                return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(bitmap.GetStream());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException or TypeInitializationException)
                {
#if NETSTANDARD
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }

                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Image objects to
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as 
        /// parameters or return types, you now automatically support 
        /// System.Drawing.Common as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original System.Drawing.Image object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="image">System.Drawing.Image will automatically be casted
        /// to <see cref="AnyBitmap"/> </param>
        public static implicit operator AnyBitmap(System.Drawing.Image image)
        {
            byte[] data;
            try
            {
                System.Drawing.Bitmap blank = new(image.Width, image.Height);
                var g = System.Drawing.Graphics.FromImage(blank);
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
                g.Dispose();

                System.Drawing.Bitmap tempImage = new(blank);
                blank.Dispose();

                System.Drawing.Imaging.ImageFormat imageFormat =
                    GetMimeType(image) != "image/unknown"
                    ? image.RawFormat
                    : System.Drawing.Imaging.ImageFormat.Bmp;
                using var memoryStream = new MemoryStream();
                tempImage.Save(memoryStream, imageFormat);
                tempImage.Dispose();

                data = memoryStream.ToArray();
                return new AnyBitmap(data);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException or TypeInitializationException)
                {
#if NETSTANDARD
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }

                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Image objects from 
        /// <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as
        /// parameters or return types, you now automatically support 
        /// System.Drawing.Common as well.</para>
        /// <para>When casting to and from AnyBitmap, 
        /// please remember to dispose your original IronSoftware.Drawing.AnyBitmap object
        /// to avoid unnecessary memory allocation.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to 
        /// a System.Drawing.Image.</param>
        public static implicit operator System.Drawing.Image(AnyBitmap bitmap)
        {
            try
            {
                return System.Drawing.Image.FromStream(bitmap.GetStream());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException or TypeInitializationException)
                {
#if NETSTANDARD
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }

                throw new Exception(e.Message, e);
            }
        }

        #endregion

        /// <summary>
        /// AnyBitmap destructor
        /// </summary>
        ~AnyBitmap()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by this <see cref="AnyBitmap"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this <see cref="AnyBitmap"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_disposed)
                {
                    return;
                }
                if (IsImageLoaded())
                {
                    foreach (var x in GetInternalImages() ?? [])
                    {
                        x.Dispose();
                    }
                }
                _lazyImage = null;
                Binary = null;
                _disposed = true;
            }
        }

        #region Private Method

        private void LoadImage(Stream stream, bool preserveOriginalFormat)
        {
            // Optimization 1: If the stream is already a MemoryStream, we can get its
            // underlying array directly, avoiding a full copy cycle.
            if (stream is MemoryStream memoryStream)
            {
                LoadImage(memoryStream.ToArray(), preserveOriginalFormat);
                return;
            }

            // Optimization 2: If the stream can report its length (like a FileStream),
            // we can create a MemoryStream with the exact capacity needed. This avoids
            // multiple buffer re-allocations as the MemoryStream grows.
            if (stream.CanSeek)
            {
                // Ensure we read from the beginning of the stream.
                stream.Position = 0;
                using (var ms = new MemoryStream((int)stream.Length))
                {
                    stream.CopyTo(ms, 16 * 1024);
                    LoadImage(ms.ToArray(), preserveOriginalFormat);
                    return;
                }
            }

            // Fallback for non-seekable streams (e.g., a network stream).
            // This is the most memory-intensive path, but necessary for this stream type.
            // We use CopyTo for a cleaner implementation of the original logic.
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms, 16 * 1024);
                LoadImage(ms.ToArray(), preserveOriginalFormat);
            }


        }

        /// <summary>
        /// Master LoadImage method
        /// </summary>
        /// <param name="span"></param>
        /// <param name="preserveOriginalFormat"></param>
        private void LoadImage(ReadOnlySpan<byte> span, bool preserveOriginalFormat)
        {
            Binary = span.ToArray();
            if (Format is TiffFormat) 
            {
                if(GetTiffFrameCountFast() > 1)
                {
                    _lazyImage = OpenTiffToImageSharp();
                }
                else
                {
                    // ImageSharp can load some single frame tiff, if failed we try again with LibTiff
                    _lazyImage = OpenImageToImageSharp(preserveOriginalFormat, tryWithLibTiff : true);
                }
              
            }
            else
            {
                _lazyImage = OpenImageToImageSharp(preserveOriginalFormat);
            }
        }

        private IEnumerable<Image> ImageFrameCollectionToImages(ImageFrameCollection imageFrames)
        {
            for (int i = 0; i < imageFrames.Count; i++)
            {
                yield return imageFrames.CloneFrame(i);
            }
        }

        private static AnyBitmap LoadSVGImage(string file, bool preserveOriginalFormat)
        {
            try

            {
                return new AnyBitmap(
                    DecodeSVG(file).Encode(SKEncodedImageFormat.Png, 100)
                    .ToArray(), preserveOriginalFormat);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException(
                    "Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(
                    "Error while reading SVG image format.", e);
            }
        }

        private static SKBitmap DecodeSVG(string strInput)
        {
            try
            {
                SkiaSharp.Extended.Svg.SKSvg svg = new();
                _ = svg.Load(strInput);

                SKBitmap toBitmap = new(
                    (int)svg.Picture.CullRect.Width,
                    (int)svg.Picture.CullRect.Height);
                using (SKCanvas canvas = new(toBitmap))
                {
                    canvas.Clear(SKColors.White);
                    canvas.DrawPicture(svg.Picture);
                    canvas.Flush();
                }

                return toBitmap;

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp.Svg " +
                    "from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while reading SVG image format.", e);
            }
        }

        private static PlatformNotSupportedException SystemDotDrawingPlatformNotSupported(Exception innerException)
        {
            return new PlatformNotSupportedException($"Microsoft has chosen " +
                $"to no longer support System.Drawing.Common on Linux or MacOS. " +
                $"To solve this please use another Bitmap type such as " +
                $"{typeof(System.Drawing.Bitmap)}, " +
                $"SkiaSharp or ImageSharp.\n\n" +
                $"https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only",
                innerException);
        }

        private static string GetMimeType(System.Drawing.Bitmap image)
        {
            Guid imgguid = image.RawFormat.Guid;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == imgguid)
                {
                    return codec.MimeType;
                }
            }

            return "image/unknown";
        }

        private static string GetMimeType(System.Drawing.Image image)
        {
            Guid imgguid = image.RawFormat.Guid;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == imgguid)
                {
                    return codec.MimeType;
                }
            }

            return "image/unknown";
        }

        private static SKImage OpenTiffToSKImage(AnyBitmap anyBitmap)
        {
            SKBitmap skBitmap = OpenTiffToSKBitmap(anyBitmap);
            if (skBitmap != null)
            {
                return SKImage.FromBitmap(skBitmap);
            }

            return null;
        }

        private static SKBitmap OpenTiffToSKBitmap(AnyBitmap anyBitmap)
        {
            try
            {
                // create a memory stream out of them
                using MemoryStream tiffStream = new(anyBitmap.Binary);

                // open a TIFF stored in the stream
                using var tifImg = Tiff.ClientOpen("in-memory", "r", tiffStream, new TiffStream());

                // read the dimensions
                int width = tifImg.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = tifImg.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

                // create the bitmap
                var bitmap = new SKBitmap();
                var info = new SKImageInfo(width, height);

                // create the buffer that will hold the pixels
                int[] raster = new int[width * height];

                // get a pointer to the buffer, and give it to the bitmap
                var ptr = GCHandle.Alloc(raster, GCHandleType.Pinned);
                _ = bitmap.InstallPixels(info, ptr.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) => ptr.Free(), null);

                // read the image into the memory buffer
                if (!tifImg.ReadRGBAImageOriented(width, height, raster, Orientation.TOPLEFT))
                {
                    // not a valid TIF image.
                    return null;
                }

                // swap the red and blue because SkiaSharp may differ from the tiff
                if (SKImageInfo.PlatformColorType == SKColorType.Bgra8888)
                {
                    SKSwizzle.SwapRedBlue(ptr.AddrOfPinnedObject(), raster.Length);
                }

                return bitmap;

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install BitMiracle.LibTiff.NET from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while reading TIFF image format.", e);
            }
        }

        /// <summary>
        /// Disable warning message written to console by BitMiracle.LibTiff.NET.
        /// </summary>
        private class DisableErrorHandler : TiffErrorHandler
        {
            public override void WarningHandler(Tiff tif, string method, string format, params object[] args)
            {
                // do nothing, ie, do not write warnings to console
            }
            public override void WarningHandlerExt(Tiff tif, object clientData, string method, string format, params object[] args)
            {
                // do nothing ie, do not write warnings to console
            }
        }

        private int GetTiffFrameCountFast()
        {
            try
            {
                using var tiffStream = new MemoryStream(Binary);

                // Disable error messages for fast check
                Tiff.SetErrorHandler(new DisableErrorHandler());

                using var tiff = Tiff.ClientOpen("in-memory", "r", tiffStream, new TiffStream());
                if (tiff == null) return 1; // Default to single frame if can't read

                return tiff.NumberOfDirectories();
            }
            catch
            {
                return 1; // Default to single frame on any error
            }
        }

        private Lazy<IReadOnlyList<Image>> OpenTiffToImageSharp()
        {
            return new Lazy<IReadOnlyList<Image>>(() =>
            {
                try
                {
                    return InternalLoadTiff();
                }
                catch (DllNotFoundException e)
                {
                    throw new DllNotFoundException("Please install BitMiracle.LibTiff.NET from NuGet.", e);
                }
                catch (Exception e)
                {
                    throw new NotSupportedException("Error while reading TIFF image format.", e);
                }
            });
        }

        private IReadOnlyList<Image> InternalLoadTiff()
        {
            int imageWidth = 0;
            int imageHeight = 0;
            double imageXResolution = 0;
            double imageYResolution = 0;
            //IEnumerable<Image> images = new();

            // create a memory stream out of them
            using MemoryStream tiffStream = new(Binary);

            // Disable warning messages
            Tiff.SetErrorHandler(new DisableErrorHandler());
            List<Image> images = new();
            // open a TIFF stored in the stream
            using (Tiff tiff = Tiff.ClientOpen("in-memory", "r", tiffStream, new TiffStream()))
            {
                SetTiffCompression(tiff);

                short num = tiff.NumberOfDirectories();
                for (short i = 0; i < num; i++)
                {
                    _ = tiff.SetDirectory(i);

                    if (IsThumbnail(tiff))
                    {
                        continue;
                    }

                    var (width, height, horizontalResolution, verticalResolution) = SetWidthHeight(tiff, i, ref imageWidth, ref imageHeight, ref imageXResolution, ref imageYResolution);

                    // Read the image into the memory buffer
                    int[] raster = new int[height * width];
                    if (!tiff.ReadRGBAImage(width, height, raster))
                    {
                        throw new NotSupportedException("Could not read image");
                    }

                    var bits = PrepareByteArray(raster, width, height, 32);
                    
                   var image = Image.LoadPixelData<Rgba32>(bits, width, height);

                    image.Metadata.HorizontalResolution = horizontalResolution;
                    image.Metadata.VerticalResolution = verticalResolution;
                    images.Add(image);

                    //Note1: it might be some case that the bytes of current Image is smaller/bigger than the original tiff
                    //Note2: 'yield return' make it super slow
                }

            }
            return images;
        }

        private Lazy<IReadOnlyList<Image>> OpenImageToImageSharp(bool preserveOriginalFormat, bool tryWithLibTiff = false)
        {
            return new Lazy<IReadOnlyList<Image>>(() =>
            {
                try
                {
                    Image img;
                    if (preserveOriginalFormat)
                    {
                        img = Image.Load(Binary);
                    }
                    else
                    {
                        PreserveOriginalFormat = preserveOriginalFormat;
                        img = Image.Load<Rgba32>(Binary);
                        if (Format.Name == "PNG")
                            img.Mutate(img => img.BackgroundColor(SixLabors.ImageSharp.Color.White));
                    }

                    CorrectImageSharp(img);

                    return [img];
                }
                catch (DllNotFoundException e)
                {
                    throw new DllNotFoundException(
                        "Please install SixLabors.ImageSharp from NuGet.", e);
                }
                catch (Exception e)
                {
                    return tryWithLibTiff
                        ? InternalLoadTiff()
                        : throw new NotSupportedException(
                           "Image could not be loaded. File format is not supported.", e);
                }
            });
        }

        private void CorrectImageSharp(Image img)
        {

            // Fix if the input image is auto-rotated; this issue is acknowledged by SixLabors.ImageSharp community
            // ref: https://github.com/SixLabors/ImageSharp/discussions/2685
            img.Mutate(x => x.AutoOrient());

            var resolutionUnit = img.Metadata.ResolutionUnits;
            var horizontal = img.Metadata.HorizontalResolution;
            var vertical = img.Metadata.VerticalResolution;

            // Check if image metadata is accurate already
            switch (resolutionUnit)
            {
                case SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerMeter:
                    // Convert metadata of the resolution unit to pixel per inch to match the conversion below of 1 meter = 37.3701 inches
                    img.Metadata.ResolutionUnits = SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerInch;
                    img.Metadata.HorizontalResolution = Math.Ceiling(horizontal / 39.3701);
                    img.Metadata.VerticalResolution = Math.Ceiling(vertical / 39.3701);
                    break;
                case SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerCentimeter:
                    // Convert metadata of the resolution unit to pixel per inch to match the conversion below of 1 inch = 2.54 centimeters
                    img.Metadata.ResolutionUnits = SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerInch;
                    img.Metadata.HorizontalResolution = Math.Ceiling(horizontal * 2.54);
                    img.Metadata.VerticalResolution = Math.Ceiling(vertical * 2.54);
                    break;
                default:
                    // No changes required due to teh metadata are accurate already
                    break;
            }
        }

        private void SetTiffCompression(Tiff tiff)
        {
            Compression tiffCompression = tiff.GetField(TiffTag.COMPRESSION) != null && tiff.GetField(TiffTag.COMPRESSION).Length > 0
                                                        ? (Compression)tiff.GetField(TiffTag.COMPRESSION)[0].ToInt()
                                                        : Compression.NONE;

            TiffCompression = tiffCompression switch
            {
                Compression.CCITTRLE => TiffCompression.Ccitt1D,
                Compression.CCITTFAX3 => TiffCompression.CcittGroup3Fax,
                Compression.CCITTFAX4 => TiffCompression.CcittGroup4Fax,
                Compression.JPEG => TiffCompression.Jpeg,
                Compression.OJPEG => TiffCompression.OldJpeg,
                Compression.NEXT => TiffCompression.NeXT,
                Compression.PACKBITS => TiffCompression.PackBits,
                Compression.THUNDERSCAN => TiffCompression.ThunderScan,
                Compression.DEFLATE => TiffCompression.Deflate,
                _ => TiffCompression.Lzw
            };
        }

        /// <summary>
        /// Determines if a TIFF frame contains a thumbnail.
        /// </summary>
        /// <param name="tiff">The <see cref="Tiff"/> which set number of directory to analyze.</param>
        /// <returns>True if the frame contains a thumbnail, otherwise false.</returns>
        private bool IsThumbnail(Tiff tiff)
        {
            FieldValue[] subFileTypeFieldValue = tiff.GetField(TiffTag.SUBFILETYPE);

            // Current thumbnail identification relies on the SUBFILETYPE tag with a value of FileType.REDUCEDIMAGE.
            // This may need refinement in the future to include additional checks
            // (e.g., FileType.COMPRESSION is NONE, Image Dimensions).
            return subFileTypeFieldValue != null && subFileTypeFieldValue.Length > 0
                && (FileType)subFileTypeFieldValue[0].Value == FileType.REDUCEDIMAGE;
        }

        private ReadOnlySpan<byte> PrepareByteArray(int[] raster, int width, int height,int bitsPerPixel)
        {
            int stride = 4 * ((width * bitsPerPixel + 31) / 32);

            byte[] bits = new byte[stride * height];

            // If no extra padding exists, copy entire rows at once.
            if (stride == width * 4 && true)
            {
                int bytesPerRow = stride;
                for (int y = 0; y < height; y++)
                {
                    int srcByteIndex = y * bytesPerRow;
                    int destByteIndex = (height - y - 1) * bytesPerRow;
                    Buffer.BlockCopy(raster, srcByteIndex, bits, destByteIndex, bytesPerRow);
                }
            }
            else
            {
                // Fallback to per-pixel processing if stride includes padding.
                for (int y = 0; y < height; y++)
                {
                    int rasterOffset = y * width;
                    int bitsOffset = (height - y - 1) * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int rgba = raster[rasterOffset++];
                        bits[bitsOffset++] = (byte)(rgba & 0xff); // R
                        bits[bitsOffset++] = (byte)((rgba >> 8) & 0xff); // G
                        bits[bitsOffset++] = (byte)((rgba >> 16) & 0xff); // B
                        bits[bitsOffset++] = (byte)((rgba >> 24) & 0xff); // A
                    }
                }
            }

            return bits;
        }

        private (int width, int height, double horizontalResolution, double verticalResolution) SetWidthHeight(Tiff tiff, short index, ref int imageWidth, ref int imageHeight, ref double imageXResolution, ref double imageYResolution)
        {
            // Find the width and height of the image
            FieldValue[] value = tiff.GetField(TiffTag.IMAGEWIDTH);
            int width = value[0].ToInt();

            value = tiff.GetField(TiffTag.IMAGELENGTH);
            int height = value[0].ToInt();

            // If resolutions are null due to damaged files, return the default value of 96
            value = tiff.GetField(TiffTag.XRESOLUTION);
            double horizontalResolution = Math.Floor(value?.FirstOrDefault().ToDouble() ?? 96);

            value = tiff.GetField(TiffTag.YRESOLUTION);
            double verticalResolution = Math.Floor(value?.FirstOrDefault().ToDouble() ?? 96);

            if (index == 0)
            {
                if (width == 0 || height == 0)
                {
                    throw new NotSupportedException("Width or Height of the first image can't be 0.");
                }
                else
                {
                    imageWidth = width;
                    imageHeight = height;
                }
            }
            else
            {
                if (width == 0)
                {
                    width = imageWidth;
                }
                if (height == 0)
                {
                    height = imageHeight;
                }
            }

            return (width, height, horizontalResolution, verticalResolution);
        }

        private static List<AnyBitmap> CreateAnyBitmaps(IEnumerable<string> imagePaths)
        {
            List<AnyBitmap> bitmaps = new();
            foreach (string imagePath in imagePaths)
            {
                bitmaps.Add(FromFile(imagePath));
            }

            return bitmaps;
        }


        private static void FindMaxWidthAndHeight(IEnumerable<Image> images, out int maxWidth, out int maxHeight)
        {
            maxWidth = images.Select(img => img.Width).Max();
            maxHeight = images.Select(img => img.Height).Max();
        }

        private static void FindMaxWidthAndHeight(IEnumerable<AnyBitmap> images, out int maxWidth, out int maxHeight)
        {
            maxWidth = images.Select(img => img.Width).Max();
            maxHeight = images.Select(img => img.Height).Max();
        }

        private int GetStride(Image source = null)
        {
            if (source == null)
            {
                return 4 * (((Width * BitsPerPixel) + 31) / 32);
            }
            else
            {
                return 4 * (((source.Width * source.PixelType.BitsPerPixel) + 31) / 32);
            }
        }

        private IntPtr GetFirstPixelData()
        {
            var image = GetFirstInternalImage();

            if(image is not Image<Rgba32>)
            {
                image = image.CloneAs<Rgba32>();
            }

            Image<Rgba32> rgbaImage = (Image<Rgba32>)image;
            byte[] pixelBytes = new byte[rgbaImage.Width * rgbaImage.Height * Unsafe.SizeOf<Rgba32>()];

            rgbaImage.CopyPixelDataTo(pixelBytes);
            ConvertRGBAtoBGRA(pixelBytes, rgbaImage.Width, rgbaImage.Height);

            IntPtr result = Marshal.AllocHGlobal(pixelBytes.Length);
            Marshal.Copy(pixelBytes, 0, result, pixelBytes.Length);

            return result;
        }

        private static void ConvertRGBAtoBGRA(byte[] data, int width, int height, int samplesPerPixel = 4)
        {
            int stride = data.Length / height;

            for (int y = 0; y < height; y++)
            {
                int offset = stride * y;
                int strideEnd = offset + (width * samplesPerPixel);

                for (int i = offset; i < strideEnd; i += samplesPerPixel)
                {
                    (data[i], data[i + 2]) = (data[i + 2], data[i]);
                }
            }
        }

        private Color GetPixelColor(int x, int y)
        {
            switch (GetFirstInternalImage())
            {
                case Image<Rgba32> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Rgb24> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Abgr32> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Argb32> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Bgr24> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Bgra32> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Rgb48> imageAsFormat:
                    return imageAsFormat[x, y];
                case Image<Rgba64> imageAsFormat:
                    return imageAsFormat[x, y];
                default:
                    //Fallback

                    //We didn't have Converter to IronSoftware.Drawing.Color for other pixel format
                    //A8, L8, L16, La16, Bgr565, Bgra4444, RgbaVector

                    //CloneAs() is expensive!
                    //Can throw out of memory exception, when this fucntion get called too much
                    using (Image<Rgb24> converted = GetFirstInternalImage().CloneAs<Rgb24>())
                    {
                        return converted[x, y];
                    }
            }
        }

        private void SetPixelColor(int x, int y, Color color)
        {
            switch (GetFirstInternalImage())
            {
                case Image<Rgba32> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Rgb24> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Abgr32> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Argb32> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Bgr24> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Bgra32> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Rgb48> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                case Image<Rgba64> imageAsFormat:
                    imageAsFormat[x, y] = color;
                    break;
                default:
                    (GetFirstInternalImage() as Image<Rgba32>)[x, y] = color;
                    break;
            }
            IsDirty = true;
        }

        private void LoadAndResizeImage(AnyBitmap original, int width, int height)
        {
            //this prevent case when original is changed before Lazy is loaded
            Binary = original.Binary;

            _lazyImage = new Lazy<IReadOnlyList<Image>>(() =>
            {

                var image = Image.Load<Rgba32>(Binary);
                image.Mutate(img => img.Resize(width, height));

                //update Binary
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, GetDefaultImageEncoder(image.Width, image.Height));
                Binary = memoryStream.ToArray();

                return [image];
            });

            ForceLoadLazyImage();
        }

        private IImageEncoder GetDefaultImageExportEncoder(ImageFormat format = ImageFormat.Default, int lossy = 100)
        {
            return format switch
            {
                ImageFormat.Jpeg => new JpegEncoder()
                {
                    Quality = lossy,
#if NET6_0_OR_GREATER
                    ColorType = JpegEncodingColor.Rgb
#else
                    ColorType = JpegColorType.Rgb
#endif
                },
                ImageFormat.Gif => new GifEncoder(),
                ImageFormat.Png => new PngEncoder(),
                ImageFormat.Webp => new WebpEncoder() { Quality = lossy },
                ImageFormat.Tiff => new TiffEncoder()
                {
                    Compression = TiffCompression

                },
                _ => GetDefaultImageEncoder(Width, Height)
            };
        }

        private static ImageFormat GetImageFormat(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new FileNotFoundException("Please provide filename.");
            }

            if (filename.ToLower().EndsWith("png"))
            {
                return ImageFormat.Png;
            }
            else if (filename.ToLower().EndsWith("jpg") || filename.ToLower().EndsWith("jpeg"))
            {
                return ImageFormat.Jpeg;
            }
            else if (filename.ToLower().EndsWith("webp"))
            {
                return ImageFormat.Jpeg;
            }
            else if (filename.ToLower().EndsWith("gif"))
            {
                return ImageFormat.Gif;
            }
            else if (filename.ToLower().EndsWith("tif") || filename.ToLower().EndsWith("tiff"))
            {
                return ImageFormat.Tiff;
            }
            else
            {
                return ImageFormat.Bmp;
            }
        }

        private static async Task<Stream> LoadUriAsync(Uri uri)
        {
            using HttpClient httpClient = new();
            MemoryStream memoryStream = new();
            using Stream stream = await httpClient.GetStreamAsync(uri);
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// return BmpEncoder for small image and PngEncoder for large image
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        private static IImageEncoder GetDefaultImageEncoder(int imageWidth, int imageHeight)
        {
            return new BmpEncoder { BitsPerPixel = BmpBitsPerPixel.Pixel32, SupportTransparency = true };
        }

        private static void InternalSaveAsMultiPageTiff(IEnumerable<Image> images, Stream stream)
        {
            using (Tiff output = Tiff.ClientOpen("in-memory", "w", null, new NonClosingTiffStream(stream)))
            {
                foreach (var image in images)
                {
                    int width = image.Width;
                    int height = image.Height;
                    int stride = width * 4; // RGBA => 4 bytes per pixel

                    // Convert to byte[] in BGRA format as required by LibTiff
                    byte[] buffer = new byte[height * stride];

                    switch (image)
                    {
                        case Image<Rgb24> imageAsFormat:
                            imageAsFormat.CopyPixelDataTo(buffer);
                            break;
                        case Image<Abgr32> imageAsFormat:
                            imageAsFormat.CopyPixelDataTo(buffer);
                            break;
                        case Image<Argb32> imageAsFormat:
                            imageAsFormat.CopyPixelDataTo(buffer);
                            break;
                        case Image<Bgr24> imageAsFormat:
                            imageAsFormat.CopyPixelDataTo(buffer);
                            break;
                        case Image<Bgra32> imageAsFormat:
                            imageAsFormat.CopyPixelDataTo(buffer);
                            break;
                        default:
                            (image as Image<Rgba32>).CopyPixelDataTo(buffer);
                            break;
                    }


                    //Note: TiffMetadata in current ImageSharp 3.1.8 is not good enough. but in the main branch of ImageSharp it looks good.
                    //TODO: revisit this TiffMetadata once release version of ImageSharp include new TiffMetadata implementation.
                    //TiffMetadata metadata = image.Metadata.GetTiffMetadata();

                    switch (image.Metadata.ResolutionUnits)
                    {
                        case SixLabors.ImageSharp.Metadata.PixelResolutionUnit.AspectRatio:
                            output.SetField(TiffTag.XRESOLUTION, image.Metadata.HorizontalResolution);
                            output.SetField(TiffTag.YRESOLUTION, image.Metadata.VerticalResolution);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.NONE);
                            break;
                        case SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerInch:
                            output.SetField(TiffTag.XRESOLUTION, image.Metadata.HorizontalResolution);
                            output.SetField(TiffTag.YRESOLUTION, image.Metadata.VerticalResolution);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                            break;
                        case SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerCentimeter:
                            output.SetField(TiffTag.XRESOLUTION, image.Metadata.HorizontalResolution);
                            output.SetField(TiffTag.YRESOLUTION, image.Metadata.VerticalResolution);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                            break;
                        case SixLabors.ImageSharp.Metadata.PixelResolutionUnit.PixelsPerMeter:
                            output.SetField(TiffTag.XRESOLUTION, image.Metadata.HorizontalResolution * 100);
                            output.SetField(TiffTag.YRESOLUTION, image.Metadata.VerticalResolution * 100);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                            break;
                    }


                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 4);
                    output.SetField(TiffTag.BITSPERSAMPLE, 8, 8, 8, 8);
                    output.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
                    output.SetField(TiffTag.ROWSPERSTRIP, height);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                    output.SetField(TiffTag.COMPRESSION, Compression.LZW); // optional
                    output.SetField(TiffTag.EXTRASAMPLES, 1, new short[] { (short)ExtraSample.ASSOCALPHA });

                    // Write each scanline
                    for (int row = 0; row < height; row++)
                    {
                        int offset = row * stride;
                        output.WriteScanline(buffer, offset, row, 0);
                    }

                    output.WriteDirectory(); // Next page
                }
            }
            stream.Position = 0;
        }
        private static void InternalSaveAsMultiPageGif(IEnumerable<Image> images, Stream stream)
        {
            // Find the maximum dimensions to create a logical screen that can fit all frames.
            int maxWidth = images.Max(f => f.Width);
            int maxHeight = images.Max(f => f.Height);

            using var gif = images.First().Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(maxWidth, maxHeight),
                Mode = ResizeMode.BoxPad, // Pad to fit the target dimensions
                PadColor = Color.Transparent, // Use transparent padding
                Position = AnchorPositionMode.Center // Center the image within the frame
            }));


            foreach (var sourceImage in images.Skip(1))
            {
                // Clone the source image and apply the more efficient Resize operation.
                // This resizes the image to fit, pads the rest with a transparent color,
                // and centers it, all in one step.
                using (var resizedFrame = sourceImage.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(maxWidth, maxHeight),
                    Mode = ResizeMode.BoxPad, // Pad to fit the target dimensions
                    PadColor = Color.Transparent, // Use transparent padding
                    Position = AnchorPositionMode.Center // Center the image within the frame
                })))
                {
                    // Add the correctly-sized new frame to the master GIF's frame collection.
                    gif.Frames.AddFrame(resizedFrame.Frames.RootFrame);
                }
            }         
            // Save the final result to the provided stream.
            gif.SaveAsGif(stream);
            stream.Position = 0;
        }

#endregion

        /// <summary>
        /// Check if image is loaded (decoded) 
        /// </summary>
        /// <returns>true if images is loaded (decoded) into the memory</returns>
        [Browsable(false)]
        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsImageLoaded()
        {
            if(_lazyImage == null)
            {
                return false;
            }
            else
            {
                return _lazyImage.IsValueCreated;
            }
        }
    }
}