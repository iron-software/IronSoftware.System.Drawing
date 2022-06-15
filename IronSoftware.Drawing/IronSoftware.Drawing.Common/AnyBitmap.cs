using System;
using System.IO;
using System.Linq;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// <para> A universally compatible Bitmap format for .Net Core, .Net 5 .Net 6 and .Net 7.   Windows, NanoServer, IIS,  MacOS, Mobile, Xamarin, iOS, Android, Google Compute, Azure, AWS and Linux compatibility.</para>
    /// <para>Plays nicely with popular Image and Bitmap formats such as System.Drawing.Bitmap, SkiaSharp, SixLabors.ImageSharp, Microsoft.Maui.Graphics.  </para>
    /// <para>Implicit casting means that using this class to input and output Bitmap and image types from public API's gives full compatibility to all image type fully supported by Microsoft.</para>
    /// <para> Unlike System.Drawing.Bitmap this bitmap object is self memory managing and does not need to be explicitly 'used' or 'disposed'</para>
    /// </summary>
    public partial class AnyBitmap
    {
        private byte[] Binary { get; set; }

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
        ///  Allows Comparability and equality.
        /// </summary>
        /// <param name="bitmap">Another <see cref="AnyBitmap"/></param>
        /// <returns>True if the Bitmaps have exactly the same raw binary data.</returns>
        public override bool Equals(object bitmap)
        {
            AnyBitmap comp = bitmap as AnyBitmap;
            if (comp == null) { return false; }

            return Binary == ((AnyBitmap)bitmap).ExportBytes();
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
        public System.IO.MemoryStream GetBytes()
        {
            return new System.IO.MemoryStream(Binary);
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
        /// Exports the Bitmap as bytes encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common or SixLabors.ImageSharp to your project to enable this feature.</para>
        /// </summary>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">Jped and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>).  Higher values return larger file sizes.  0 is lowest quality , 100 is highest.</param>
        /// <returns>Transcoded image bytes.[</returns>
        public byte[] ExportBytes(ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            using var mem = new System.IO.MemoryStream();
            ExportStream(mem, Format, Lossy);
            return mem.ToArray();
        }

        /// <summary>
        /// Exports the Bitmap as a file encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="File">A fully qualified file path</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">Jpeg and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>).  Higher values return larger file sizes.  0 is lowest quality , 100 is highest.</param>
        /// <returns>Void.  Saves a file to disk.[</returns>

        public void ExportFile(string File, ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            using var mem = new System.IO.MemoryStream();
            ExportStream(mem, Format, Lossy);

            System.IO.File.WriteAllBytes(File, mem.ToArray());
        }

        /// <summary>
        /// Exports the Bitmap as a <see cref="MemoryStream"/> encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">Jped and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>).  Higher values return larger file sizes.  0 is lowest quality , 100 is highest.</param>
        /// <returns>Transcoded image bytes in a <see cref="MemoryStream"/>.</returns>
        public System.IO.MemoryStream ToStream(ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            var stream = new System.IO.MemoryStream();
            ExportStream(stream, Format, Lossy);
            return stream;
        }

        /// <summary>
        /// Saves the Bitmap to an existing <see cref="Stream"/> encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="Stream">An image encoding format.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">Jpeg and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>).  Higher values return larger file sizes.  0 is lowest quality , 100 is highest.</param>
        /// <returns>Void.  Saves Transcoded image bytes to you <see cref="Stream"/>.</returns>
        public void ExportStream(System.IO.Stream Stream, ImageFormat Format = ImageFormat.Default, int Lossy = 100)
        {
            if (Format == ImageFormat.Default)
            {
                Stream.Read(Binary, 0, Binary.Length);
                return;
            }

            if (Lossy < 0 || Lossy > 100) { Lossy = 100; }

            if (Type.GetType("SkiaSharp.SKImage") != null)
            {
                using SkiaSharp.SKImage img = this; // magic implicit cast

                var encoding = (SkiaSharp.SKEncodedImageFormat)((int)Format);

                var skdata = img.Encode(encoding, Lossy);

                skdata.SaveTo(Stream);
                return;
            }
#if NETSTANDARD
            else if (Type.GetType("SixLabors.ImageSharp.Image") != null)
            {
                using SixLabors.ImageSharp.Image img = this; // magic implicit cast

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

            img.Save(Stream, enc);
                return;
            }
#endif
            else if (Type.GetType("System.Drawing.Imaging.ImageFormat") != null)
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
                    try
                    {
                        var encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                        encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Lossy);
                        var jpegEncoder = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");
                        img.Save(Stream, jpegEncoder, encoderParams);
                    }
                    catch { }
                }
                else
                {
                    img.Save(Stream, exportFormat);
                }

                return;
            }

            throw NoConverterException(Format, null);
        }

        /// <summary>
        /// Saves the raw image data to a file.
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        ///               <seealso cref="TrySaveAs(string)"/>
        public void SaveAs(string File)
        {
            System.IO.File.WriteAllBytes(File, Binary);
        }

        /// <summary>
        /// Saves the image data to a file. Allows for the image to be transcoded to popular image formats.
        ///  <para>Add SkiaSharp, System.Drawing.Common or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">Jpeg and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>).  Higher values return larger file sizes.  0 is lowest quality , 100 is highest.</param>
        /// <returns>Void.  Saves Transcoded image bytes to your File.</returns>
        /// <seealso cref="TrySaveAs(string, ImageFormat, int)"/>
        ///  <seealso cref="TrySaveAs(string)"/>
        public void SaveAs(string File, ImageFormat Format, int Lossy = 100)
        {
            System.IO.File.WriteAllBytes(File, Binary);
        }

        /// <summary>
        /// Tries to Save the image data to a file. Allows for the image to be transcoded to popular image formats.
        ///  <para>Add SkiaSharp, System.Drawing.Common or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">Jpeg and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>).  Higher values return larger file sizes.  0 is lowest quality , 100 is highest.</param>
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
        ///  Tries to Save the raw image data to a file.
        ///      <returns>returns true on success, false on failure.</returns>
        /// </summary>
        /// <param name="File">A fully qualified file path.</param>
        ///         <seealso cref="SaveAs(string)"/>
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
        /// <para> Support includes SixLabors.ImageSharp.Image,  SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats. </para>
        /// <para>Syntax sugar.  Explicit casts already also exist to and from  <see cref="AnyBitmap"/> and all supported types.</para>
        /// </summary>
        /// <typeparam name="T">The Type to cast from.  Support includes SixLabors.ImageSharp.Image,  SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats.</typeparam>
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
        /// <para> Support includes SixLabors.ImageSharp.Image,  SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats. </para>
        /// <para>Syntax sugar.  Explicit casts already also exist to and from  <see cref="AnyBitmap"/> and all supported types.</para>
        /// </summary>
        /// <typeparam name="T">The Type to cast to.  Support includes SixLabors.ImageSharp.Image,  SkiaSharp.SKImage, SkiaSharp.SKBitmap, System.Drawing.Bitmap, System.Drawing.Image and Microsoft.Maui.Graphics formats.</typeparam>

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
        /// <param name="Bytes">A ByteArray of image data in any common format/</param>
        /// <seealso cref="FromBytes"/>
        ///  <seealso cref="AnyBitmap(byte[])"/>
        public static AnyBitmap FromBytes(byte[] Bytes)
        {
            return new AnyBitmap(Bytes);
        }
        /// <summary>
        /// Construct a new Bitmap from binary data (bytes)
        /// </summary>
        /// <param name="Bytes">A ByteArray of image data in any common format/</param>
        /// <seealso cref="FromBytes"/>
        ///  <seealso cref="AnyBitmap"/>

        public AnyBitmap(byte[] Bytes)
        {
            Binary = Bytes;
        }

        /// <summary>
        /// Create a new Bitmap from a <see cref="Stream"/> (bytes)
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/>  of image data in any common format/</param>
        /// <seealso cref="FromStream"/>
        ///  <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(System.IO.Stream Stream)
        {
            return new AnyBitmap(Stream);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes)
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/>  of image data in any common format/</param>
        /// <seealso cref="FromStream"/>
        ///  <seealso cref="AnyBitmap"/>
        public AnyBitmap(System.IO.Stream Stream)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                Binary = ms.ToArray();
            }
        }

        /// <summary>
        /// Create a new Bitmap from a file. (bytes)
        /// </summary>
        /// <param name="File">A fully qualified file path./</param>
        /// <seealso cref="FromFile"/>        
        /// ///  <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromFile(string File)
        {
            return new AnyBitmap(File);
        }

        /// <summary>
        /// Construct a new Bitmap from a file. (bytes)
        /// </summary>
        /// <param name="File">A fully qualified file path./</param>
        /// <seealso cref="FromFile"/>        
        /// ///  <seealso cref="AnyBitmap"/>
        public AnyBitmap(string File)
        {
            Binary = System.IO.File.ReadAllBytes(File);
        }

#if NETSTANDARD
        /// <summary>
        /// Implicitly casts ImageSharp objects to <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/> </param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image Image)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                return new AnyBitmap(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Implicitly casts ImageSharp objects from <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is Implicitly cast to an SixLabors.ImageSharp.Image </param>
        static public implicit operator SixLabors.ImageSharp.Image(AnyBitmap bitmap)
        {
            return SixLabors.ImageSharp.Image.Load(bitmap.Binary);
        }
        
#endif

        /// <summary>
        /// Implicitly casts SkiaSharp.SKImage  objects to <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support SkiaSharp  as well.</para>
        /// </summary>
        /// <param name="Image">SkiaSharp.SKImage  will automatically be cast to <see cref="AnyBitmap"/> </param>
        public static implicit operator AnyBitmap(SkiaSharp.SKImage Image)
        {
#if NETFRAMEWORK
            return new AnyBitmap(SkiaSharp.SKBitmap.FromImage(Image).Bytes);
#else
            return new AnyBitmap(SkiaSharp.SKBitmap.FromImage(Image).Encode(SkiaSharp.SKEncodedImageFormat.Bmp, 100).ToArray());
#endif
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKImage objects from <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support SkiaSharp.SKImage as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is Implicitly cast to an SkiaSharp.SKImage </param>
        static public implicit operator SkiaSharp.SKImage(AnyBitmap bitmap)
        {
            return SkiaSharp.SKImage.FromBitmap(SkiaSharp.SKBitmap.Decode(bitmap.Binary));
        }
        /// <summary>
        /// Implicitly casts SkiaSharp.SKBitmap  objects to <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support SkiaSharp  as well.</para>
        /// </summary>
        /// <param name="Image">SkiaSharp.SKBitmap  will automatically be cast to <see cref="AnyBitmap"/> </param>

        public static implicit operator AnyBitmap(SkiaSharp.SKBitmap Image)
        {
#if NETFRAMEWORK
            return new AnyBitmap(Image.Bytes);
#else
            return new AnyBitmap(Image.Encode(SkiaSharp.SKEncodedImageFormat.Bmp, 100).ToArray());
#endif
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKBitmap objects from <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support SkiaSharp.SKBitmap as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is explicitly cast to an SkiaSharp.SKBitmap </param>
        static public implicit operator SkiaSharp.SKBitmap(AnyBitmap bitmap)
        {
            return SkiaSharp.SKBitmap.Decode(bitmap.Binary);
        }
#if NETSTANDARD
        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Platform.PlatformImage  objects to <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics  as well.</para>
        /// </summary>
        /// <param name="Image">Microsoft.Maui.Graphics.Platform.PlatformImage  will automatically be cast to <see cref="AnyBitmap"/> </param>

        public static implicit operator AnyBitmap(Microsoft.Maui.Graphics.Platform.PlatformImage Image)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                Image.Save(memoryStream);
                return new AnyBitmap(memoryStream.ToArray());
            }
        }
        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Platform.PlatformImage objects from <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics  as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is Implicitly cast to an Microsoft.Maui.Graphics.Platform.PlatformImage  </param>

        static public implicit operator Microsoft.Maui.Graphics.Platform.PlatformImage(AnyBitmap bitmap)
        {
            return (Microsoft.Maui.Graphics.Platform.PlatformImage)Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(bitmap.GetStream());
        }
#endif

        /// <summary>
        /// Implicitly casts System.Drawing.Bitmap objects to <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support System.Drawing.Common  as well.</para>
        /// </summary>
        /// <param name="Image">System.Drawing.Bitmap will automatically be cast to <see cref="AnyBitmap"/> </param>

        public static implicit operator AnyBitmap(System.Drawing.Bitmap Image)
        {
            Byte[] data;

            try
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);

                    data = memoryStream.ToArray();
                    return new AnyBitmap(data);
                }
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
        /// Implicitly casts System.Drawing.Bitmap objects from <see cref="AnyBitmap"/>.  
        /// <para>When your .NET Class methods to use <see cref="AnyBitmap"/> as parameters and return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to an System.Drawing.Bitmap  </param>


        static public implicit operator System.Drawing.Bitmap(AnyBitmap bitmap)
        {
            try
            {
                return (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(new System.IO.MemoryStream(bitmap.Binary));
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

        private static PlatformNotSupportedException SystemDotDrawingPlatformNotSupported(Exception innerException)
        {
            return new PlatformNotSupportedException("Microsoft has chosen to no longer support System.Drawing.Common on Linux or MacOS.  To solve this please use another Bitmap type such as {typeof(Bitmap).ToString()}, SkiaSharp or ImageSharp.\n\nhttps://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only", innerException);
        }

        private static InvalidCastException ImageCastException(string fullTypeName, Exception innerException)
        {
            return new InvalidCastException($"IronSoftware.Drawing does not yet support casting  {fullTypeName} to  {typeof(AnyBitmap).FullName}.  Try using System.Drawing.Common, SkiaSharp or ImageSharp.", innerException);
        }

        private static InvalidOperationException NoConverterException(ImageFormat Format, Exception innerException)
        {
            return new InvalidOperationException($"{typeof(AnyBitmap)} is unable to convert  your image data to {Format.ToString()} because it requires a suitable encoder to be added to your project via Nuget.\nPlease try SkiaSharp, System.Drawing.Common, SixLabors.ImageSharp, Microsoft.Maui.Graphics; or alternatively save using ImageFormat.Default", innerException);
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

            /// <summary> The WBMP image format.  Will default to BMP if not supported on the runtime platform..</summary>
            Wbmp = 5,

            /// <summary> The new WebP image format.</summary>
            Webp = 6,

            /// <summary> The Icon image format.</summary>
            Icon = 7,

            /// <summary> The Wmf image format.</summary>
            Wmf = 8,

            /// <summary> The existing raw image format.</summary>
            Default = -1,
        }
    }
}

