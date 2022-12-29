using BitMiracle.LibTiff.Classic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// <para>A universally compatible Bitmap format for .NET 7, .NET 6, .NET 5, and .NET Core. As well as compatiblity with Windows, NanoServer, IIS, macOS, Mobile, Xamarin, iOS, Android, Google Compute, Azure, AWS, and Linux.</para>
    /// <para>Works nicely with popular Image and Bitmap formats such as System.Drawing.Bitmap, SkiaSharp, SixLabors.ImageSharp, Microsoft.Maui.Graphics.</para>
    /// <para>Implicit casting means that using this class to input and output Bitmap and image types from public API's gives full compatibility to all image type fully supported by Microsoft.</para>
    /// <para>Unlike System.Drawing.Bitmap this bitmap object is self-memory-managing and does not need to be explicitly 'used' or 'disposed'.</para>
    /// </summary>
    public partial class AnyBitmap
    {
        private SixLabors.ImageSharp.Image Image { get; set; }
        private byte[] Binary { get; set; }
        private IImageFormat Format { get; set; }

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
        /// </summary>
        /// <returns>The bitmap data as a Base64 string.</returns>
        /// <seealso cref="System.Convert.ToBase64String(byte[])"/>
        public override string ToString()
        {
            return System.Convert.ToBase64String(Binary ?? new byte[0]);
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
        /// The raw image data as a <see cref="System.IO.MemoryStream"/>
        /// </summary>
        /// <returns><see cref="System.IO.MemoryStream"/></returns>
        public System.IO.MemoryStream GetStream()
        {
            return new System.IO.MemoryStream(Binary);
        }

        /// <summary>
        /// Creates an exact duplicate <see cref="AnyBitmap"/>
        /// </summary>
        /// <returns></returns>
        public AnyBitmap Clone()
        {
            return new AnyBitmap(this.Binary);
        }

        /// <summary>
        /// Creates an exact duplicate <see cref="AnyBitmap"/> of the cropped area.
        /// </summary>
        /// <param name="Rectangle">Defines the portion of this <see cref="AnyBitmap"/> to copy.</param>
        /// <returns></returns>
        public AnyBitmap Clone(CropRectangle Rectangle)
        {
            using SixLabors.ImageSharp.Image image = Image.Clone(img => img.Crop(Rectangle));
            using var memoryStream = new System.IO.MemoryStream();
            image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
            {
                BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32,
                SupportTransparency = true
            });
            return new AnyBitmap(memoryStream.ToArray());
        }

        /// <summary>
        /// Exports the Bitmap as bytes encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable this feature.</para>
        /// </summary>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality , 100 is highest.</param>
        /// <returns>Transcoded image bytes.</returns>
        public byte[] ExportBytes(ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            using var mem = new System.IO.MemoryStream();
            ExportStream(mem, Format, Lossy);
            return mem.ToArray();
        }

        /// <summary>
        /// Exports the Bitmap as a file encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Void. Saves a file to disk.</returns>

        public void ExportFile(string File, ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            using var mem = new System.IO.MemoryStream();
            ExportStream(mem, Format, Lossy);

            System.IO.File.WriteAllBytes(File, mem.ToArray());
        }

        /// <summary>
        /// Exports the Bitmap as a <see cref="MemoryStream"/> encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Transcoded image bytes in a <see cref="MemoryStream"/>.</returns>
        public System.IO.MemoryStream ToStream(ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            var stream = new System.IO.MemoryStream();
            ExportStream(stream, Format, Lossy);
            return stream;
        }

        /// <summary>
        /// Exports the Bitmap as a Func<see cref="MemoryStream"/>> encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Transcoded image bytes in a Func<see cref="MemoryStream"/>>.</returns>
        public Func<Stream> ToStreamFn(ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            var stream = new System.IO.MemoryStream();
            ExportStream(stream, Format, Lossy);
            stream.Position = 0;
            return () => stream;
        }

        /// <summary>
        /// Saves the Bitmap to an existing <see cref="Stream"/> encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="Stream">An image encoding format.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Void. Saves Transcoded image bytes to you <see cref="Stream"/>.</returns>
        public void ExportStream(System.IO.Stream Stream, ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            if (Format == ImageFormat.Default || Format == ImageFormat.RawFormat)
            {
                var writer = new BinaryWriter(Stream);
                writer.Write(Binary);
                return;
            }

            if (Lossy < 0 || Lossy > 100) { Lossy = 100; }

            try
            {
                SixLabors.ImageSharp.Formats.IImageEncoder enc;
                switch (Format)
                {
                    case ImageFormat.Jpeg: 
                        enc = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() 
                        { 
                            Quality = Lossy,
                            ColorType = SixLabors.ImageSharp.Formats.Jpeg.JpegColorType.Rgb
                        };
                        break;
                    case ImageFormat.Gif: enc = new SixLabors.ImageSharp.Formats.Gif.GifEncoder(); break;
                    case ImageFormat.Png: enc = new SixLabors.ImageSharp.Formats.Png.PngEncoder(); break;
                    case ImageFormat.Webp: enc = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder() { Quality = Lossy }; break;
                    case ImageFormat.Tiff: enc = new SixLabors.ImageSharp.Formats.Tiff.TiffEncoder(); break;

                    default: 
                        enc = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
                        {
                            BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32,
                            SupportTransparency = true
                        };
                        break;
                }

                Image.Save(Stream, enc);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException($"Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot export stream with SixLabors.ImageSharp, {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the raw image data to a file.
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <seealso cref="TrySaveAs(string)"/>
        public void SaveAs(string File)
        {
            System.IO.File.WriteAllBytes(File, Binary);
        }

        /// <summary>
        /// Saves the image data to a file. Allows for the image to be transcoded to popular image formats.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality , 100 is highest.</param>
        /// <returns>Void.  Saves Transcoded image bytes to your File.</returns>
        /// <seealso cref="TrySaveAs(string, ImageFormat, int)"/>
        /// <seealso cref="TrySaveAs(string)"/>
        public void SaveAs(string File, ImageFormat Format, int Lossy = 100)
        {
            System.IO.File.WriteAllBytes(File, ExportBytes(Format, Lossy));
        }

        /// <summary>
        /// Tries to Save the image data to a file. Allows for the image to be transcoded to popular image formats.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality , 100 is highest.</param>
        /// <returns>returns true on success, false on failure.</returns>
        /// <seealso cref="SaveAs(string, ImageFormat, int)"/>
        public bool TrySaveAs(string File, ImageFormat Format, int Lossy = 100)
        {
            try
            {
                ExportFile(File, Format, Lossy);

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
        /// <param name="File">A fully qualified file path.</param>
        /// <seealso cref="SaveAs(string)"/>
        public bool TrySaveAs(string File)
        {
            try
            {
                SaveAs(File);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generic method to convert popular image types to <see cref="AnyBitmap"/>.
        /// <para> Support includes SixLabors.ImageSharp.Image, SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats.</para>
        /// <para>Syntax sugar. Explicit casts already also exist to and from <see cref="AnyBitmap"/> and all supported types.</para>
        /// </summary>
        /// <typeparam name="T">The Type to cast from. Support includes SixLabors.ImageSharp.Image, SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats.</typeparam>
        /// <param name="OtherBitmapFormat">A bitmap or image format from another graphics library.</param>
        /// <returns>A <see cref="AnyBitmap"/></returns>
        public static AnyBitmap FromBitmap<T>(T OtherBitmapFormat)
        {
            try
            {
                AnyBitmap result = (AnyBitmap)Convert.ChangeType(OtherBitmapFormat, typeof(AnyBitmap));
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidCastException(typeof(T).FullName, e);
            }
        }
        /// <summary>
        /// Generic method to convert <see cref="AnyBitmap"/> to popular image types.
        /// <para> Support includes SixLabors.ImageSharp.Image, SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats.</para>
        /// <para>Syntax sugar. Explicit casts already also exist to and from <see cref="AnyBitmap"/> and all supported types.</para>
        /// </summary>
        /// <typeparam name="T">The Type to cast to. Support includes SixLabors.ImageSharp.Image, SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats.</typeparam>
        /// <returns>A <see cref="AnyBitmap"/></returns>
        public T ToBitmap<T>()
        {
            try
            {
                T result = (T)Convert.ChangeType(this, typeof(T));
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidCastException(typeof(T).FullName, e);
            }
        }

        /// <summary>
        /// Create a new Bitmap from a a Byte Array.
        /// </summary>
        /// <param name="Bytes">A ByteArray of image data in any common format.</param>
        /// <seealso cref="FromBytes"/>
        /// <seealso cref="AnyBitmap(byte[])"/>
        public static AnyBitmap FromBytes(byte[] Bytes)
        {
            return new AnyBitmap(Bytes);
        }
        /// <summary>
        /// Construct a new Bitmap from binary data (bytes).
        /// </summary>
        /// <param name="Bytes">A ByteArray of image data in any common format.</param>
        /// <seealso cref="FromBytes"/>
        /// <seealso cref="AnyBitmap"/>

        public AnyBitmap(byte[] Bytes)
        {
            LoadImage(Bytes);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(Stream)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(System.IO.MemoryStream Stream)
        {
            return new AnyBitmap(Stream);
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(MemoryStream)"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(System.IO.Stream Stream)
        {
            return new AnyBitmap(Stream);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(Stream)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(System.IO.MemoryStream Stream)
        {
            LoadImage(Stream.ToArray());
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream(MemoryStream)"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(System.IO.Stream Stream)
        {
            LoadImage(Stream);
        }

        /// <summary>
        /// Create a new Bitmap from a file.
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <seealso cref="FromFile"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromFile(string File)
        {
            if (File.ToLower().EndsWith(".svg"))
            {
                return LoadSVGImage(File);
            }
            else
            {
                return new AnyBitmap(File);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a file.
        /// </summary>
        /// <param name="File">A fully qualified file path./</param>
        /// <seealso cref="FromFile"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(string File)
        {
            LoadImage(File);
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri
        /// </summary>
        /// <param name="Uri">The uri of the image.</param>
        /// <seealso cref="FromUri"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(Uri Uri)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    LoadImage(client.OpenRead(Uri));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Construct a new Bitmap from a Uri
        /// </summary>
        /// <param name="Uri">The uri of the image.</param>
        /// <returns></returns>
        /// <seealso cref="AnyBitmap"/>
        /// <seealso cref="FromUri"/>
        public static AnyBitmap FromUri(Uri Uri)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return new AnyBitmap(client.OpenRead(Uri));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while loading AnyBitmap from Uri", e);
            }
        }

        /// <summary>
        /// Gets colors depth, in number of bits per pixel.
        /// </summary>
        public int BitsPerPixel
        {
            get
            {
                return Image.PixelType.BitsPerPixel;
            }
        }

        /// <summary>
        /// Returns the number of frames in our loaded Image.  Each “frame” is a page of an image such as  Tiff or Gif.  All other image formats return 1. 
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
        /// Returns all of the cloned frames in our loaded Image. Each "frame" is a page of an image such as Tiff or Gif. All other image formats return an IEnumerable of length 1.
        /// </summary>
        /// <seealso cref="FrameCount" />
        /// <seealso cref="System.Linq" />
        public IEnumerable<AnyBitmap> GetAllFrames
        {
            get
            {
                if (FrameCount > 1)
                {
                    List<AnyBitmap> images = new List<AnyBitmap>();

                    for (int currFrameIndex = 0; currFrameIndex < FrameCount; currFrameIndex++)
                    {
                        images.Add(Image.Frames.CloneFrame(currFrameIndex));
                    }
                    return images;
                }
                else
                {
                    return new List<AnyBitmap>() { this.Clone() };
                }
            }
        }

        /// <summary>
        /// Creates a multi-frame TIFF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and height.</para>
        /// <para>The image dimension still the same with original dimension with black background.</para>
        /// </summary>
        /// <param name="imagePaths">Array of fully qualified file path to merge into Tiff image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameTiff(IEnumerable<string> imagePaths)
        {
            MemoryStream stream = CreateMultiFrameImage(CreateAnyBitmaps(imagePaths));

            if (stream == null)
                throw new NotSupportedException("Image could not be loaded. File format is not supported.");

            stream.Seek(0, SeekOrigin.Begin);
            return AnyBitmap.FromStream(stream);
        }

        /// <summary>
        /// Creates a multi-frame TIFF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and height.</para>
        /// <para>The image dimension still the same with original dimension with black background.</para>
        /// </summary>
        /// <param name="images">Array of <see cref="AnyBitmap"/> to merge into Tiff image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameTiff(IEnumerable<AnyBitmap> images)
        {
            MemoryStream stream = CreateMultiFrameImage(images);

            if (stream == null) 
                throw new NotSupportedException("Image could not be loaded. File format is not supported.");

            stream.Seek(0, SeekOrigin.Begin);
            return AnyBitmap.FromStream(stream);
        }

        /// <summary>
        /// Creates a multi-frame GIF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and height.</para>
        /// <para>The image dimension still the same with original dimension with background transparent.</para>
        /// </summary>
        /// <param name="imagePaths">Array of fully qualified file path to merge into Gif image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameGif(IEnumerable<string> imagePaths)
        {
            MemoryStream stream = CreateMultiFrameImage(CreateAnyBitmaps(imagePaths), ImageFormat.Gif);

            if (stream == null)
                throw new NotSupportedException("Image could not be loaded. File format is not supported.");

            stream.Seek(0, SeekOrigin.Begin);
            return AnyBitmap.FromStream(stream);
        }

        /// <summary>
        /// Creates a multi-frame GIF image from multiple AnyBitmaps.
        /// <para>All images should have the same dimension.</para>
        /// <para>If not dimension will be scaling to the largest width and height.</para>
        /// <para>The image dimension still the same with original dimension with background transparent.</para>
        /// </summary>
        /// <param name="images">Array of <see cref="AnyBitmap"/> to merge into Gif image.</param>
        /// <returns></returns>
        public static AnyBitmap CreateMultiFrameGif(IEnumerable<AnyBitmap> images)
        {
            MemoryStream stream = CreateMultiFrameImage(images, ImageFormat.Gif);

            if (stream == null)
                throw new NotSupportedException("Image could not be loaded. File format is not supported.");

            stream.Seek(0, SeekOrigin.Begin);
            return AnyBitmap.FromStream(stream);
        }

        /// <summary>
        /// Gets the stride width (also called scan width) of the <see cref="AnyBitmap"/> object.
        /// </summary>
        public int Stride
        {
            get
            {
                return GetStride();
            }
        }

        /// <summary>
        /// Gets the address of the first pixel data in the <see cref="AnyBitmap"/>. This can also be thought of as the first scan line in the <see cref="AnyBitmap"/>.
        /// </summary>
        /// <returns>The address of the first 32bpp BGRA pixel data in the <see cref="AnyBitmap"/>.</returns>
        public IntPtr Scan0
        {
            get
            {
                return GetFirstPixelData();
            }
        }

        /// <summary>
        /// Returns the <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types">HTTP MIME types</see> of the image. 
        /// <para>must be one of the following: image/bmp, image/jpeg, image/png, image/gif, image/tiff, image/webp, or image/unknown.</para>
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
        /// <returns><see cref="AnyBitmap.ImageFormat"/></returns>
        public ImageFormat GetImageFormat()
        {
            switch (Format?.DefaultMimeType) 
            {
                case "image/gif": return ImageFormat.Gif;
                case "image/tiff": return ImageFormat.Tiff;
                case "image/jpeg": return ImageFormat.Jpeg;
                case "image/png": return ImageFormat.Png;
                case "image/webp": return ImageFormat.Webp;
                case "image/vnd.microsoft.icon": return ImageFormat.Icon;

                default: return ImageFormat.Bmp;
            }
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
        /// Implicitly casts SixLabors.ImageSharp.Image objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24> Image)
        {
            try
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
                    {
                        BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel24
                    });
                    return new AnyBitmap(memoryStream.ToArray());
                }

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap from SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Image objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SixLabors.ImageSharp.Image.</param>
        static public implicit operator SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24>(AnyBitmap bitmap)
        {
            try
            {
                return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(bitmap.Binary);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Image objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> Image)
        {
            try
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
                    {
                        BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32,
                        SupportTransparency = true
                    });
                    return new AnyBitmap(memoryStream.ToArray());
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap from SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Image objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SixLabors.ImageSharp.Image.</param>
        static public implicit operator SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(AnyBitmap bitmap)
        {
            try
            {
                return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bitmap.Binary);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Image objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image Image)
        {
            try
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
                    {
                        BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32,
                        SupportTransparency = true
                    });
                    return new AnyBitmap(memoryStream.ToArray());
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap from SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Image objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SixLabors.ImageSharp.Image.</param>
        static public implicit operator SixLabors.ImageSharp.Image(AnyBitmap bitmap)
        {
            try
            {
                return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bitmap.Binary);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap to SixLabors.ImageSharp.Image", e);
            }
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKImage objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp as well.</para>
        /// </summary>
        /// <param name="Image">SkiaSharp.SKImage will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SkiaSharp.SKImage Image)
        {
            try
            {
                return new AnyBitmap(Image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap from SkiaSharp", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKImage objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp.SKImage as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SkiaSharp.SKImage.</param>
        static public implicit operator SkiaSharp.SKImage(AnyBitmap bitmap)
        {
            try
            {
                SkiaSharp.SKImage result = null;
                try
                {
                    result = SkiaSharp.SKImage.FromBitmap(SkiaSharp.SKBitmap.Decode(bitmap.Binary));
                }
                catch { }

                if (result != null)
                {
                    return result;
                }

                return OpenTiffToSKImage(bitmap);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap to SkiaSharp", e);
            }
        }
        /// <summary>
        /// Implicitly casts SkiaSharp.SKBitmap objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp as well.</para>
        /// </summary>
        /// <param name="Image">SkiaSharp.SKBitmap will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SkiaSharp.SKBitmap Image)
        {
            try
            {
                return new AnyBitmap(Image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap from SkiaSharp", e);
            }
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKBitmap objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp.SKBitmap as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is explicitly cast to a SkiaSharp.SKBitmap.</param>
        static public implicit operator SkiaSharp.SKBitmap(AnyBitmap bitmap)
        {
            try
            {
                SkiaSharp.SKBitmap result = null;
                try
                {
                    result = SkiaSharp.SKBitmap.Decode(bitmap.Binary);
                }
                catch { }

                if (result != null)
                {
                    return result;
                }

                return OpenTiffToSKBitmap(bitmap);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap to SkiaSharp", e);
            }
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Platform.PlatformImage objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support Microsoft.Maui.Graphics as well.</para>
        /// </summary>
        /// <param name="Image">Microsoft.Maui.Graphics.Platform.PlatformImage will automatically be cast to <see cref="AnyBitmap"/>.</param>

        public static implicit operator AnyBitmap(Microsoft.Maui.Graphics.Platform.PlatformImage Image)
        {
            try
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    Image.Save(memoryStream);
                    return new AnyBitmap(memoryStream.ToArray());
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install Microsoft.Maui.Graphics from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap from Microsoft.Maui.Graphics", e);
            }
        }
        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.Platform.PlatformImage objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support Microsoft.Maui.Graphics as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a Microsoft.Maui.Graphics.Platform.PlatformImage.</param>

        static public implicit operator Microsoft.Maui.Graphics.Platform.PlatformImage(AnyBitmap bitmap)
        {
            try
            {
                return (Microsoft.Maui.Graphics.Platform.PlatformImage)Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(bitmap.GetStream());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install Microsoft.Maui.Graphics from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while casting AnyBitmap to Microsoft.Maui.Graphics", e);
            }
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Bitmap objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="Image">System.Drawing.Bitmap will automatically be cast to <see cref="AnyBitmap"/> </param>
        public static implicit operator AnyBitmap(System.Drawing.Bitmap Image)
        {
            Byte[] data;
            try
            {
                System.Drawing.Bitmap blank = new System.Drawing.Bitmap(Image.Width, Image.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(blank);
                g.Clear(Color.Transparent);
                g.DrawImage(Image, 0, 0, Image.Width, Image.Height);

                System.Drawing.Bitmap tempImage = new System.Drawing.Bitmap(blank);
                blank.Dispose();

                System.Drawing.Imaging.ImageFormat imageFormat = GetMimeType(Image) != "image/unknown" ? Image.RawFormat : System.Drawing.Imaging.ImageFormat.Bmp;
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    tempImage.Save(memoryStream, imageFormat);
                    tempImage.Dispose();

                    data = memoryStream.ToArray();
                    return new AnyBitmap(data);
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException || e is TypeInitializationException)
                {
#if NETSTANDARD
                    if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }
                throw e;
            }
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Bitmap objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a System.Drawing.Bitmap.</param>
        static public implicit operator System.Drawing.Bitmap(AnyBitmap bitmap)
        {
            try
            {
                return (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(new System.IO.MemoryStream(bitmap.Binary));
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException || e is TypeInitializationException)
                {
#if NETSTANDARD
                    if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }
                throw e;
            }
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Image objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="Image">System.Drawing.Image will automatically be cast to <see cref="AnyBitmap"/> </param>
        public static implicit operator AnyBitmap(System.Drawing.Image Image)
        {
            Byte[] data;
            try
            {
                System.Drawing.Bitmap blank = new System.Drawing.Bitmap(Image.Width, Image.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(blank);
                g.Clear(Color.Transparent);
                g.DrawImage(Image, 0, 0, Image.Width, Image.Height);

                System.Drawing.Bitmap tempImage = new System.Drawing.Bitmap(blank);
                blank.Dispose();

                System.Drawing.Imaging.ImageFormat imageFormat = GetMimeType(Image) != "image/unknown" ? Image.RawFormat : System.Drawing.Imaging.ImageFormat.Bmp;
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    tempImage.Save(memoryStream, imageFormat);
                    tempImage.Dispose();

                    data = memoryStream.ToArray();
                    return new AnyBitmap(data);
                }
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException || e is TypeInitializationException)
                {
#if NETSTANDARD
                    if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }
                throw e;
            }
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Image objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a System.Drawing.Image.</param>
        static public implicit operator System.Drawing.Image(AnyBitmap bitmap)
        {
            try
            {
                return System.Drawing.Image.FromStream(new System.IO.MemoryStream(bitmap.Binary));
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.", e);
            }
            catch (Exception e)
            {
                if (e is PlatformNotSupportedException || e is TypeInitializationException)
                {
#if NETSTANDARD
                    if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    {
                        throw SystemDotDrawingPlatformNotSupported(e);
                    }
#endif
                }
                throw e;
            }
        }

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

            /// <summary> The WBMP image format. Will default to BMP if not supported on the runtime platform.</summary>
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

        #region Private Method

        private void LoadImage(byte[] Bytes)
        {
            try
            {
                IImageFormat format;
                Image = SixLabors.ImageSharp.Image.Load(Bytes, out format);
                Binary = Bytes;
                Format = format;
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (NotSupportedException e)
            {
                try
                {
                    OpenTiffToImageSharp(Bytes);
                }
                catch
                {
                    throw new NotSupportedException("Image could not be loaded. File format is not supported.", e);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while loading image bytes.", e);

            }
        }

        private void LoadImage(string File)
        {
            try
            {
                IImageFormat format;
                Image = SixLabors.ImageSharp.Image.Load(File, out format);
                Binary = System.IO.File.ReadAllBytes(File);
                Format = format;
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.", e);
            }
            catch (NotSupportedException)
            {
                try
                {
                    OpenTiffToImageSharp(System.IO.File.ReadAllBytes(File));
                }
                catch (Exception e)
                {
                    throw new NotSupportedException("Image could not be loaded. File format is not supported.", e);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while loading image file.", e);
            }
        }

        private void SetBinaryFromImageSharp(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> tiffImage)
        {
            using (var memoryStream = new MemoryStream())
            {
                tiffImage.Save(memoryStream, new SixLabors.ImageSharp.Formats.Tiff.TiffEncoder());
                memoryStream.Seek(0, SeekOrigin.Begin);
                LoadImage(memoryStream);
            }
        }

        private void LoadImage(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                LoadImage(ms.ToArray());
            }
        }

        private static AnyBitmap LoadSVGImage(string File)
        {
            try
            {
                return new AnyBitmap(DecodeSVG(File).Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while reading SVG image format.", e);
            }
        }

        private static SkiaSharp.SKBitmap DecodeSVG(string strInput)
        {
            try
            {
                SkiaSharp.Extended.Svg.SKSvg svg = new SkiaSharp.Extended.Svg.SKSvg();
                svg.Load(strInput);

                SkiaSharp.SKBitmap toBitmap = new SkiaSharp.SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
                using (SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(toBitmap))
                {
                    canvas.Clear(SkiaSharp.SKColors.White);
                    canvas.DrawPicture(svg.Picture);
                    canvas.Flush();
                }

                return toBitmap;

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install SkiaSharp.Svg from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while reading SVG image format.", e);
            }
        }

        private List<Exception> TryExportStream(System.IO.Stream Stream, ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            List<Exception> exceptions = new List<Exception>();
            bool isSucceed = false;
            if (IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                try
                {
                    SixLabors.ImageSharp.Formats.IImageEncoder enc;
                    switch (Format)
                    {
                        case ImageFormat.Jpeg: enc = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = Lossy }; break;
                        case ImageFormat.Gif: enc = new SixLabors.ImageSharp.Formats.Gif.GifEncoder(); break;
                        case ImageFormat.Png: enc = new SixLabors.ImageSharp.Formats.Png.PngEncoder(); break;
                        case ImageFormat.Webp: enc = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder() { Quality = Lossy }; break;
                        case ImageFormat.Tiff: enc = new SixLabors.ImageSharp.Formats.Tiff.TiffEncoder(); break;

                        default: enc = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(); break;
                    }

                    Image.Save(Stream, enc);
                    isSucceed = true;
                }
                catch (Exception ex)
                {
                    exceptions.Add(new Exception($"Cannot export stream with SixLabors.ImageSharp, {ex.Message}"));
                }
            }
            else
            {
                exceptions.Add(new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet."));
            }

            if (!isSucceed)
            {
                if (IsLoadedType("SkiaSharp.SKImage"))
                {
                    try
                    {
                        using SkiaSharp.SKImage img = this; // magic implicit cast

                        if (Format == ImageFormat.Gif || Format == ImageFormat.Tiff || Format == ImageFormat.Bmp)
                        {
                            var writer = new BinaryWriter(Stream);
                            writer.Write(Binary);
                        }
                        else
                        {
                            var skdata = img.Encode((SkiaSharp.SKEncodedImageFormat)((int)Format), Lossy);
                            skdata.SaveTo(Stream);
                        }

                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new Exception($"Cannot export stream with SkiaSharp, {ex.Message}"));
                    }
                }
                else
                {
                    exceptions.Add(new DllNotFoundException("Please install SkiaSharp from NuGet."));
                }

            }

            if (!isSucceed)
            {
                if (IsLoadedType("System.Drawing.Bitmap"))
                {
                    try
                    {
                        using System.Drawing.Bitmap img = (System.Drawing.Bitmap)this; // magic implicit cast

                        System.Drawing.Imaging.ImageFormat exportFormat;
                        switch (Format)
                        {
                            case ImageFormat.Jpeg: exportFormat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                            case ImageFormat.Gif: exportFormat = System.Drawing.Imaging.ImageFormat.Gif; break;
                            case ImageFormat.Png: exportFormat = System.Drawing.Imaging.ImageFormat.Png; break;
                            case ImageFormat.Tiff: exportFormat = System.Drawing.Imaging.ImageFormat.Tiff; break;
                            case ImageFormat.Wmf: exportFormat = System.Drawing.Imaging.ImageFormat.Wmf; break;
                            case ImageFormat.Icon: exportFormat = System.Drawing.Imaging.ImageFormat.Icon; break;
                            default: exportFormat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        }

                        if (exportFormat == System.Drawing.Imaging.ImageFormat.Jpeg)
                        {
                            var encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                            encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Lossy);
                            var jpegEncoder = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");
                            img.Save(Stream, jpegEncoder, encoderParams);
                        }
                        else
                        {
                            img.Save(Stream, exportFormat);
                            isSucceed = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new Exception($"Cannot export stream with System.Drawing.Bitmap, {ex.Message}"));
                    }
                }
                else
                {
                    exceptions.Add(new DllNotFoundException("Please install System.Drawing from NuGet."));
                }

            }
            return exceptions;
        }

        private static PlatformNotSupportedException SystemDotDrawingPlatformNotSupported(Exception innerException)
        {
            return new PlatformNotSupportedException($"Microsoft has chosen to no longer support System.Drawing.Common on Linux or MacOS. To solve this please use another Bitmap type such as {typeof(System.Drawing.Bitmap).ToString()}, SkiaSharp or ImageSharp.\n\nhttps://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only", innerException);
        }

        private static InvalidCastException ImageCastException(string fullTypeName, Exception innerException)
        {
            return new InvalidCastException($"IronSoftware.Drawing does not yet support casting {fullTypeName} to {typeof(AnyBitmap).FullName}. Try using System.Drawing.Common, SkiaSharp or ImageSharp.", innerException);
        }

        private static AggregateException NoConverterException(ImageFormat Format, List<Exception> innerExceptions)
        {
            return new AggregateException($"{typeof(AnyBitmap)} is unable to convert your image data to {Format.ToString()} because it requires a suitable encoder to be added to your project via Nuget.\nPlease try SkiaSharp, System.Drawing.Common, SixLabors.ImageSharp, Microsoft.Maui.Graphics, or alternatively save using ImageFormat.Default", innerExceptions);
        }

        private static bool IsLoadedType(string typeName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (a.GetTypes().Any(t => t.FullName == typeName)) return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not load {a.FullName} : {ex.Message}");
                }
            }
            return false;
        }

        private static string GetMimeType(System.Drawing.Bitmap Image)
        {
            var imgguid = Image.RawFormat.Guid;
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == imgguid)
                    return codec.MimeType;
            }
            return "image/unknown";
        }

        private static string GetMimeType(System.Drawing.Image Image)
        {
            var imgguid = Image.RawFormat.Guid;
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == imgguid)
                    return codec.MimeType;
            }
            return "image/unknown";
        }

        private static SkiaSharp.SKImage OpenTiffToSKImage(AnyBitmap anyBitmap)
        {
            SkiaSharp.SKBitmap skBitmap = OpenTiffToSKBitmap(anyBitmap);
            if (skBitmap != null)
            {
                return SkiaSharp.SKImage.FromBitmap(skBitmap);
            }

            return null;
        }

        private static SkiaSharp.SKBitmap OpenTiffToSKBitmap(AnyBitmap anyBitmap)
        {
            try
            {
                // create a memory stream out of them
                MemoryStream tiffStream = new MemoryStream(anyBitmap.Binary);

                // open a TIFF stored in the stream
                using (var tifImg = BitMiracle.LibTiff.Classic.Tiff.ClientOpen("in-memory", "r", tiffStream, new BitMiracle.LibTiff.Classic.TiffStream()))
                {
                    // read the dimensions
                    var width = tifImg.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGEWIDTH)[0].ToInt();
                    var height = tifImg.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGELENGTH)[0].ToInt();

                    // create the bitmap
                    var bitmap = new SkiaSharp.SKBitmap();
                    var info = new SkiaSharp.SKImageInfo(width, height);

                    // create the buffer that will hold the pixels
                    var raster = new int[width * height];

                    // get a pointer to the buffer, and give it to the bitmap
                    var ptr = System.Runtime.InteropServices.GCHandle.Alloc(raster, System.Runtime.InteropServices.GCHandleType.Pinned);
                    bitmap.InstallPixels(info, ptr.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) => ptr.Free(), null);

                    // read the image into the memory buffer
                    if (!tifImg.ReadRGBAImageOriented(width, height, raster, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT))
                    {
                        // not a valid TIF image.
                        return null;
                    }

                    // swap the red and blue because SkiaSharp may differ from the tiff
                    if (SkiaSharp.SKImageInfo.PlatformColorType == SkiaSharp.SKColorType.Bgra8888)
                    {
                        SkiaSharp.SKSwizzle.SwapRedBlue(ptr.AddrOfPinnedObject(), raster.Length);
                    }

                    return bitmap;
                }

            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install BitMiracle.LibTiff.NET from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while reading TIFF image format.", e);
            }
        }

        private void OpenTiffToImageSharp(byte[] bytes)
        {
            try
            {
                List<SixLabors.ImageSharp.Image> images = new List<SixLabors.ImageSharp.Image>();

                // create a memory stream out of them
                MemoryStream tiffStream = new MemoryStream(bytes);

                // open a TIFF stored in the stream
                using (var tif = BitMiracle.LibTiff.Classic.Tiff.ClientOpen("in-memory", "r", tiffStream, new BitMiracle.LibTiff.Classic.TiffStream()))
                {
                    var num = tif.NumberOfDirectories();
                    for (short i = 0; i < num; i++)
                    {
                        tif.SetDirectory(i);

                        // Find the width and height of the image
                        FieldValue[] value = tif.GetField(TiffTag.IMAGEWIDTH);
                        int width = value[0].ToInt();

                        value = tif.GetField(TiffTag.IMAGELENGTH);
                        int height = value[0].ToInt();

                        // Read the image into the memory buffer
                        int[] raster = new int[height * width];
                        if (!tif.ReadRGBAImage(width, height, raster))
                        {
                            throw new Exception("Could not read image");
                        }

                        using SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> bmp = new(width, height);
                        SixLabors.ImageSharp.Rectangle rect = new SixLabors.ImageSharp.Rectangle(0, 0, bmp.Width, bmp.Height);

                        int stride = GetStride(bmp);

                        byte[] bits = new byte[stride * bmp.Height];
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            int rasterOffset = y * bmp.Width;
                            int bitsOffset = (bmp.Height - y - 1) * stride;

                            for (int x = 0; x < bmp.Width; x++)
                            {
                                int rgba = raster[rasterOffset++];
                                bits[bitsOffset++] = (byte)(rgba & 0xff); // R
                                bits[bitsOffset++] = (byte)((rgba >> 8) & 0xff); // G
                                bits[bitsOffset++] = (byte)((rgba >> 16) & 0xff); // B
                                bits[bitsOffset++] = (byte)((rgba >> 24) & 0xff); // A
                            }
                        }

                        images.Add(SixLabors.ImageSharp.Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(bits, bmp.Width, bmp.Height));
                    }
                }

                if (Image != null)
                    Image.Dispose();

                FindMaxWidthAndHeight(images, out int maxWidth, out int maxHeight);

                using SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> tiffImage = CloneAndResizeImageSharp(images[0], maxWidth, maxHeight);
                for (int i = 1; i < images.Count; i++)
                {
                    SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image = CloneAndResizeImageSharp(images[i], maxWidth, maxHeight);
                    tiffImage.Frames.AddFrame(image.Frames.RootFrame);
                }
                SetBinaryFromImageSharp(tiffImage);
            }
            catch (DllNotFoundException e)
            {
                throw new DllNotFoundException("Please install BitMiracle.LibTiff.NET from NuGet.", e);
            }
            catch (Exception e)
            {
                throw new Exception("Error while reading TIFF image format.", e);
            }
        }

        private static List<AnyBitmap> CreateAnyBitmaps(IEnumerable<string> imagePaths)
        {
            List<AnyBitmap> bitmaps = new List<AnyBitmap>();
            foreach (string imagePath in imagePaths)
            {
                bitmaps.Add(AnyBitmap.FromFile(imagePath));
            }
            return bitmaps;
        }

        private static MemoryStream CreateMultiFrameImage(IEnumerable<AnyBitmap> images, ImageFormat imageFormat = ImageFormat.Tiff)
        {
            FindMaxWidthAndHeight(images, out int maxWidth, out int maxHeight);

            SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> result = null;
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
                        SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image = 
                            LoadAndResizeImageSharp(images.ElementAt(i).GetBytes(), maxWidth, maxHeight, i);
                        result.Frames.AddFrame(image.Frames.RootFrame);
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

            return resultStream;
        }

        private static void FindMaxWidthAndHeight(IEnumerable<SixLabors.ImageSharp.Image> images, out int maxWidth, out int maxHeight)
        {
            maxWidth = images.Select(img => img.Width).Max();
            maxHeight = images.Select(img => img.Height).Max();
        }

        private static void FindMaxWidthAndHeight(IEnumerable<AnyBitmap> images, out int maxWidth, out int maxHeight)
        {
            maxWidth = images.Select(img => img.Width).Max();
            maxHeight = images.Select(img => img.Height).Max();
        }

        private SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> CloneAndResizeImageSharp(
            SixLabors.ImageSharp.Image source, int maxWidth, int maxHeight)
        {
            SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image = 
                source.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();
            // Keep Image dimension the same
            return ResizeWithPadToPng(image, maxWidth, maxHeight);
        }

        private static SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> LoadAndResizeImageSharp(byte[] bytes, 
            int maxWidth, int maxHeight, int index)
        {
            try
            {
                using SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> result = 
                    SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bytes);
                // Keep Image dimension the same
                return ResizeWithPadToPng(result, maxWidth, maxHeight);
            }
            catch (Exception e)
            {
                throw new NotSupportedException($"Image index {index} cannot be loaded. File format doesn't supported.", e);
            }
        }

        private static SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> ResizeWithPadToPng(
            SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> result, int maxWidth, int maxHeight)
        {
            result.Mutate(img => img.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(maxWidth, maxHeight),
                Mode = SixLabors.ImageSharp.Processing.ResizeMode.BoxPad,
                PadColor = SixLabors.ImageSharp.Color.Transparent
            }));

            using (var memoryStream = new MemoryStream())
            {
                result.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder
                {
                    TransparentColorMode = SixLabors.ImageSharp.Formats.Png.PngTransparentColorMode.Preserve
                });
                memoryStream.Seek(0, SeekOrigin.Begin);

                return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(memoryStream);
            }
        }

        private int GetStride(SixLabors.ImageSharp.Image source = null)
        {
            if (source == null)
                return 4 * ((Image.Width * Image.PixelType.BitsPerPixel + 31) / 32);
            else
                return 4 * ((source.Width * source.PixelType.BitsPerPixel + 31) / 32);
        }

        private IntPtr GetFirstPixelData()
        {
            byte[] pixelBytes = new byte[Image.Width * Image.Height * Unsafe.SizeOf<Rgba32>()];
            var clonedImage = Image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();
            clonedImage.CopyPixelDataTo(pixelBytes);
            ConvertRGBAtoBGRA(pixelBytes, clonedImage.Width, clonedImage.Height);

            IntPtr result = Marshal.AllocHGlobal(pixelBytes.Length);
            Marshal.Copy(pixelBytes, 0, result, pixelBytes.Length);

            return result;
        }

        private void ConvertRGBAtoBGRA(byte[] data, int width, int height, int samplesPerPixel = 4)
        {
            int stride = data.Length / height;

            for (int y = 0; y < height; y++)
            {
                int offset = stride * y;
                int strideEnd = offset + width * samplesPerPixel;

                for (int i = offset; i < strideEnd; i += samplesPerPixel)
                {
                    byte temp = data[i + 2];
                    data[i + 2] = data[i];
                    data[i] = temp;
                }
            }
        }

        #endregion
    }
}

