using System;
using System.IO;

namespace IronSoftware.Drawing
{
public partial class AnyBitmap
    {
        #pragma warning disable CS0618
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
    }
}