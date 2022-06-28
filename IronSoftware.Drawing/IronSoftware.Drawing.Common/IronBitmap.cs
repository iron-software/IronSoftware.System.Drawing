using SkiaSharp;
using System;

namespace IronSoftware.Drawing
{
    public static class IronBitmap
    {
        /// <summary>
        /// Resize an image with scaling.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="scale">Scale of new image 0 - 1.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap Resize(this AnyBitmap bitmap, float scale)
        {
            SKBitmap originalBitmap = bitmap;
            SKBitmap toBitmap = new SKBitmap((int)(originalBitmap.Width * scale), (int)(originalBitmap.Height * scale), originalBitmap.ColorType, originalBitmap.AlphaType);

            using (SKCanvas canvas = new SKCanvas(toBitmap))
            {
                // Draw a bitmap rescaled
#if NETFRAMEWORK
                canvas.SetMatrix(SKMatrix.MakeScale(scale, scale));
#else
                canvas.SetMatrix(SKMatrix.CreateScale(scale, scale));
#endif
                canvas.DrawBitmap(originalBitmap, 0, 0);
                canvas.ResetMatrix();
                canvas.Flush();
            }

            return toBitmap;
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="width">Width of the new resized image.</param>
        /// <param name="height">Height of the new resized image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap Resize(this AnyBitmap bitmap, int width, int height)
        {
            SKBitmap originalBitmap = bitmap;
            SKBitmap toBitmap = new SKBitmap(width, height, originalBitmap.ColorType, originalBitmap.AlphaType);
            originalBitmap.ExtractSubset(toBitmap, new CropRectangle(0, 0, width, height));

            return toBitmap;
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="width">Width of the new resized image.</param>
        /// <param name="height">Height of the new resized image.</param>
        /// <param name="scale">Scale of new image 0 - 1.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap Resize(this AnyBitmap bitmap, int width, int height, float scale)
        {
            SKBitmap originalBitmap = bitmap;
            SKBitmap toBitmap = new SKBitmap(width, height, originalBitmap.ColorType, originalBitmap.AlphaType);

            using (SKCanvas canvas = new SKCanvas(toBitmap))
            {
                // Draw a bitmap rescaled
#if NETFRAMEWORK
                canvas.SetMatrix(SKMatrix.MakeScale(scale, scale));
#else
                canvas.SetMatrix(SKMatrix.CreateScale(scale, scale));
#endif
                canvas.DrawBitmap(originalBitmap, 0, 0);
                canvas.ResetMatrix();
                canvas.Flush();
            }

            return toBitmap;
        }

        /// <summary>
        /// Resize an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="cropRect">CropArea to crop an image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap CropImage(this AnyBitmap bitmap, CropRectangle cropRect)
        {
            if (cropRect != null)
            {
                CropRectangle dest = ValidateCropArea(bitmap, cropRect);
                SKBitmap croppedBitmap = new SKBitmap((int)dest.Width, (int)dest.Height);
                try
                {
                    using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                    {
                        canvas.DrawBitmap(bitmap, cropRect, dest);
                        canvas.Flush();
                    }

                    return croppedBitmap;
                }
                catch (OutOfMemoryException)
                {
                    try { croppedBitmap.Dispose(); } catch { }
                    throw new Exception("Crop Rectangle is larger than the input image.");
                }
            }
            else
            {
                return bitmap;
            }
        }

        /// <summary>
        /// Rotate an image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to resize.</param>
        /// <param name="angle">Angle for rotate image. Default (null): Will try to determine the image's rotation angle.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap RotateImage(this AnyBitmap bitmap, double? angle = null)
        {
            double skewAngle = angle ?? SkewImageLib.GetSkewAngle(bitmap);
            double radians = Math.PI * skewAngle / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));

            int originalWidth = ((SKBitmap)bitmap).Width;
            int originalHeight = ((SKBitmap)bitmap).Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            SKBitmap rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (SKCanvas canvas = new SKCanvas(rotatedBitmap))
            {
                canvas.Clear(SKColors.LightPink);
                canvas.Translate(rotatedWidth / 2, rotatedHeight / 2);
                canvas.RotateDegrees((float)angle);
                canvas.Translate(-originalWidth / 2, -originalHeight / 2);
                canvas.DrawBitmap(bitmap, new SKPoint());
            }

            return rotatedBitmap;
        }

        /// <summary>
        /// Determine the image's skew angle.
        /// </summary>
        /// <param name="bitmap">Original bitmap to get skew angle from.</param>
        /// <return>Image's angle of skew.</return>
        public static double GetSkewAngle(this AnyBitmap bitmap)
        {
            return SkewImageLib.GetSkewAngle(bitmap);
        }

        /// <summary>
        /// Trim white space of the image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to trim.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static SKBitmap Trim(this AnyBitmap bitmap)
        {

            try
            {
                SKBitmap originalBitmap = bitmap;
                int[] rgbValues = new int[originalBitmap.Height * originalBitmap.Width];

                // Determine Left
                int newLeft = -1;
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        SKColor color = originalBitmap.GetPixel(x, y);
                        if (color != SKColors.White)
                        {
                            newLeft = x;
                            break;
                        }
                    }
                    if (newLeft != -1)
                        break;
                }
                // Determine Right
                int newRight = -1;
                for (int x = originalBitmap.Width - 1; x >= 0; x--)
                {
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        SKColor color = originalBitmap.GetPixel(x, y);
                        if (color != SKColors.White)
                        {
                            newRight = x;
                            break;
                        }
                    }
                    if (newRight != -1)
                        break;
                }
                // Determine Bottom
                int newBottom = -1;
                for (int y = 0; y < originalBitmap.Height; y++)
                {
                    for (int x = 0; x < originalBitmap.Width; x++)
                    {
                        SKColor color = originalBitmap.GetPixel(x, y);
                        if (color != SKColors.White)
                        {
                            newBottom = x;
                            break;
                        }
                    }
                    if (newBottom != -1)
                        break;
                }
                // Determine Top
                int newTop = -1;
                for (int y = originalBitmap.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < originalBitmap.Width; x++)
                    {
                        SKColor color = originalBitmap.GetPixel(x, y);
                        if (color != SKColors.White)
                        {
                            newTop = x;
                            break;
                        }
                    }
                    if (newTop != -1)
                        break;
                }

                return CropImage(bitmap, new SKRect(newLeft, newTop, newRight, newBottom));
            }
            catch
            {
                return bitmap.Clone();
            }
        }

        /// <summary>
        /// Add a colored border around the image.
        /// </summary>
        /// <param name="bitmap">Original bitmap to add a border to.</param>
        /// <param name="color">Color of the border.</param>
        /// <param name="width">Width of the border in pixel.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap AddBorder(this AnyBitmap bitmap, IronSoftware.Drawing.Color color, int width)
        {
            SKBitmap originalBitmap = bitmap;
            int maxWidth = originalBitmap.Width + width * 2;
            int maxHeight = originalBitmap.Height + width * 2;
            SKBitmap toBitmap = new SKBitmap(maxWidth, maxHeight);

            var ratioX = (double)maxWidth / originalBitmap.Width;
            var ratioY = (double)maxHeight / originalBitmap.Height;
            var scale = (float)Math.Min(ratioX, ratioY);

            using (SKCanvas canvas = new SKCanvas(toBitmap))
            {
#if NETFRAMEWORK
                canvas.SetMatrix(SKMatrix.MakeScale(scale, scale));
#else
                canvas.SetMatrix(SKMatrix.CreateScale(scale, scale));
#endif
                canvas.Clear(color);
                int x = (toBitmap.Width - originalBitmap.Width) / 2;
                int y = (toBitmap.Height - originalBitmap.Height) / 2;
                canvas.DrawBitmap(bitmap, x, y);
                canvas.ResetMatrix();
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
#endregion
    }
}
