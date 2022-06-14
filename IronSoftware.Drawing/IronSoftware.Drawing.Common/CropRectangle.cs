namespace IronSoftware.Drawing
{
    public partial class CropRectangle
    {
        public CropRectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// The x-coordinate of the upper-left corner of this Rectangle structure. The default is 0.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The y-coordinate of the upper-left corner of this Rectangle structure. The default is 0.
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The width of this Rectangle structure. The default is 0.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// The height of this Rectangle structure. The default is 0.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Implicitly casts System.Drawing.Rectangle objects to <see cref="CropRectangle"/>.  
        /// <para>When your .NET Class methods to use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="Rectangle">SkiaSharp.SKBitmap  will automatically be cast to <see cref="CropRectangle"/> </param>

        public static implicit operator CropRectangle(System.Drawing.Rectangle Rectangle)
        {
            return new CropRectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Rectangle objects from <see cref="CropRectangle"/>.  
        /// <para>When your .NET Class methods to use <see cref="CropRectangle"/> as parameters and return types, you now automatically support Rectangle as well.</para>
        /// </summary>
        /// <param name="CropRectangle"><see cref="CropRectangle"/> is explicitly cast to an System.Drawing.Rectangle </param>
        static public implicit operator System.Drawing.Rectangle(CropRectangle CropRectangle)
        {
            return new System.Drawing.Rectangle(CropRectangle.X, CropRectangle.Y, CropRectangle.Width, CropRectangle.Height);
        }
    }
}
