using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
        private Image Image { get; set; }
        private byte[] Binary { get; set; }

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
        /// Exports the Bitmap as a Func<<see cref="MemoryStream"/>> encoded in the <see cref="ImageFormat"/> of your choice.
        /// <para>Add SkiaSharp, System.Drawing.Common, or SixLabors.ImageSharp to your project to enable the encoding feature.</para>
        /// </summary>
        /// <param name="Format">An image encoding format.</param>
        /// <param name="Lossy">JPEG and WebP encoding quality (ignored for all other values of <see cref="ImageFormat"/>). Higher values return larger file sizes. 0 is lowest quality, 100 is highest.</param>
        /// <returns>Transcoded image bytes in a Func<<see cref="MemoryStream"/>>.</returns>
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
            if (Format == ImageFormat.Default)
            {
                var writer = new BinaryWriter(Stream);
                writer.Write(Binary);
                return;
            }

            if (Lossy < 0 || Lossy > 100) { Lossy = 100; }

            List<Exception> exceptions = TryExportStream(Stream, Format, Lossy);
            if (exceptions != null && exceptions.Count > 0)
                throw NoConverterException(Format, exceptions);
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
        /// <seealso cref="FromStream"/>
        /// <seealso cref="AnyBitmap"/>
        public static AnyBitmap FromStream(System.IO.MemoryStream Stream)
        {
            return new AnyBitmap(Stream);
        }

        /// <summary>
        /// Construct a new Bitmap from a <see cref="Stream"/> (bytes).
        /// </summary>
        /// <param name="Stream">A <see cref="Stream"/> of image data in any common format.</param>
        /// <seealso cref="FromStream"/>
        /// <seealso cref="AnyBitmap"/>
        public AnyBitmap(System.IO.MemoryStream Stream)
        {
            LoadImage(Stream.ToArray());
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
        /// Implicitly casts ImageSharp objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24> Image)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            using (var memoryStream = new System.IO.MemoryStream())
            {
                Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                return new AnyBitmap(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Implicitly casts ImageSharp objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SixLabors.ImageSharp.Image.</param>
        static public implicit operator SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24>(AnyBitmap bitmap)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(bitmap.Binary);
        }

        /// <summary>
        /// Implicitly casts ImageSharp objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> Image)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            using (var memoryStream = new System.IO.MemoryStream())
            {
                Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                return new AnyBitmap(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Implicitly casts ImageSharp objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SixLabors.ImageSharp.Image.</param>
        static public implicit operator SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(AnyBitmap bitmap)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bitmap.Binary);
        }

        /// <summary>
        /// Implicitly casts ImageSharp objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="Image">SixLabors.ImageSharp.Image will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SixLabors.ImageSharp.Image Image)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            using (var memoryStream = new System.IO.MemoryStream())
            {
                Image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder());
                return new AnyBitmap(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Implicitly casts ImageSharp objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support ImageSharp as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to a SixLabors.ImageSharp.Image.</param>
        static public implicit operator SixLabors.ImageSharp.Image(AnyBitmap bitmap)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(bitmap.Binary);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKImage objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp as well.</para>
        /// </summary>
        /// <param name="Image">SkiaSharp.SKImage will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SkiaSharp.SKImage Image)
        {
            if (!IsLoadedType("SkiaSharp.SKImage"))
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.");
            }

            return new AnyBitmap(Image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKImage objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp.SKImage as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to an SkiaSharp.SKImage.</param>
        static public implicit operator SkiaSharp.SKImage(AnyBitmap bitmap)
        {
            if (!IsLoadedType("SkiaSharp.SKImage"))
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.");
            }

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
        /// <summary>
        /// Implicitly casts SkiaSharp.SKBitmap objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp as well.</para>
        /// </summary>
        /// <param name="Image">SkiaSharp.SKBitmap will automatically be cast to <see cref="AnyBitmap"/>.</param>
        public static implicit operator AnyBitmap(SkiaSharp.SKBitmap Image)
        {
            if (!IsLoadedType("SkiaSharp.SKBitmap"))
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.");
            }

            return new AnyBitmap(Image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKBitmap objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support SkiaSharp.SKBitmap as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is explicitly cast to a SkiaSharp.SKBitmap.</param>
        static public implicit operator SkiaSharp.SKBitmap(AnyBitmap bitmap)
        {
            if (!IsLoadedType("SkiaSharp.SKBitmap"))
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.");
            }

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

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Platform.PlatformImage objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support Microsoft.Maui.Graphics as well.</para>
        /// </summary>
        /// <param name="Image">Microsoft.Maui.Graphics.Platform.PlatformImage will automatically be cast to <see cref="AnyBitmap"/>.</param>

        public static implicit operator AnyBitmap(Microsoft.Maui.Graphics.Platform.PlatformImage Image)
        {
            if (!IsLoadedType("Microsoft.Maui.Graphics.Platform.PlatformImage"))
            {
                throw new DllNotFoundException("Please install Microsoft.Maui.Graphics from NuGet.");
            }

            using (var memoryStream = new System.IO.MemoryStream())
            {
                Image.Save(memoryStream);
                return new AnyBitmap(memoryStream.ToArray());
            }
        }
        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Platform.PlatformImage objects from <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support Microsoft.Maui.Graphics as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to an Microsoft.Maui.Graphics.Platform.PlatformImage.</param>

        static public implicit operator Microsoft.Maui.Graphics.Platform.PlatformImage(AnyBitmap bitmap)
        {
            if (!IsLoadedType("Microsoft.Maui.Graphics.Platform.PlatformImage"))
            {
                throw new DllNotFoundException("Please install Microsoft.Maui.Graphics from NuGet.");
            }

            return (Microsoft.Maui.Graphics.Platform.PlatformImage)Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(bitmap.GetStream());
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Bitmap objects to <see cref="AnyBitmap"/>.
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="Image">System.Drawing.Bitmap will automatically be cast to <see cref="AnyBitmap"/> </param>

        public static implicit operator AnyBitmap(System.Drawing.Bitmap Image)
        {
            if (!IsLoadedType("System.Drawing.Bitmap"))
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.");
            }

            Byte[] data;
            try
            {
                System.Drawing.Bitmap blank = new System.Drawing.Bitmap(Image.Width, Image.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(blank);
                g.Clear(Color.White);
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
        /// <para>When your .NET Class methods use <see cref="AnyBitmap"/> as parameters or return types, you now automatically support System.Drawing.Common as well.</para>
        /// </summary>
        /// <param name="bitmap"><see cref="AnyBitmap"/> is implicitly cast to an System.Drawing.Bitmap.</param>
        static public implicit operator System.Drawing.Bitmap(AnyBitmap bitmap)
        {
            if (!IsLoadedType("System.Drawing.Bitmap"))
            {
                throw new DllNotFoundException("Please install System.Drawing from NuGet.");
            }

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

            /// <summary> The existing raw image format.</summary>
            Default = -1,
        }

#region Private Method

        private void LoadImage(byte[] Bytes)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            Image = Image.Load(Bytes);
            Binary = Bytes;
        }

        private void LoadImage(string File)
        {
            if (!IsLoadedType("SixLabors.ImageSharp.Image"))
            {
                throw new DllNotFoundException("Please install SixLabors.ImageSharp from NuGet.");
            }

            Image = Image.Load(File);
            Binary = System.IO.File.ReadAllBytes(File);
        }

        private static AnyBitmap LoadSVGImage(string File)
        {
            if (!IsLoadedType("SkiaSharp.SKBitmap"))
            {
                throw new DllNotFoundException("Please install SkiaSharp from NuGet.");
            }

            return new AnyBitmap(DecodeSVG(File).Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).ToArray());
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
            if (IsLoadedType("BitMiracle.LibTiff.Classic.Tiff"))
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
                catch { }

                return anyBitmap;
            }
            else
            {
                throw new DllNotFoundException("Please install BitMiracle.LibTiff.NET from NuGet.");
            }
        }

        #endregion
    }
}

