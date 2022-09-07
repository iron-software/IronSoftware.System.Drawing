namespace IronSoftware.Drawing
{
    public partial class CropRectangle
    {
        public CropRectangle() { }

        public CropRectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
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
}
