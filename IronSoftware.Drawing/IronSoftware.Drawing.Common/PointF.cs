using System;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// Represents an ordered pair of integer x- and y-coordinates that defines a point in a two-dimensional plane.
    /// </summary>
    public partial class PointF
    {
        /// <summary>
        /// Gets or sets the x-coordinate of this <see cref="IronSoftware.Drawing.PointF"/>.
        /// </summary>
        public float X { get; set; } = 0;

        /// <summary>
        /// Gets or sets the y-coordinate of this <see cref="IronSoftware.Drawing.PointF"/>.
        /// </summary>
        public float Y { get; set; } = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="IronSoftware.Drawing.PointF"/> struct with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Translates this <see cref="IronSoftware.Drawing.PointF"/> by the specified amount.
        /// </summary>
        /// <param name="dx">The amount to offset the x-coordinate.</param>
        /// <param name="dy">The amount to offset the y-coordinate.</param>
        public void Offset(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        #region Implicit Operators

        /// <summary>
        /// Implicitly casts SixLabors.ImageSharp.PointF objects to <see cref="Point"/>.  
        /// <para>When your .NET Class methods use <see cref="Point"/> as parameters or return types, you now automatically support Points as well.</para>
        /// </summary>
        /// <param name="point">System.Drawing.PointF will automatically be cast to <see cref="Point"/> </param>
        public static implicit operator PointF(SixLabors.ImageSharp.PointF point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Point objects to <see cref="Point"/>.  
        /// <para>When your .NET Class methods use <see cref="Point"/> as parameters or return types, you now automatically support Points as well.</para>
        /// </summary>
        /// <param name="point">Microsoft.Maui.Graphics.Point will automatically be cast to <see cref="Point"/> </param>
        public static implicit operator PointF(Microsoft.Maui.Graphics.PointF point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKPoint objects to <see cref="Point"/>.  
        /// <para>When your .NET Class methods use <see cref="Point"/> as parameters or return types, you now automatically support Points as well.</para>
        /// </summary>
        /// <param name="point">SkiaSharp.SKPoint will automatically be cast to <see cref="Point"/> </param>
        public static implicit operator PointF(SkiaSharp.SKPoint point)
        {
            return new PointF(point.X, point.Y);
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
