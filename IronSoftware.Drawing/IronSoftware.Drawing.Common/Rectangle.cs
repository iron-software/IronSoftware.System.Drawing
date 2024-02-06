using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// A universally compatible Rectangle for .NET 7, .NET 6, .NET 5, and .NET Core. As well as compatibility with Windows, NanoServer, IIS, macOS, Mobile, Xamarin, iOS, Android, Google Compute, Azure, AWS, and Linux.
    /// <para>Works nicely with popular Image Rectangle such as System.Drawing.Rectangle, SkiaSharp.SKRect, SixLabors.ImageSharp.Rectangle, Microsoft.Maui.Graphics.Rect.</para>
    /// <para>Implicit casting means that using this class to input and output Rectangle from public APIs gives full compatibility to all Rectangle type fully supported by Microsoft.</para>
    /// </summary>
    public class Rectangle
    {
        /// <summary>
        /// Construct a new Rectangle.
        /// </summary>
        /// <seealso cref="Rectangle"/>
        public Rectangle() { }

        /// <summary>
        /// Construct a new Rectangle.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of this Rectangle</param>
        /// <param name="y">The y-coordinate of the upper-left corner of this Rectangle</param>
        /// <param name="width">The width of this Rectangle</param>
        /// <param name="height">The height of this Rectangle</param>
        /// <param name="units">The measurement unit of this Rectangle</param>
        /// <seealso cref="Rectangle"/>
        public Rectangle(int x, int y, int width, int height, MeasurementUnits units = MeasurementUnits.Pixels)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Units = units;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="point">
        /// The <see cref="Point"/> which specifies the rectangles point in a two-dimensional plane.
        /// </param>
        /// <param name="size">
        /// The <see cref="Size"/> which specifies the rectangles height and width.
        /// </param>
        /// <param name="units">The measurement unit of this Rectangle</param>
        /// <seealso cref="Rectangle"/>
        public Rectangle(Point point, Size size, MeasurementUnits units = MeasurementUnits.Pixels)
        {
            X = (int)point.X;
            Y = (int)point.Y;
            Width = size.Width;
            Height = size.Height;
            Units = units;
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
        /// The measurement unit of this Rectangle. The default is Pixels
        /// </summary>
        public MeasurementUnits Units
        {
            get;
            set;
        } = MeasurementUnits.Pixels;

        /// <summary>
        /// Convert this rectangle to the specified units of measurement using the specified DPI
        /// </summary>
        /// <param name="units">Unit of measurement</param>
        /// <param name="dpi">DPI (Dots per inch) for conversion</param>
        /// <returns>A new rectangle which uses the desired units of measurement</returns>
        /// <exception cref="NotImplementedException">Conversion not implemented</exception>
        public Rectangle ConvertTo(MeasurementUnits units, int dpi = 96)
        {
            // no conversion
            if (units == Units)
            {
                return this;
            }
            // convert mm to pixels
            if (units == MeasurementUnits.Pixels)
            {
                int x = (int)(X / 25.4 * dpi);
                int y = (int)(Y / 25.4 * dpi);
                int width = (int)(Width / 25.4 * dpi);
                int height = (int)(Height / 25.4 * dpi);
                return new Rectangle(x, y, width, height, MeasurementUnits.Pixels);
            }
            // convert pixels to mm
            if (units == MeasurementUnits.Millimeters)
            {
                int x = (int)(X / (double)dpi * 25.4);
                int y = (int)(Y / (double)dpi * 25.4);
                int width = (int)(Width / (double)dpi * 25.4);
                int height = (int)(Height / (double)dpi * 25.4);
                return new Rectangle(x, y, width, height, MeasurementUnits.Millimeters);

            }

            throw new NotImplementedException($"Rectangle conversion from {Units} to {units} is not implemented");
        }

        /// <summary>
        /// Gets or sets the size of this <see cref="Rectangle"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(Width, Height);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// Gets the x-coordinate of the right edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Right
        {
            get => X + Width;
        }

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Bottom
        {
            get => Y + Height;
        }

        /// <summary>
        /// Gets the x-coordinate of the left edge of this <see cref="Rectangle"/>.
        /// </summary>
        public int Left => X;

        /// <summary>
        /// Determines if the specified point is contained within the rectangular region defined by
        /// this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the given point.</param>
        /// <param name="y">The y-coordinate of the given point.</param>
        public bool Contains(int x, int y)
        {
            return X <= x && x < Right && Y <= y && y < Bottom;
        }

        /// <summary>
        /// Implicitly casts to RectangleF objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support RectangleF as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a RectangleF.</param>
        public static implicit operator RectangleF(Rectangle Rectangle)
        {
            return new RectangleF(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Rectangle objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="rectangle">System.Drawing.Rectangle will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(System.Drawing.Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.Rectangle objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a System.Drawing.Rectangle.</param>
        public static implicit operator System.Drawing.Rectangle(Rectangle Rectangle)
        {
            return new System.Drawing.Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKRect objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="sKRect">SkiaSharp.SKRect will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(SkiaSharp.SKRect sKRect)
        {
            SkiaSharp.SKRect standardizedSKRect = sKRect.Standardized;
            return CreateRectangle((int)standardizedSKRect.Left, (int)standardizedSKRect.Top, (int)standardizedSKRect.Right, (int)standardizedSKRect.Bottom);
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKRect objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a SkiaSharp.SKRect.</param>
        public static implicit operator SkiaSharp.SKRect(Rectangle Rectangle)
        {
            return SkiaSharp.SKRect.Create(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKRectI objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRectI as well.</para>
        /// </summary>
        /// <param name="sKRectI">SkiaSharp.SKRectI will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(SkiaSharp.SKRectI sKRectI)
        {
            SkiaSharp.SKRectI standardizedSKRectI = sKRectI.Standardized;
            return CreateRectangle(standardizedSKRectI.Left, standardizedSKRectI.Top, standardizedSKRectI.Right, standardizedSKRectI.Bottom);
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKRectI objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRectI as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a SkiaSharp.SKRectI.</param>
        public static implicit operator SkiaSharp.SKRectI(Rectangle Rectangle)
        {
            return SkiaSharp.SKRectI.Create(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Rectangle objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.Rectangle as well.</para>
        /// </summary>
        /// <param name="rectangle">SixLabors.ImageSharp.Rectangle will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(SixLabors.ImageSharp.Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.Rectangle objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.Rectangle as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a SixLabors.ImageSharp.Rectangle.</param>
        public static implicit operator SixLabors.ImageSharp.Rectangle(Rectangle Rectangle)
        {
            return new SixLabors.ImageSharp.Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.RectangleF objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.RectangleF as well.</para>
        /// </summary>
        /// <param name="rectangle">SixLabors.ImageSharp.RectangleF will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(SixLabors.ImageSharp.RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.RectangleF objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.RectangleF as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a SixLabors.ImageSharp.RectangleF.</param>
        public static implicit operator SixLabors.ImageSharp.RectangleF(Rectangle Rectangle)
        {
            return new SixLabors.ImageSharp.RectangleF(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Rect objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.Rect as well.</para>
        /// </summary>
        /// <param name="rectangle">Microsoft.Maui.Graphics.Rect will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(Microsoft.Maui.Graphics.Rect rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.Rect objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.Rect as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a Microsoft.Maui.Graphics.Rect.</param>
        public static implicit operator Microsoft.Maui.Graphics.Rect(Rectangle Rectangle)
        {
            return new Microsoft.Maui.Graphics.Rect(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.RectF objects to <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.RectF as well.</para>
        /// </summary>
        /// <param name="rectangle">Microsoft.Maui.Graphics.RectF will automatically be casted to <see cref="Rectangle"/>.</param>
        public static implicit operator Rectangle(Microsoft.Maui.Graphics.RectF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.RectF objects from <see cref="Rectangle"/>.
        /// <para>When your .NET Class methods use <see cref="Rectangle"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.RectF as well.</para>
        /// </summary>
        /// <param name="Rectangle"><see cref="Rectangle"/> is explicitly cast to a Microsoft.Maui.Graphics.RectF.</param>
        public static implicit operator Microsoft.Maui.Graphics.RectF(Rectangle Rectangle)
        {
            return new Microsoft.Maui.Graphics.RectF(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        #region Private Method

        private static Rectangle CreateRectangle(int left, int top, int right, int bottom)
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }

        #endregion
    }

    /// <summary>
    /// A defined unit of measurement
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
