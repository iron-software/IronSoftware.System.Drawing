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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        private Image Image { get; set; }
        private byte[] Binary { get; set; }
        private IImageFormat Format { get; set; }
        private TiffCompression TiffCompression { get; set; } = TiffCompression.Lzw;

        /// <summary>
        /// Width of the image.
        /// </summary>
        public int Width
        {
            get
            {
                return Image.Width;
            }
        }

        /// <summary>
        /// Height of the image.
        /// </summary>
        public int Height
        {
            get
            {
                return Image.Height;
            }
        }

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
            return new AnyBitmap(Binary);
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
            using Image image = Image.Clone(img => img.Crop(rectangle));
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new BmpEncoder()
            {
                BitsPerPixel = BmpBitsPerPixel.Pixel32,
                SupportTransparency = true
            });
            return new AnyBitmap(memoryStream.ToArray());
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
            if (format is ImageFormat.Default or ImageFormat.RawFormat)
            {
                var writer = new BinaryWriter(stream);
                writer.Write(Binary);
                return;
            }

            if (lossy is < 0 or > 100)
            {
                lossy = 100;
            }

            try
            {
                IImageEncoder enc = format switch
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
                    _ => new BmpEncoder()
                    {
                        BitsPerPixel = BmpBitsPerPixel.Pixel32,
                        SupportTransparency = true
                    },
                };

                Image.Save(stream, enc);
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
            return new AnyBitmap(span);
        }

        /// <summary>
        /// Create a new Bitmap from a a Byte Array.
        /// </summary>
        /// <param name="bytes">A ByteArray of image data in any common format.</param>
        public static AnyBitmap FromBytes(byte[] bytes)
        {
            return new AnyBitmap(bytes);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any 
        /// common format.</param>
        /// <seealso cref="FromStream(Stream)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(MemoryStream stream)
        {
            return new AnyBitmap(stream);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any 
        /// common format.</param>
        /// <seealso cref="FromStream(MemoryStream)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(Stream stream)
        {
            return new AnyBitmap(stream);
        }

        /// <summary>
        /// Construct a new Bitmap from binary data (byte span).
        /// </summary>
        /// <param name="span">A byte span of image data in any common format.</param>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(ReadOnlySpan<byte> span)
        {
            LoadImage(span);
        }

        /// <summary>
        /// Construct a new Bitmap from binary data (bytes).
        /// </summary>
        /// <param name="bytes">A ByteArray of image data in any common format.</param>
        /// <seealso cref="FromBytes"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(byte[] bytes)
        {
            LoadImage(bytes);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any 
        /// common format.</param>
        /// <seealso cref="FromStream(Stream)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(MemoryStream stream)
        {
            LoadImage(stream.ToArray());
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> of image data in any 
        /// common format.</param>
        /// <seealso cref="FromStream(MemoryStream)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Stream stream)
        {
            LoadImage(stream);
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
        /// <seealso cref="FromFile"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(string file)
        {
            LoadImage(File.ReadAllBytes(file));
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <seealso cref="FromUriAsync"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Uri uri)
        {
            try
            {
                using Stream stream = LoadUriAsync(uri).GetAwaiter().GetResult();
                LoadImage(stream);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from width and height
        /// </summary>
        /// <param name="width">Width of new AnyBitmap</param>
        /// <param name="height">Height of new AnyBitmap</param>
        /// <param name="backgroundColor">Background color of new AnyBitmap</param>
        public AnyBitmap(int width, int height, Color backgroundColor = null)
        {
            CreateNewImageInstance(width, height, backgroundColor);
        }

        /// <summary>
        /// Create a new Bitmap from a file.
        /// </summary>
        /// <param name="file">A fully qualified file path.</param>
        /// <seealso cref="FromFile"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromFile(string file)
        {
            if (file.ToLower().EndsWith(".svg"))
            {
                return LoadSVGImage(file);
            }
            else
            {
                return new AnyBitmap(file);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri
        /// </summary>
        /// <param name="uri">The uri of the image.</param>
        /// <returns></returns>
        /// <seealso cref="AnyBitmap"/>
        /// <seealso cref="FromUri"/>
        /// <seealso cref="FromUriAsync"/>
        public static async Task<AnyBitmap> FromUriAsync(Uri uri)
        {
            try
            {
                using Stream stream = await LoadUriAsync(uri);
                return new AnyBitmap(stream);
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
        /// <seealso cref="FromUriAsync"/>
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
        /// Creates an AnyBitmap object from a buffer of RGB pixel data.
        /// </summary>
        /// <param name="buffer">An array of bytes representing the RGB pixel data. This should contain 3 bytes (one each for red, green, and blue) for each pixel in the image.</param>
        /// <param name="width">The width of the image, in pixels.</param>
        /// <param name="height">The height of the image, in pixels.</param>
        /// <returns>An AnyBitmap object that represents the image defined by the provided pixel data, width, and height.</returns>
        public static AnyBitmap LoadAnyBitmapFromRGBBuffer(byte[] buffer, int width, int height)
        {
            using var memoryStream = new MemoryStream();
            using var image = Image.LoadPixelData<Rgb24>(buffer, width, height);
            image.Save(memoryStream, new BmpEncoder()
            {
                BitsPerPixel = BmpBitsPerPixel.Pixel24
            });
            return new AnyBitmap(memoryStream.ToArray());
        }

        /// <summary>
        /// Gets colors depth, in number of bits per pixel.
        /// <br/><para><b>Further Documentation:</b><br/>
        /// <a href="https://ironsoftware.com/open-source/csharp/drawing/examples/get-color-depth/">
        /// Code Example</a></para>
        /// </summary>
        public int BitsPerPixel
        {
            get
            {
                return Image.PixelType.BitsPerPixel;
            }
        }

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
                return Image.Frames.Count;
            }
        }

        /// <summary>
        /// Returns all of the cloned frames in our loaded Image. Each "frame" 
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
                if (FrameCount > 1)
                {
                    List<AnyBitmap> images = new();

                    for (int currFrameIndex = 0; currFrameIndex < FrameCount; currFrameIndex++)
                    {
                        images.Add(Image.Frames.CloneFrame(currFrameIndex));
                    }

                    return images;
                }
                else
                {
                    return new List<AnyBitmap>() { Clone() };
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
            using MemoryStream stream =
                CreateMultiFrameImage(CreateAnyBitmaps(imagePaths))
                ?? throw new NotSupportedException("Image could not be loaded. File format is not supported.");
            _ = stream.Seek(0, SeekOrigin.Begin);
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
            using MemoryStream stream =
                CreateMultiFrameImage(images)
                ?? throw new NotSupportedException("Image could not be loaded. File format is not supported.");
            _ = stream.Seek(0, SeekOrigin.Begin);
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
            using MemoryStream stream =
                CreateMultiFrameImage(CreateAnyBitmaps(imagePaths), ImageFormat.Gif)
                ?? throw new NotSupportedException("Image could not be loaded. File format is not supported.");
            _ = stream.Seek(0, SeekOrigin.Begin);
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
            using MemoryStream stream =
                CreateMultiFrameImage(images, ImageFormat.Gif)
                ?? throw new NotSupportedException("Image could not be loaded. File format is not supported.");
            _ = stream.Seek(0, SeekOrigin.Begin);
            return FromStream(stream);
        }

        /// <summary>
        /// Extracts the alpha channel data from an image.
        /// </summary>
        /// <returns>An array of bytes representing the alpha values of the image's pixels.</returns>
        /// <exception cref="NotSupportedException">Thrown when the image's bit depth is not 32 bpp.</exception>
        public byte[] ExtractAlphaData()
        {
            if (BitsPerPixel == 32)
            {
                var alpha = new byte[Image.Width * Image.Height];
                int alphaIndex = 0;
                using var rgbaImage = Image is Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image
                    ? image
                    : Image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();
                rgbaImage.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        // Get the row as a span of Rgba32.
                        Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                        // Interpret the row as a span of bytes.
                        Span<byte> rowBytes = MemoryMarshal.AsBytes(pixelRow);

                        // Each pixel is 4 bytes: R, G, B, A.
                        // The alpha channel is the fourth byte (index 3, 7, 11, ...).
                        for (int i = 3; i < rowBytes.Length; i += 4)
                        {
                            alpha[alphaIndex++] = rowBytes[i];
                        }
                    }
                });

                return alpha.ToArray();
            }
            else
            {
                throw new NotSupportedException($"Extracting alpha data is not supported for {BitsPerPixel} bpp images.");
            }
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

            using var memoryStream = new MemoryStream();
            using var image = Image.Load(bitmap.ExportBytes());

            image.Mutate(x => x.RotateFlip(rotateModeImgSharp, flipModeImgSharp));
            image.Save(memoryStream, new BmpEncoder()
            {
                BitsPerPixel = BmpBitsPerPixel.Pixel32,
                SupportTransparency = true
            });

            return new AnyBitmap(memoryStream.ToArray());
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
            using var memoryStream = new MemoryStream();
            using var image = Image.Load(bitmap.ExportBytes());
            Rectangle rectangle = Rectangle;
            var brush = new SolidBrush(color);
            image.Mutate(ctx => ctx.Fill(brush, rectangle));
            image.Save(memoryStream, new BmpEncoder()
            {
                BitsPerPixel = BmpBitsPerPixel.Pixel32,
                SupportTransparency = true
            });

            return new AnyBitmap(memoryStream.ToArray());
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
                return Image?.Metadata.HorizontalResolution ?? null;
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
                return Image?.Metadata.VerticalResolution ?? null;
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
        /// Sets the <see cref="Color"/> of the specified pixel in this 
        /// <see cref="AnyBitmap"/>
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
            using Image<Rgb24> image = Image.CloneAs<Rgb24>();

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
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, new BmpEncoder()
                {
                    BitsPerPixel = BmpBitsPerPixel.Pixel24
                });
                return new AnyBitmap(memoryStream.ToArray());

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
            try
            {
                return Image.Load<Rgb24>(bitmap.Binary);
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
        /// <param name="image">SixLabors.ImageSharp.Image will automatically be
        /// cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(Image<Rgba32> image)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, new BmpEncoder()
                {
                    BitsPerPixel = BmpBitsPerPixel.Pixel32,
                    SupportTransparency = true
                });
                return new AnyBitmap(memoryStream.ToArray());
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
                return Image.Load<Rgba32>(bitmap.Binary);
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
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, new BmpEncoder()
                {
                    BitsPerPixel = BmpBitsPerPixel.Pixel32,
                    SupportTransparency = true
                });
                return new AnyBitmap(memoryStream.ToArray());
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
                return Image.Load(bitmap.Binary);
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
                return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(new MemoryStream(bitmap.Binary));
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
                return System.Drawing.Image.FromStream(new MemoryStream(bitmap.Binary));
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

        #region Enum Classes

        /// <summary>
        /// Popular image formats which <see cref="AnyBitmap"/> can read and export.
        /// </summary>
        /// <seealso cref="ExportFile(string, ImageFormat, int)"/>
        /// <seealso cref="ExportStream(Stream, ImageFormat, int)"/>
        /// <seealso cref="ExportBytes(ImageFormat, int)"/>
        public enum ImageFormat
        {
            /// <summary> The Bitmap image format.</summary>
            Bmp = 0,

            /// <summary> The Gif image format.</summary>
            Gif = 1,

            /// <summary> The Tiff image format.</summary>
            Tiff = 2,

            /// <summary> The Jpeg image format.</summary>
            Jpeg = 3,

            /// <summary> The PNG image format.</summary>
            Png = 4,

            /// <summary> The WBMP image format. Will default to BMP if not 
            /// supported on the runtime platform.</summary>
            Wbmp = 5,

            /// <summary> The new WebP image format.</summary>
            Webp = 6,

            /// <summary> The Icon image format.</summary>
            Icon = 7,

            /// <summary> The Wmf image format.</summary>
            Wmf = 8,

            /// <summary> The Raw image format.</summary>
            RawFormat = 9,

            /// <summary> The existing raw image format.</summary>
            Default = -1

        }
# pragma warning disable CS0618
        /// <summary>
        /// Converts the legacy <see cref="RotateFlipType"/> to <see cref="RotateMode"/> and <see cref="FlipMode"/>
        /// </summary>
        [Obsolete("RotateFlipType is legacy support from System.Drawing. " +
            "Please use RotateMode and FlipMode instead.")]
        internal static (RotateMode, FlipMode) ParseRotateFlipType(RotateFlipType rotateFlipType)
        {
            return rotateFlipType switch
            {
                RotateFlipType.RotateNoneFlipNone or RotateFlipType.Rotate180FlipXY => (RotateMode.None, FlipMode.None),
                RotateFlipType.Rotate90FlipNone or RotateFlipType.Rotate270FlipXY => (RotateMode.Rotate90, FlipMode.None),
                RotateFlipType.RotateNoneFlipXY or RotateFlipType.Rotate180FlipNone => (RotateMode.Rotate180, FlipMode.None),
                RotateFlipType.Rotate90FlipXY or RotateFlipType.Rotate270FlipNone => (RotateMode.Rotate270, FlipMode.None),
                RotateFlipType.RotateNoneFlipX or RotateFlipType.Rotate180FlipY => (RotateMode.None, FlipMode.Horizontal),
                RotateFlipType.Rotate90FlipX or RotateFlipType.Rotate270FlipY => (RotateMode.Rotate90, FlipMode.Horizontal),
                RotateFlipType.RotateNoneFlipY or RotateFlipType.Rotate180FlipX => (RotateMode.None, FlipMode.Vertical),
                RotateFlipType.Rotate90FlipY or RotateFlipType.Rotate270FlipX => (RotateMode.Rotate90, FlipMode.Vertical),
                _ => throw new ArgumentOutOfRangeException(nameof(rotateFlipType), rotateFlipType, null),
            };
        }
# pragma warning restore CS0618

        /// <summary>
        /// Provides enumeration over how the image should be rotated.
        /// </summary>
        public enum RotateMode
        {
            /// <summary>
            /// Do not rotate the image.
            /// </summary>
            None,

            /// <summary>
            /// Rotate the image by 90 degrees clockwise.
            /// </summary>
            Rotate90 = 90,

            /// <summary>
            /// Rotate the image by 180 degrees clockwise.
            /// </summary>
            Rotate180 = 180,

            /// <summary>
            /// Rotate the image by 270 degrees clockwise.
            /// </summary>
            Rotate270 = 270
        }

        /// <summary>
        /// Provides enumeration over how a image should be flipped.
        /// </summary>
        public enum FlipMode
        {
            /// <summary>
            /// Don't flip the image.
            /// </summary>
            None,

            /// <summary>
            /// Flip the image horizontally.
            /// </summary>
            Horizontal,

            /// <summary>
            /// Flip the image vertically.
            /// </summary>
            Vertical
        }

        /// <summary>
        /// Specifies how much an image is rotated and the axis used to flip 
        /// the image. This follows the legacy System.Drawing.RotateFlipType 
        /// notation.
        /// </summary>
        [Obsolete("RotateFlipType is legacy support from System.Drawing. " +
            "Please use RotateMode and FlipMode instead.")]
        public enum RotateFlipType
        {
            /// <summary>
            /// Specifies no clockwise rotation and no flipping.
            /// </summary>
            RotateNoneFlipNone,
            /// <summary>
            /// Specifies a 180-degree clockwise rotation followed by a 
            /// horizontal and vertical flip.
            /// </summary>
            Rotate180FlipXY,

            /// <summary>
            /// Specifies a 90-degree clockwise rotation without flipping.
            /// </summary>
            Rotate90FlipNone,
            /// <summary>
            /// Specifies a 270-degree clockwise rotation followed by a 
            /// horizontal and vertical flip.
            /// </summary>
            Rotate270FlipXY,

            /// <summary>
            /// Specifies no clockwise rotation followed by a horizontal and 
            /// vertical flip.
            /// </summary>
            RotateNoneFlipXY,
            /// <summary>
            /// Specifies a 180-degree clockwise rotation without flipping.
            /// </summary>
            Rotate180FlipNone,

            /// <summary>
            /// Specifies a 90-degree clockwise rotation followed by a 
            /// horizontal and vertical flip.
            /// </summary>
            Rotate90FlipXY,
            /// <summary>
            /// Specifies a 270-degree clockwise rotation without flipping.
            /// </summary>
            Rotate270FlipNone,

            /// <summary>
            /// Specifies no clockwise rotation followed by a horizontal flip.
            /// </summary>
            RotateNoneFlipX,
            /// <summary>
            /// Specifies a 180-degree clockwise rotation followed by a 
            /// vertical flip.
            /// </summary>
            Rotate180FlipY,

            /// <summary>
            /// Specifies a 90-degree clockwise rotation followed by a 
            /// horizontal flip.
            /// </summary>
            Rotate90FlipX,
            /// <summary>
            /// Specifies a 270-degree clockwise rotation followed by a 
            /// vertical flip.
            /// </summary>
            Rotate270FlipY,

            /// <summary>
            /// Specifies no clockwise rotation followed by a vertical flip.
            /// </summary>
            RotateNoneFlipY,
            /// <summary>
            /// Specifies a 180-degree clockwise rotation followed by a 
            /// horizontal flip.
            /// </summary>
            Rotate180FlipX,

            /// <summary>
            /// Specifies a 90-degree clockwise rotation followed by a 
            /// vertical flip.
            /// </summary>
            Rotate90FlipY,
            /// <summary>
            /// Specifies a 270-degree clockwise rotation followed by a 
            /// horizontal flip.
            /// </summary>
            Rotate270FlipX
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

                Image?.Dispose();
                Image = null;
                Binary = null;
                _disposed = true;
            }
        }

        #region Private Method

        private void CreateNewImageInstance(int width, int height, Color backgroundColor)
        {
            Image = new Image<Rgba32>(width, height);
            if (backgroundColor != null)
            {
                Image.Mutate(context => context.Fill(backgroundColor));
            }
            using var stream = new MemoryStream();
            Image.SaveAsBmp(stream);
            Binary = stream.ToArray();
        }
        
        private void LoadImage(ReadOnlySpan<byte> bytes)
        {
            Format = Image.DetectFormat(bytes);
            try
            {
                if(Format is TiffFormat)
                    OpenTiffToImageSharp(bytes);
                else
                {
                    Binary = bytes.ToArray();
                    Image = Image.Load(bytes);
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
                    "Image could not be loaded. File format is not supported.", e);
            }
        }

        private void LoadImage(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new();
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            LoadImage(ms.ToArray());
        }

        private static AnyBitmap LoadSVGImage(string file)
        {
            try
            {
                return new AnyBitmap(
                    DecodeSVG(file).Encode(SKEncodedImageFormat.Png, 100)
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

        private void OpenTiffToImageSharp(ReadOnlySpan<byte> bytes)
        {
            try
            {
                int imageWidth = 0;
                int imageHeight = 0;
                List<Image> images = new();

                // create a memory stream out of them
                using MemoryStream tiffStream = new(bytes.ToArray());

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

                        var (width, height) = SetWidthHeight(tiff, i, ref imageWidth, ref imageHeight);

                        // Read the image into the memory buffer
                        int[] raster = new int[height * width];
                        if (!tiff.ReadRGBAImage(width, height, raster))
                        {
                            throw new NotSupportedException("Could not read image");
                        }

                        using Image<Rgba32> bmp = new(width, height);

                        var bits = PrepareByteArray(bmp, raster, width, height);

                        images.Add(Image.LoadPixelData<Rgba32>(bits, bmp.Width, bmp.Height));
                    }
                }

                // find max
                FindMaxWidthAndHeight(images, out int maxWidth, out int maxHeight);

                // mute first image
                images[0].Mutate(img => img.Resize(new ResizeOptions
                {
                    Size = new Size(maxWidth, maxHeight),
                    Mode = ResizeMode.BoxPad,
                    PadColor = SixLabors.ImageSharp.Color.Transparent
                }));

                // iterate through images past the first
                for (int i = 1; i < images.Count; i++)
                {
                    // mute image
                    images[i].Mutate(img => img.Resize(new ResizeOptions
                    {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.BoxPad,
                        PadColor = SixLabors.ImageSharp.Color.Transparent
                    }));

                    // add frames to first image
                    _ = images[0].Frames.AddFrame(images[i].Frames.RootFrame);

                    // dispose images past the first
                    images[i].Dispose();
                }                

                // get raw binary
                using var memoryStream = new MemoryStream();
                images[0].Save(memoryStream, new TiffEncoder());
                memoryStream.Seek(0, SeekOrigin.Begin);

                // store result
                Binary = memoryStream.ToArray();
                Image?.Dispose();
                Image = images[0];
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

        private void SetTiffCompression(Tiff tiff)
        {
            Compression tiffCompression  = tiff.GetField(TiffTag.COMPRESSION) != null && tiff.GetField(TiffTag.COMPRESSION).Length > 0
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

        private ReadOnlySpan<byte> PrepareByteArray(Image<Rgba32> bmp, int[] raster, int width, int height)
        {
            int stride = GetStride(bmp);
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

        private (int width, int height) SetWidthHeight(Tiff tiff, short index, ref int imageWidth, ref int imageHeight)
        {
            // Find the width and height of the image
            FieldValue[] value = tiff.GetField(TiffTag.IMAGEWIDTH);
            int width = value[0].ToInt();

            value = tiff.GetField(TiffTag.IMAGELENGTH);
            int height = value[0].ToInt();

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

            return (width, height);
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

        private static MemoryStream CreateMultiFrameImage(IEnumerable<AnyBitmap> images, ImageFormat imageFormat = ImageFormat.Tiff)
        {
            FindMaxWidthAndHeight(images, out int maxWidth, out int maxHeight);

            Image<Rgba32> result = null;
            for (int i = 0; i < images.Count(); i++)
            {
                if (i == 0)
                {
                    result = LoadAndResizeImageSharp(images.ElementAt(i).GetBytes(), maxWidth, maxHeight, i);
                }
                else
                {
                    if (result == null)
                    {
                        result = LoadAndResizeImageSharp(images.ElementAt(i).GetBytes(), maxWidth, maxHeight, i);
                    }
                    else
                    {
                        Image<Rgba32> image =
                            LoadAndResizeImageSharp(images.ElementAt(i).GetBytes(), maxWidth, maxHeight, i);
                        _ = result.Frames.AddFrame(image.Frames.RootFrame);
                    }
                }
            }

            MemoryStream resultStream = null;
            if (result != null)
            {
                resultStream = new MemoryStream();
                if (imageFormat == ImageFormat.Gif)
                {
                    result.SaveAsGif(resultStream);
                }
                else
                {
                    result.SaveAsTiff(resultStream);
                }
            }

            result?.Dispose();

            return resultStream;
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

        private static Image<Rgba32> CloneAndResizeImageSharp(
            Image source, int maxWidth, int maxHeight)
        {
            using Image<Rgba32> image =
                source.CloneAs<Rgba32>();
            // Keep Image dimension the same
            return ResizeWithPadToPng(image, maxWidth, maxHeight);
        }

        private static Image<Rgba32> LoadAndResizeImageSharp(byte[] bytes,
            int maxWidth, int maxHeight, int index)
        {
            try
            {
                using var result =
                    Image.Load<Rgba32>(bytes);
                // Keep Image dimension the same
                return ResizeWithPadToPng(result, maxWidth, maxHeight);
            }
            catch (Exception e)
            {
                throw new NotSupportedException($"Image index {index} cannot " +
                    $"be loaded. File format doesn't supported.", e);
            }
        }

        private static Image<Rgba32> ResizeWithPadToPng(
            Image<Rgba32> result, int maxWidth, int maxHeight)
        {
            result.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new Size(maxWidth, maxHeight),
                Mode = ResizeMode.BoxPad,
                PadColor = SixLabors.ImageSharp.Color.Transparent
            }));

            using var memoryStream = new MemoryStream();
            result.Save(memoryStream, new PngEncoder
            {
                TransparentColorMode = PngTransparentColorMode.Preserve
            });
            _ = memoryStream.Seek(0, SeekOrigin.Begin);

            return Image.Load<Rgba32>(memoryStream);
        }

        private int GetStride(Image source = null)
        {
            if (source == null)
            {
                return 4 * (((Image.Width * Image.PixelType.BitsPerPixel) + 31) / 32);
            }
            else
            {
                return 4 * (((source.Width * source.PixelType.BitsPerPixel) + 31) / 32);
            }
        }

        private IntPtr GetFirstPixelData()
        {
            byte[] pixelBytes = new byte[Image.Width * Image.Height * Unsafe.SizeOf<Rgba32>()];
            Image<Rgba32> clonedImage = Image.CloneAs<Rgba32>();
            clonedImage.CopyPixelDataTo(pixelBytes);
            ConvertRGBAtoBGRA(pixelBytes, clonedImage.Width, clonedImage.Height);

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
            switch (Image)
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
                    using (Image<Rgb24> converted = Image.CloneAs<Rgb24>())
                    {
                        return converted[x, y];
                    }
            }
        }

        private void SetPixelColor(int x, int y, Color color)
        {
            switch (Image)
            {
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
                default:
                    (Image as Image<Rgba32>)[x, y] = color;
                    break;
            }
        }

        private void LoadAndResizeImage(AnyBitmap original, int width, int height)
        {
#if NET6_0_OR_GREATER
            using var image = Image.Load<Rgba32>(original.Binary);
            IImageFormat format = image.Metadata.DecodedImageFormat;
#else
            using var image = Image.Load<Rgba32>(original.Binary, out IImageFormat format);
#endif
            image.Mutate(img => img.Resize(width, height));
            byte[] pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];
            image.CopyPixelDataTo(pixelBytes);

            Image = image.Clone();
            Binary = pixelBytes;
            Format = format;
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

        #endregion
    }
}