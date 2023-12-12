using System;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// A universally compatible RectangleF for .NET 7, .NET 6, .NET 5, and .NET Core. As well as compatibility with Windows, NanoServer, IIS, macOS, Mobile, Xamarin, iOS, Android, Google Compute, Azure, AWS, and Linux.
    /// <para>Works nicely with popular Image RectangleF such as System.Drawing.RectangleF, SkiaSharp.SKRect, SixLabors.ImageSharp.RectangleF, Microsoft.Maui.Graphics.Rect.</para>
    /// <para>Implicit casting means that using this class to input and output RectangleF from public APIs gives full compatibility to all RectangleF type fully supported by Microsoft.</para>
    /// </summary>
    public class RectangleF
    {
        /// <summary>
        /// Construct a new RectangleF.
        /// </summary>
        /// <seealso cref="RectangleF"/>
        public RectangleF() { }

        /// <summary>
        /// Construct a new RectangleF.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of this RectangleF</param>
        /// <param name="y">The y-coordinate of the upper-left corner of this RectangleF</param>
        /// <param name="width">The width of this RectangleF</param>
        /// <param name="height">The height of this RectangleF</param>
        /// <param name="units">The measurement unit of this RectangleF</param>
        /// <seealso cref="RectangleF"/>
        public RectangleF(float x, float y, float width, float height, MeasurementUnits units = MeasurementUnits.Pixels)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Units = units;
        }

        /// <summary>
        /// The x-coordinate of the upper-left corner of this RectangleF. The default is 0
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// The y-coordinate of the upper-left corner of this RectangleF. The default is 0
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// The width of this RectangleF. The default is 0
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// The height of this RectangleF. The default is 0
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// The measurement unit of this RectangleF. The default is Pixels
        /// </summary>
        public MeasurementUnits Units
        {
            get;
            set;
        } = MeasurementUnits.Pixels;

        /// <summary>
        /// Convert this RectangleF to the specified units of measurement using the specified DPI
        /// </summary>
        /// <param name="units">Unit of measurement</param>
        /// <param name="dpi">DPI (Dots per inch) for conversion</param>
        /// <returns>A new RectangleF which uses the desired units of measurement</returns>
        /// <exception cref="NotImplementedException">Conversion not implemented</exception>
        public RectangleF ConvertTo(MeasurementUnits units, int dpi = 96)
        {
            // no conversion
            if (units == Units)
            {
                return this;
            }
            // convert mm to pixels
            if (units == MeasurementUnits.Pixels)
            {
                float x = X / 25.4f * dpi;
                float y = Y / 25.4f * dpi;
                float width = Width / 25.4f * dpi;
                float height = Height / 25.4f * dpi;
                return new RectangleF(x, y, width, height, MeasurementUnits.Pixels);
            }
            // convert pixels to mm
            if (units == MeasurementUnits.Millimeters)
            {
                float x = X / dpi * 25.4f;
                float y = Y / dpi * 25.4f;
                float width = Width / dpi * 25.4f;
                float height = Height / dpi * 25.4f;
                return new RectangleF(x, y, width, height, MeasurementUnits.Millimeters);

            }

            throw new NotImplementedException($"RectangleF conversion from {Units} to {units} is not implemented");
        }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// Gets the x-coordinate of the right edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Right
        {
            get => X + Width;
        }

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Bottom
        {
            get => Y + Height;
        }

        /// <summary>
        /// Gets the x-coordinate of the left edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Left => X;

        /// <summary>
        /// Determines if the specified point is contained within the rectangular region defined by
        /// this <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the given point.</param>
        /// <param name="y">The y-coordinate of the given point.</param>
        public bool Contains(int x, int y)
        {
            return X <= x && x < Right && Y <= y && y < Bottom;
        }

        /// <summary>
        /// Implicitly casts System.Drawing.RectangleF objects to <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support RectangleF as well.</para>
        /// </summary>
        /// <param name="RectangleF">System.Drawing.RectangleF will automatically be cast to <see cref="RectangleF"/>.</param>
        public static implicit operator RectangleF(System.Drawing.RectangleF RectangleF)
        {
            return new RectangleF(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts to System.Drawing.RectangleF objects from <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support RectangleF as well.</para>
        /// </summary>
        /// <param name="RectangleF"><see cref="RectangleF"/> is explicitly cast to a System.Drawing.RectangleF.</param>
        public static implicit operator System.Drawing.RectangleF(RectangleF RectangleF)
        {
            return new System.Drawing.RectangleF(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKRect objects to <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="sKRect">SkiaSharp.SKRect will automatically be cast to <see cref="RectangleF"/>.</param>
        public static implicit operator RectangleF(SkiaSharp.SKRect sKRect)
        {
            SkiaSharp.SKRect standardizedSKRect = sKRect.Standardized;
            return CreateRectangleF(standardizedSKRect.Left, standardizedSKRect.Top, standardizedSKRect.Right, standardizedSKRect.Bottom);
        }

        /// <summary>
        /// Implicitly casts to SkiaSharp.SKRect objects from <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="RectangleF"><see cref="RectangleF"/> is explicitly cast to a SkiaSharp.SKRect.</param>
        public static implicit operator SkiaSharp.SKRect(RectangleF RectangleF)
        {
            return SkiaSharp.SKRect.Create(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.RectangleF objects to <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.RectangleF as well.</para>
        /// </summary>
        /// <param name="RectangleF">SixLabors.ImageSharp.RectangleF will automatically be cast to <see cref="RectangleF"/>.</param>
        public static implicit operator RectangleF(SixLabors.ImageSharp.RectangleF RectangleF)
        {
            return new RectangleF(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts to SixLabors.ImageSharp.RectangleF objects from <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support SixLabors.ImageSharp.RectangleF as well.</para>
        /// </summary>
        /// <param name="RectangleF"><see cref="RectangleF"/> is explicitly cast to a SixLabors.ImageSharp.RectangleF.</param>
        public static implicit operator SixLabors.ImageSharp.RectangleF(RectangleF RectangleF)
        {
            return new SixLabors.ImageSharp.RectangleF(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Rect objects to <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.Rect as well.</para>
        /// </summary>
        /// <param name="RectangleF">Microsoft.Maui.Graphics.Rect will automatically be cast to <see cref="RectangleF"/>.</param>
        public static implicit operator RectangleF(Microsoft.Maui.Graphics.Rect RectangleF)
        {
            return new RectangleF((float)RectangleF.X, (float)RectangleF.Y, (float)RectangleF.Width, (float)RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.Rect objects from <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.Rect as well.</para>
        /// </summary>
        /// <param name="RectangleF"><see cref="RectangleF"/> is explicitly cast to a Microsoft.Maui.Graphics.Rect.</param>
        public static implicit operator Microsoft.Maui.Graphics.Rect(RectangleF RectangleF)
        {
            return new Microsoft.Maui.Graphics.Rect(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.RectF objects to <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.RectF as well.</para>
        /// </summary>
        /// <param name="RectangleF">Microsoft.Maui.Graphics.RectF will automatically be cast to <see cref="RectangleF"/>.</param>
        public static implicit operator RectangleF(Microsoft.Maui.Graphics.RectF RectangleF)
        {
            return new RectangleF(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        /// <summary>
        /// Implicitly casts to Microsoft.Maui.Graphics.RectF objects from <see cref="RectangleF"/>.
        /// <para>When your .NET Class methods use <see cref="RectangleF"/> as parameters and return types, you now automatically support Microsoft.Maui.Graphics.RectF as well.</para>
        /// </summary>
        /// <param name="RectangleF"><see cref="RectangleF"/> is explicitly cast to a Microsoft.Maui.Graphics.RectF.</param>
        public static implicit operator Microsoft.Maui.Graphics.RectF(RectangleF RectangleF)
        {
            return new Microsoft.Maui.Graphics.RectF(RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
        }

        #region Private Method

        private static RectangleF CreateRectangleF(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        #endregion
    }
}
