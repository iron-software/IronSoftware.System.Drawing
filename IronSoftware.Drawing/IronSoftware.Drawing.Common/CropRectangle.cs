using System;

namespace IronSoftware.Drawing
{
    // <summary>
    /// <para>A universally compatible Rectangle for .NET 7 and .NET 6, .NET 5, .NET Core. Windows, NanoServer, IIS,  macOS, Mobile, Xamarin, iOS, Android, Google Compute, Azure, AWS and Linux compatibility.</para>
    /// <para>Works nicely with popular Image Rectangle such as System.Drawing.Rectangle, SkiaSharp.SKRect, SixLabors.ImageSharp.Rectangle, Microsoft.Maui.Graphics.Rect.</para>
    /// <para>Implicit casting means that using this class to input and output Rectangle from public API's gives full compatibility to all Rectangle type fully supported by Microsoft.</para>
    /// </summary>
    public partial class CropRectangle
    {
        /// <summary>
        /// Construct a new CropRectangle.
        /// </summary>
        /// <seealso cref="CropRectangle"/>
        public CropRectangle() { }

        /// <summary>
        /// Construct a new CropRectangle.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of this Rectangle</param>
        /// <param name="y">The y-coordinate of the upper-left corner of this Rectangle</param>
        /// <param name="width">The width of this Rectangle</param>
        /// <param name="height">The height of this Rectangle</param>
        /// <param name="units">The unit of measurement of this Rectangle</param>
        /// <seealso cref="CropRectangle"/>
        public CropRectangle(int x, int y, int width, int height, MeasurementUnits units = MeasurementUnits.Pixels)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Units = units;
        }

        /// <summary>
        /// The x-coordinate of the upper-left corner of this Rectangle. The default is 0.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The y-coordinate of the upper-left corner of this Rectangle. The default is 0.
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The width of this Rectangle. The default is 0.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The height of this Rectangle. The default is 0.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Unit of measurement. The default is Pixels
        /// </summary>
        public MeasurementUnits Units
        {
            get;
            set;
        } = MeasurementUnits.Pixels;

        /// <summary>
        /// Convert this crop rectangle to the specified units of measurement using the specified DPI
        /// </summary>
        /// <param name="units">Unit of measurement</param>
        /// <param name="dpi">DPI for conversion</param>
        /// <returns>A new crop rectangle which uses the desired units of measurement</returns>
        /// <exception cref="NotImplementedException">Conversion not implemented</exception>
        public CropRectangle ConvertTo(MeasurementUnits units, int dpi = 96)
        {
            // no conversion
            if (units == this.Units)
                return this;
            // convert mm to pixels
            if (units == MeasurementUnits.Pixels)
            {
                int x = (int)(((double)this.X / 25.4) * (double)dpi);
                int y = (int)(((double)this.Y / 25.4) * (double)dpi);
                int width = (int)(((double)this.Width / 25.4) * (double)dpi);
                int height = (int)(((double)this.Height / 25.4) * (double)dpi);
                return new CropRectangle(x, y, width, height, MeasurementUnits.Pixels);
            }
            // convert pixels to mm
            if (units == MeasurementUnits.Millimeters)
            {
                int x = (int)((this.X / (double)dpi) * 25.4);
                int y = (int)((this.Y / (double)dpi) * 25.4);
                int width = (int)((this.Width / (double)dpi) * 25.4);
                int height = (int)((this.Height / (double)dpi) * 25.4);
                return new CropRectangle(x, y, width, height, MeasurementUnits.Millimeters);

            }
            throw new NotImplementedException($"CropRectangle conversion from {this.Units} to {units} is not implemented");
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Rectangle objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="Rectangle">System.Drawing.Rectangle will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(System.Drawing.Rectangle Rectangle)
        {
            return new CropRectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Rectangle objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a System.Drawing.Rectangle.</param>
        static public implicit operator System.Drawing.Rectangle(CropRectangle CropRectangle)
        {
            return new System.Drawing.Rectangle(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKRect objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="SKRect">SkiaSharp.SKRect will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(SkiaSharp.SKRect SKRect)
        {
            SkiaSharp.SKRect standardizedSKRect = SKRect.Standardized;
            return CreateCropRectangle((int)standardizedSKRect.Left, (int)standardizedSKRect.Top, (int)standardizedSKRect.Right, (int)standardizedSKRect.Bottom);
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKRect objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a SkiaSharp.SKRect.</param>
        static public implicit operator SkiaSharp.SKRect(CropRectangle CropRectangle)
        {
            return SkiaSharp.SKRect.Create(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKRectI objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRectI as well.</para>
        /// </summary>
        /// <param name="SKRectI">SkiaSharp.SKRectI will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(SkiaSharp.SKRectI SKRectI)
        {
            SkiaSharp.SKRectI standardizedSKRectI = SKRectI.Standardized;
            return CreateCropRectangle((int)standardizedSKRectI.Left, (int)standardizedSKRectI.Top, (int)standardizedSKRectI.Right, (int)standardizedSKRectI.Bottom);
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKRectI objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRectI as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a SkiaSharp.SKRectI.</param>
        static public implicit operator SkiaSharp.SKRectI(CropRectangle CropRectangle)
        {
            return SkiaSharp.SKRectI.Create(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Rectangle objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.Rectangle as well.</para>
        /// </summary>
        /// <param name="Rectangle">SixLabors.ImageSharp.Rectangle will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(SixLabors.ImageSharp.Rectangle Rectangle)
        {
            return new CropRectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Rectangle objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.Rectangle as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a SixLabors.ImageSharp.Rectangle.</param>
        static public implicit operator SixLabors.ImageSharp.Rectangle(CropRectangle CropRectangle)
        {
            return new SixLabors.ImageSharp.Rectangle(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.RectangleF objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.RectangleF as well.</para>
        /// </summary>
        /// <param name="Rectangle">SixLabors.ImageSharp.RectangleF will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(SixLabors.ImageSharp.RectangleF Rectangle)
        {
            return new CropRectangle((int)Rectangle.X, (int)Rectangle.Y, (int)Rectangle.Width, (int)Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.RectangleF objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.RectangleF as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a SixLabors.ImageSharp.RectangleF.</param>
        static public implicit operator SixLabors.ImageSharp.RectangleF(CropRectangle CropRectangle)
        {
            return new SixLabors.ImageSharp.RectangleF(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Rect objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.Rect as well.</para>
        /// </summary>
        /// <param name="Rectangle">Microsoft.Maui.Graphics.Rect will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(Microsoft.Maui.Graphics.Rect Rectangle)
        {
            return new CropRectangle((int)Rectangle.X, (int)Rectangle.Y, (int)Rectangle.Width, (int)Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.Rect objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.Rect as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a Microsoft.Maui.Graphics.Rect.</param>
        static public implicit operator Microsoft.Maui.Graphics.Rect(CropRectangle CropRectangle)
        {
            return new Microsoft.Maui.Graphics.Rect(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.RectF objects to <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.RectF as well.</para>
        /// </summary>
        /// <param name="Rectangle">Microsoft.Maui.Graphics.RectF will automatically be cast to <see cref="CropRectangle"/>.</param>
        public static implicit operator CropRectangle(Microsoft.Maui.Graphics.RectF Rectangle)
        {
            return new CropRectangle((int)Rectangle.X, (int)Rectangle.Y, (int)Rectangle.Width, (int)Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.RectF objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.RectF as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a Microsoft.Maui.Graphics.RectF.</param>
        static public implicit operator Microsoft.Maui.Graphics.RectF(CropRectangle CropRectangle)
        {
            return new Microsoft.Maui.Graphics.RectF(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        #region Private Method

        private static CropRectangle CreateCropRectangle(int left, int top, int right, int bottom)
        {
            return new CropRectangle(left, top, right - left, bottom - top);
        }

        #endregion
    }

    /// <summary>
    /// Units of measurement
    /// </summary>
    public enum MeasurementUnits : int
    {
        /// <summary>
        /// Pixels
        /// </summary>
        Pixels = 0,
        /// <summary>
        /// Millimeters
        /// </summary>
        Millimeters = 1
    }
}
