using SkiaSharp;

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
        /// Implicitly casts System.Drawing.Rectangle objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to an System.Drawing.Rectangle.</param>
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
        /// Implicitly casts SkiaSharp.SKRect objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRect as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to an SkiaSharp.SKRect.</param>
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
        /// Implicitly casts SkiaSharp.SKRectI objects from <see cref="CropRectangle"/>.
        /// <para>When your .NET Class methods use <see cref="CropRectangle"/> as parameters and return types, you now automatically support SkiaSharp.SKRectI as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to a SkiaSharp.SKRectI.</param>
        static public implicit operator SkiaSharp.SKRectI(CropRectangle CropRectangle)
        {
            return SkiaSharp.SKRectI.Create(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }

        #region Private Method

        private static CropRectangle CreateCropRectangle(int left, int top, int right, int bottom)
        {
            return new CropRectangle(left, top, right - left, bottom - top);
        }

        #endregion
    }
}
