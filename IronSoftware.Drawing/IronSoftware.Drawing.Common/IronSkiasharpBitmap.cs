using AForge.Imaging.Filters;
using BitMiracle.LibTiff.Classic;
using SkiaSharp;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// For internal usage
    /// </summary>
    public static class IronSkiasharpBitmap
    {
        /// <summary>
        /// Resize an image with scaling.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="scale">Scale of new image 0 - 1.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Resize(this SKBitmap bitmap, float scale)
        {
            if (bitmap != null)
            {
                SKBitmap toBitmap = new SKBitmap((int)(bitmap.Width * scale), (int)(bitmap.Height * scale), bitmap.ColorType, bitmap.AlphaType);

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.SetMatrix(SKMatrix.CreateScale(scale, scale));
                    canvas.DrawBitmap(bitmap, 0, 0, CreateHighQualityPaint());
                    canvas.ResetMatrix();
                    canvas.Flush();
                }

                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="width">Width of the new resized image.</param>
        /// <param name="height">Height of the new resized image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height)
        {
            if (bitmap != null)
            {
                SKBitmap toBitmap = new SKBitmap(width, height, bitmap.ColorType, bitmap.AlphaType);

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.SetMatrix(SKMatrix.CreateScale(CalculateScaleOfWidth(bitmap, width), CalculateScaleOfHeight(bitmap, height)));
                    canvas.DrawBitmap(bitmap, 0, 0, CreateHighQualityPaint());
                    canvas.ResetMatrix();
                    canvas.Flush();
                }
                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to crop.</param>
        /// <param name="cropArea">Crop area for the image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap CropImage(this SKBitmap bitmap, CropRectangle cropArea)
        {
            if (cropArea != null && bitmap != null)
            {
                SKRect cropRect = ValidateCropArea(bitmap, cropArea);
                SKBitmap croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);

                SKRect dest = new SKRect(0, 0, cropRect.Width, cropRect.Height);
                SKRect source = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);

                try
                {
                    using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                    {
                        canvas.DrawBitmap(bitmap, source, dest, CreateHighQualityPaint());
                    }

                    return croppedBitmap;
                }
                catch (OutOfMemoryException ex)
                {
                    try { croppedBitmap.Dispose(); } catch { }
                    throw new Exception("Crop Rectangle is larger than the input image.", ex);
                }
            }
            else
            {
                throw new Exception("Please provide a bitmap and crop area to process.");
            }
        }

        /// <summary>
        /// Crop an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to crop.</param>
        /// <param name="width">Width of the new cropped image.</param>
        /// <param name="height">Height of the new cropped image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap CropImage(this SKBitmap bitmap, int width, int height)
        {

            if (bitmap != null)
            {
                SKBitmap toBitmap = new SKBitmap(width, height, bitmap.ColorType, bitmap.AlphaType);
                bitmap.ExtractSubset(toBitmap, new CropRectangle(0, 0, width, height));

                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Rotate an image. 
        /// </summary>
        /// <param name="bitmap">Original bitmap to rotate.</param>
        /// <param name="angle">Angle to rotate the image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap RotateImage(this SKBitmap bitmap, double angle)
        {
            if (bitmap != null)
            {
                double radians = Math.PI * angle / 180;
                float sine = (float)Math.Abs(Math.Sin(radians));
                float cosine = (float)Math.Abs(Math.Cos(radians));

                int originalWidth = (bitmap).Width;
                int originalHeight = (bitmap).Height;
                int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
                int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

                SKBitmap rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

                using (SKCanvas canvas = new SKCanvas(rotatedBitmap))
                {
                    canvas.Clear(SKColors.White);
                    canvas.Translate(rotatedWidth / 2, rotatedHeight / 2);
                    canvas.RotateDegrees((float)angle);
                    canvas.Translate(-originalWidth / 2, -originalHeight / 2);
                    canvas.DrawBitmap(bitmap, new SKPoint(), CreateHighQualityPaint());
                }

                return rotatedBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        public static double GetSkewAngle(this SKBitmap bitmap, int? MaxAngle = null)
        {
            SKBitmap grayScale = bitmap.GrayScale();
            SKBitmap toBitmap = new SKBitmap(grayScale.Info);

            using (SKCanvas canvas = new SKCanvas(toBitmap))
            {
                canvas.DrawBitmap(grayScale, new SKPoint(), CreateHighQualityPaint());
                return SkewImageLib.GetSkewAngle(toBitmap);
            }
        }

        /// <summary>
        /// Trim white space of the image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to trim.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Trim(this SKBitmap bitmap)
        {
            if (bitmap != null)
            {
                int[] rgbValues = new int[bitmap.Height * bitmap.Width];
                Marshal.Copy(bitmap.GetPixels(), rgbValues, 0, rgbValues.Length);

                int left = bitmap.Width;
                int top = bitmap.Height;
                int right = 0;
                int bottom = 0;

                DetermineTop(bitmap, rgbValues, ref left, ref top, ref right, ref bottom);
                DetermineBottom(bitmap, rgbValues, ref left, ref right, ref bottom);

                if (bottom > top)
                {
                    DetermineLeftAndRight(bitmap, rgbValues, ref left, top, ref right, bottom);
                }

                return bitmap.CropImage(new SKRect(left, top, right, bottom));
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        /// <summary>
        /// Add a colored border around the image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to add a border to.</param>
        /// <param name="color">Color of the border.</param>
        /// <param name="width">Width of the border in pixel.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap AddBorder(this SKBitmap bitmap, IronSoftware.Drawing.Color color, int width)
        {
            if (bitmap != null)
            {
                int maxWidth = bitmap.Width + width * 2;
                int maxHeight = bitmap.Height + width * 2;
                SKBitmap toBitmap = new SKBitmap(maxWidth, maxHeight);

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.Clear(color);
                    SKRect dest = new SKRect(width, width, width + bitmap.Width, width + bitmap.Height);
                    canvas.DrawBitmap(bitmap, dest, CreateHighQualityPaint());
                    canvas.Flush();
                }

                return toBitmap;
            }
            else
            {
                throw new Exception("Please provide a bitmap to process.");
            }
        }

        public static SkiaSharp.SKBitmap OpenTiffToSKBitmap(string imagePath)
        {
            return OpenTiffToSKBitmap(File.ReadAllBytes(imagePath));
        }

        public static SkiaSharp.SKBitmap OpenTiffToSKBitmap(byte[] bytes)
        {
            return OpenTiffToSKBitmap(new MemoryStream(bytes));
        }

        public static SkiaSharp.SKBitmap OpenTiffToSKBitmap(MemoryStream tiffStream)
        {
            try
            {
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public static SkiaSharp.SKBitmap Sharpen(this SkiaSharp.SKBitmap bitmap, int grayScale = 50)
        {
            SkiaSharp.SKBitmap result = new SkiaSharp.SKBitmap(bitmap.Width, bitmap.Height);
            bitmap.CopyTo(result);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if (color.GetLuminance() > grayScale)
                    {
                        result.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        result.SetPixel(i, j, Color.Black);
                    }
                }
            }
            bitmap.Dispose();

            return result;
        }

        public static SkiaSharp.SKBitmap GrayScale(this SkiaSharp.SKBitmap bitmap)
        {
            SKImageInfo info = new SKImageInfo(bitmap.Width, bitmap.Height);
            SKBitmap toBitmap = new SKBitmap(info);

            using (SKPaint paint = new SKPaint())
            {
                paint.ColorFilter =
                    SKColorFilter.CreateColorMatrix(new float[]
                    {
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0.21f, 0.72f, 0.07f, 0, 0,
                    0,     0,     0,     1, 0
                    });

                using (SKCanvas canvas = new SKCanvas(toBitmap))
                {
                    canvas.DrawBitmap(bitmap, info.Rect, paint);
                }
            }

            return toBitmap;
        }

        internal static SkiaSharp.SKBitmap DecodeSVG(string strInput)
        {
            SkiaSharp.Extended.Svg.SKSvg svg = new SkiaSharp.Extended.Svg.SKSvg();
            svg.Load(strInput);

            SkiaSharp.SKBitmap toBitmap = new SkiaSharp.SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
            using (SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(toBitmap))
            {
                canvas.Clear(SKColors.White);
                canvas.DrawPicture(svg.Picture);
                canvas.Flush();
            }

            return toBitmap;
        }

        #region Private Method

        private static CropRectangle ValidateCropArea(SKBitmap img, CropRectangle CropArea)
        {
            int maxWidth = img.Width;
            int maxHeight = img.Height;

            int cropAreaX = CropArea.X > 0 ? CropArea.X : 0;
            int cropAreaY = CropArea.Y > 0 ? CropArea.Y : 0;
            int cropAreaWidth = CropArea.Width > 0 ? CropArea.Width : img.Width;
            int cropAreaHeight = CropArea.Height > 0 ? CropArea.Height : img.Height;

            int croppedWidth = cropAreaX + cropAreaWidth;
            int croppedHeight = cropAreaY + cropAreaHeight;

            int newWidth = cropAreaWidth;
            int newHeight = cropAreaHeight;
            if (croppedWidth > maxWidth)
            {
                newWidth = maxWidth - cropAreaX;
            }
            if (croppedHeight > maxHeight)
            {
                newHeight = maxHeight - cropAreaY;
            }
            return new CropRectangle(cropAreaX, cropAreaY, newWidth, newHeight);
        }

        private static void DetermineLeftAndRight(SKBitmap originalBitmap, int[] rgbValues, ref int left, int top, ref int right, int bottom)
        {
            for (int r = top + 1; r < bottom; r++)
            {
                DetermineLeft(originalBitmap, rgbValues, ref left, r);
                DetermineRight(originalBitmap, rgbValues, ref right, r);
            }
        }

        private static void DetermineRight(SKBitmap originalBitmap, int[] rgbValues, ref int right, int r)
        {
            for (int c = originalBitmap.Width - 1; c > right; c--)
            {
                int color = rgbValues[r * originalBitmap.Width + c] & 0xffffff;
                if (color != 0xffffff)
                {
                    if (right < c)
                    {
                        right = c;
                        break;
                    }
                }
            }
        }

        private static void DetermineLeft(SKBitmap originalBitmap, int[] rgbValues, ref int left, int r)
        {
            for (int c = 0; c < left; c++)
            {
                int color = rgbValues[r * originalBitmap.Width + c] & 0xffffff;
                if (color != 0xffffff)
                {
                    if (left > c)
                    {
                        left = c;
                        break;
                    }
                }
            }
        }

        private static void DetermineBottom(SKBitmap originalBitmap, int[] rgbValues, ref int left, ref int right, ref int bottom)
        {
            for (int i = rgbValues.Length - 1; i >= 0; i--)
            {
                int color = rgbValues[i] & 0xffffff;
                if (color != 0xffffff)
                {
                    int r = i / originalBitmap.Width;
                    int c = i % originalBitmap.Width;

                    if (left > c)
                    {
                        left = c;
                    }
                    if (right < c)
                    {
                        right = c;
                    }
                    bottom = r;
                    break;
                }
            }
        }

        private static void DetermineTop(SKBitmap originalBitmap, int[] rgbValues, ref int left, ref int top, ref int right, ref int bottom)
        {
            for (int i = 0; i < rgbValues.Length; i++)
            {
                int color = rgbValues[i] & 0xffffff;
                if (color != 0xffffff)
                {
                    int r = i / originalBitmap.Width;
                    int c = i % originalBitmap.Width;

                    if (left > c)
                    {
                        left = c;
                    }
                    if (right < c)
                    {
                        right = c;
                    }
                    bottom = r;
                    top = r;
                    break;
                }
            }
        }

        private static float CalculateScaleOfWidth(SKBitmap originalBitmap, int width)
        {
            return (float)width / originalBitmap.Width;
        }

        private static float CalculateScaleOfHeight(SKBitmap originalBitmap, int height)
        {
            return (float)height / originalBitmap.Height;
        }

        private static SKPaint CreateHighQualityPaint()
        {
            return new SKPaint()
            {
                FilterQuality = SKFilterQuality.High
            };
        }

        #endregion
    }
}
