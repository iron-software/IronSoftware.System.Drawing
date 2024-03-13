using System;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// Represents an ordered pair of double x- and y-coordinates that defines a point in a two-dimensional plane.
    /// </summary>
    public partial class Point
    {
        /// <summary>
        /// Gets or sets the x-coordinate of this <see cref="IronSoftware.Drawing.Point"/>.
        /// </summary>
        public int X { get; set; } = 0;

        /// <summary>
        /// Gets or sets the y-coordinate of this <see cref="IronSoftware.Drawing.Point"/>.
        /// </summary>
        public int Y { get; set; } = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="IronSoftware.Drawing.Point"/> struct with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Translates this <see cref="IronSoftware.Drawing.Point"/> by the specified amount.
        /// </summary>
        /// <param name="dx">The amount to offset the x-coordinate.</param>
        /// <param name="dy">The amount to offset the y-coordinate.</param>
        public void Offset(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        #region Implicit Operators

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.Point objects to Point
        /// </summary>
        /// <param name="point">System.Drawing.Point will automatically be casted to <see cref="Point"/> </param>
        public static implicit operator Point(SixLabors.ImageSharp.Point point)
        {
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts Point objects to SixLabors.ImageSharp.Point
        /// </summary>
        /// <param name="point">Point will automatically be casted to SixLabors.ImageSharp.Point</param>
        public static implicit operator SixLabors.ImageSharp.Point(Point point)
        {
            return new SixLabors.ImageSharp.Point(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts System.Drawing.Point objects to <see cref="Point"/>.
        /// </summary>
        /// <param name="point">System.Drawing.Point will automatically be casted to <see cref="Point"/> </param>
        public static implicit operator Point(System.Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts Point objects to System.Drawing.Point
        /// </summary>
        /// <param name="point">Point will automatically be casted to System.Drawing.Point</param>
        public static implicit operator System.Drawing.Point(Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Point objects to <see cref="Point"/>.
        /// </summary>
        /// <param name="point">Microsoft.Maui.Graphics.Point will automatically be casted to <see cref="Point"/> </param>
        public static implicit operator Point(Microsoft.Maui.Graphics.Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Implicitly casts Point objects to Microsoft.Maui.Graphics.Point
        /// </summary>
        /// <param name="point">Point will automatically be casted to Microsoft.Maui.Graphics.Point</param>
        public static implicit operator Microsoft.Maui.Graphics.Point(Point point)
        {
            return new Microsoft.Maui.Graphics.Point(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKPointI objects to <see cref="Point"/>.
        /// </summary>
        /// <param name="point">SkiaSharp.SKPointI will automatically be casted to <see cref="Point"/> </param>
        public static implicit operator Point(SkiaSharp.SKPointI point)
        {
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts Point objects to SkiaSharp.SKPointI
        /// </summary>
        /// <param name="point">Point will automatically be casted to SkiaSharp.SKPointI</param>
        public static implicit operator SkiaSharp.SKPointI(Point point)
        {
            return new SkiaSharp.SKPointI(point.X, point.Y);
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Specifies whether this <see cref="IronSoftware.Drawing.Point"/> instance contains the same coordinates as another <see cref="IronSoftware.Drawing.Point"/>.
        /// </summary>
        /// <param name="obj">The point to test for equality.</param>
        /// <returns>true if other has the same coordinates as this point instance.</returns>
        public override bool Equals(object obj)
        {
            Point otherPoint;
            try
            {
                otherPoint = obj as Point;
            }
            catch
            {
                return false;
            }
            return (this.X == otherPoint.X && this.Y == otherPoint.Y);
        }

        /// <summary>
        /// Hashing integer based on image raw binary data.
        /// </summary>
        /// <returns>Int</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
