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

            float scale = CalculateImageScale(originalBitmap, width, height);

            using (SKCanvas canvas = new SKCanvas(toBitmap))
            {
                // Draw a bitmap rescaled
#if NETFRAMEWORK
                canvas.SetMatrix(SKMatrix.MakeScale(CalculateScaleOfWidth(originalBitmap, width), CalculateScaleOfHeight(originalBitmap, height)));
#else
                canvas.SetMatrix(SKMatrix.CreateScale(CalculateScaleOfWidth(originalBitmap, width), CalculateScaleOfHeight(originalBitmap, height)));
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
        /// <param name="bitmap">Original bitmap to crop.</param>
        /// <param name="cropArea">Crop area for the image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap CropImage(this AnyBitmap bitmap, CropRectangle cropArea)
        {
            if (cropArea != null)
            {
                SKRect cropRect = ValidateCropArea(bitmap, cropArea);
                SKBitmap croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);

                SKRect dest = new SKRect(0, 0, cropRect.Width, cropRect.Height);
                SKRect source = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);
                try
                {
                    using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                    {
                        canvas.DrawBitmap(bitmap, source, dest);
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
        /// Crop an image with width and height.
        /// </summary>
        /// <param name="bitmap">Original bitmap to crop.</param>
        /// <param name="width">Width of the new cropped image.</param>
        /// <param name="height">Height of the new cropped image.</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap CropImage(this AnyBitmap bitmap, int width, int height)
        {
            SKBitmap originalBitmap = bitmap;
            SKBitmap toBitmap = new SKBitmap(width, height, originalBitmap.ColorType, originalBitmap.AlphaType);
            originalBitmap.ExtractSubset(toBitmap, new CropRectangle(0, 0, width, height));

            return toBitmap;
        }

        /// <summary>
        /// Rotate an image. 
        /// </summary>
        /// <param name="bitmap">Original bitmap to rotate.</param>
        /// <param name="angle">Angle to rotate the image. Default (null): Will attempt to deskew image if it is not aligned (Required: System.Drawing.Bitmap).</param>
        /// <return>IronSoftware.Drawing.AnyBitmap.</return>
        public static AnyBitmap RotateImage(this AnyBitmap bitmap, double? angle = null)
        {
            double skewAngle = angle ?? (-1 * GetSkewAngle(bitmap));
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
                canvas.Clear();
                canvas.Translate(rotatedWidth / 2, rotatedHeight / 2);
                canvas.RotateDegrees((float)skewAngle);
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
        public static double DetermineSkewAngle(AnyBitmap bitmap)
        {
            // TODO find Image's angle
            // Now the working solution is AForge.Imaging.DocumentSkewChecker but it required System.Drawing.Bitmap
            return GetSkewAngle(bitmap);
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

                int newLeft = DetermineLeft(originalBitmap);
                int newRight = DetermineRight(originalBitmap);
                int newBottom = DetermineBottom(originalBitmap);
                int newTop = DetermineTop(originalBitmap);

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

            float scale = CalculateImageScale(originalBitmap, maxWidth, maxHeight);

            using (SKCanvas canvas = new SKCanvas(toBitmap))
            {
                canvas.Clear(color);
                SKRect dest = new SKRect(width, width, width + originalBitmap.Width, width + originalBitmap.Height);
                canvas.DrawBitmap(originalBitmap, dest);
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

        private static bool DifferentColor(Color source, Color target)
        {
            return !IsTransparent(source) && (source.R != target.R || source.G != target.G || source.B != target.B || source.A != target.A);
        }

        private static bool IsTransparent(Color source)
        {
            return (SKColor)source == SKColors.Transparent;
        }

        private static int DetermineRight(SKBitmap originalBitmap)
        {
            int result = -1;
            for (int x = originalBitmap.Width - 1; x >= 0; x--)
            {
                for (int y = 0; y < originalBitmap.Height; y++)
                {
                    SKColor color = originalBitmap.GetPixel(x, y);
                    if (color != SKColors.White)
                    {
                        result = x;
                        break;
                    }
                }
                if (result != -1)
                    break;
            }

            return result;
        }

        private static int DetermineLeft(SKBitmap originalBitmap)
        {
            int result = -1;
            for (int x = 0; x < originalBitmap.Width; x++)
            {
                for (int y = 0; y < originalBitmap.Height; y++)
                {
                    SKColor color = originalBitmap.GetPixel(x, y);
                    if (DifferentColor(color, Color.White))
                    {
                        result = x;
                        break;
                    }
                }
                if (result != -1)
                    break;
            }

            return result;
        }

        private static int DetermineTop(SKBitmap originalBitmap)
        {
            int newTop = -1;
            for (int y = originalBitmap.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    SKColor color = originalBitmap.GetPixel(x, y);
                    if (DifferentColor(color, Color.White))
                    {
                        newTop = y;
                        break;
                    }
                }
                if (newTop != -1)
                    break;
            }

            return newTop;
        }

        private static int DetermineBottom(SKBitmap originalBitmap)
        {
            int newBottom = -1;
            for (int y = 0; y < originalBitmap.Height; y++)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    SKColor color = originalBitmap.GetPixel(x, y);
                    if (DifferentColor(color, Color.White))
                    {
                        newBottom = y;
                        break;
                    }
                }
                if (newBottom != -1)
                    break;
            }

            return newBottom;
        }

        private static float CalculateImageScale(SKBitmap originalBitmap, int width, int height)
        {
            float ratioX = CalculateScaleOfWidth(originalBitmap, width);
            float ratioY = CalculateScaleOfHeight(originalBitmap, height);
            return Math.Min(ratioX, ratioY);
        }

        private static float CalculateScaleOfWidth(SKBitmap originalBitmap, int width)
        {
            return (float)width / originalBitmap.Width;
        }

        private static float CalculateScaleOfHeight(SKBitmap originalBitmap, int height)
        {
            return (float)height / originalBitmap.Height;
        }

        private static double GetSkewAngle(System.Drawing.Bitmap inputImage, int? MaxAngle = null)
        {
            System.Drawing.Bitmap image;
            if (inputImage.PixelFormat.ToString().Equals("Format8bppIndexed"))
            {
                image = (System.Drawing.Bitmap)inputImage.Clone();
            }
            else
            {
                image = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(inputImage);
            }

            AForge.Imaging.DocumentSkewChecker skewChecker = new AForge.Imaging.DocumentSkewChecker();
            skewChecker.MaxSkewToDetect = MaxAngle.HasValue ? MaxAngle.Value : 270;

            // get documents skew angle
            double angle = skewChecker.GetSkewAngle(image);
            image.Dispose();

            return angle;
        }

        #endregion
    }
}
