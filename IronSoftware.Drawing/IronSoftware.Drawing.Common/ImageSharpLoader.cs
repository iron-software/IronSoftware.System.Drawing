// .NET Framework has no SixLabors.ImageSharp dependency (see SkiaEngine/), so this ImageSharp-typed API
// exists only on the other targets. AnyBitmap uses SkiaEngine.ImageSharpLoader there instead.
#if !NETFRAMEWORK
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// Decodes images with SixLabors.ImageSharp after rejecting input that the referenced ImageSharp
    /// version cannot decode safely. Use these methods instead of calling
    /// <see cref="Image.Load(byte[])"/> directly on untrusted data.
    /// <para>On .NET Standard 2.0 (used by .NET Framework) ImageSharp 2.x is referenced, which Six Labors
    /// no longer patches. There, BigTIFF data whose image file directory declares more entries than the
    /// data can hold is rejected, because ImageSharp 2.x loops on it without making progress
    /// (CVE-2026-106116, GHSA-wmxv-xphr-5c9g). On .NET 6 and .NET 8 the referenced ImageSharp versions
    /// are patched and these methods behave exactly like <see cref="Image.Load(byte[])"/>.</para>
    /// </summary>
    public static class ImageSharpLoader
    {
        /// <summary>
        /// Decodes an image from <paramref name="data"/>, like <see cref="Image.Load(byte[])"/>.
        /// </summary>
        /// <param name="data">The encoded image.</param>
        /// <returns>The decoded image.</returns>
        /// <exception cref="InvalidImageContentException">The data is a malformed BigTIFF (.NET Standard 2.0 only), or ImageSharp rejects it.</exception>
        public static Image Load(byte[] data)
        {
            ThrowIfUnsafe(data);
            return Image.Load(data);
        }

        /// <summary>
        /// Decodes an image from <paramref name="data"/>, like <see cref="Image.Load{TPixel}(byte[])"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format to decode to.</typeparam>
        /// <param name="data">The encoded image.</param>
        /// <returns>The decoded image.</returns>
        /// <exception cref="InvalidImageContentException">The data is a malformed BigTIFF (.NET Standard 2.0 only), or ImageSharp rejects it.</exception>
        public static Image<TPixel> Load<TPixel>(byte[] data) where TPixel : unmanaged, IPixel<TPixel>
        {
            ThrowIfUnsafe(data);
            return Image.Load<TPixel>(data);
        }

        /// <summary>
        /// Decodes an image from the current position of <paramref name="stream"/>, like
        /// <see cref="Image.Load(Stream)"/>. The remaining stream content is read into memory first.
        /// </summary>
        /// <param name="stream">The stream containing the encoded image.</param>
        /// <returns>The decoded image.</returns>
        /// <exception cref="InvalidImageContentException">The data is a malformed BigTIFF (.NET Standard 2.0 only), or ImageSharp rejects it.</exception>
        public static Image Load(Stream stream)
        {
            return Load(ReadToEnd(stream));
        }

        /// <summary>
        /// Decodes an image from the current position of <paramref name="stream"/>, like
        /// <see cref="Image.Load{TPixel}(Stream)"/>. The remaining stream content is read into memory first.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format to decode to.</typeparam>
        /// <param name="stream">The stream containing the encoded image.</param>
        /// <returns>The decoded image.</returns>
        /// <exception cref="InvalidImageContentException">The data is a malformed BigTIFF (.NET Standard 2.0 only), or ImageSharp rejects it.</exception>
        public static Image<TPixel> Load<TPixel>(Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            return Load<TPixel>(ReadToEnd(stream));
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
        }

        /// <summary>
        /// Rejects BigTIFF data whose image file directory declares more entries than the data can
        /// hold, applying the same bound as the ImageSharp 3.2.0 / 4.1.2 fix. No-op on .NET 6 and later.
        /// </summary>
        /// <exception cref="InvalidImageContentException">A BigTIFF directory entry count exceeds the available data.</exception>
        internal static void ThrowIfUnsafe(byte[] data)
        {
#if !NET6_0_OR_GREATER
            bool littleEndian;
            if (data == null || data.Length < 16)
            {
                return;
            }
            else if (data[0] == 0x49 && data[1] == 0x49 && data[2] == 0x2B && data[3] == 0x00)
            {
                littleEndian = true;
            }
            else if (data[0] == 0x4D && data[1] == 0x4D && data[2] == 0x00 && data[3] == 0x2B)
            {
                littleEndian = false;
            }
            else
            {
                return; // not a BigTIFF
            }

            ulong ReadUInt64(long position)
            {
                ReadOnlySpan<byte> bytes = data.AsSpan((int)position, 8);
                return littleEndian
                    ? System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(bytes)
                    : System.Buffers.Binary.BinaryPrimitives.ReadUInt64BigEndian(bytes);
            }

            // Walk the directory chain the way ImageSharp 2.x does: stop at a zero or out-of-range
            // offset, and leave non-increasing (circular) offsets to ImageSharp, which rejects them.
            ulong offset = ReadUInt64(8);
            for (int directory = 0; offset != 0 && offset < (ulong)data.Length && directory < 1000; directory++)
            {
                // Same bound as the upstream fix: an 8-byte entry count, 20 bytes per entry and an
                // 8-byte next-directory offset must all fit in the data after the directory start.
                long remaining = data.Length - (long)offset - 8;
                ulong count = remaining >= 0 ? ReadUInt64((long)offset) : 0;
                if (remaining < 8 || count > (ulong)((remaining - 8) / 20))
                {
                    throw new InvalidImageContentException("The BigTIFF directory entry count exceeds the available data.");
                }

                ulong next = ReadUInt64((long)offset + 8 + ((long)count * 20));
                if (next <= offset)
                {
                    return;
                }

                offset = next;
            }
#endif
        }
    }
}
#endif
