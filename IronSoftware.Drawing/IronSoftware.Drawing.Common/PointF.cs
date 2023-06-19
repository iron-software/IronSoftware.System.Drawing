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
        /// Implicitly casts SixLabors.ImageSharp.PointF objects to PointF
        /// </summary>
        /// <param name="point">SixLabors.ImageSharp.PointF will automatically be cast to PointF</param>
        public static implicit operator PointF(SixLabors.ImageSharp.PointF point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts a PointF object to SixLabors.ImageSharp.PointF
        /// </summary>
        /// <param name="point">PointF will automatically be cast to SixLabors.ImageSharp.PointF</param>
        public static implicit operator SixLabors.ImageSharp.PointF(PointF point)
        {
            return new SixLabors.ImageSharp.PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts Microsoft.Maui.Graphics.Point objects to PointF
        /// </summary>
        /// <param name="point">Microsoft.Maui.Graphics.Point will automatically be cast to PointF</param>
        public static implicit operator PointF(Microsoft.Maui.Graphics.PointF point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts PointF objects to Microsoft.Maui.Graphics.Point
        /// </summary>
        /// <param name="point">PointF will automatically be cast to Microsoft.Maui.Graphics.Point</param>
        public static implicit operator Microsoft.Maui.Graphics.PointF(PointF point)
        {
            return new Microsoft.Maui.Graphics.PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts SkiaSharp.SKPoint objects to PointF
        /// </summary>
        /// <param name="point">SkiaSharp.SKPoint will automatically be cast to PointF</param>
        public static implicit operator PointF(SkiaSharp.SKPoint point)
        {
            return new PointF(point.X, point.Y);
        }

        /// <summary>
        /// Implicitly casts PointF objects to SkiaSharp.SKPoint
        /// </summary>
        /// <param name="point">PointF will automatically be cast to SkiaSharp.SKPoint</param>
        public static implicit operator SkiaSharp.SKPoint(PointF point)
        {
            return new SkiaSharp.SKPoint(point.X, point.Y);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Specifies whether this <see cref="IronSoftware.Drawing.PointF"/> instance contains the same coordinates as another <see cref="IronSoftware.Drawing.PointF"/>.
        /// </summary>
        /// <param name="obj">The point to test for equality.</param>
        /// <returns>true if other has the same coordinates as this point instance.</returns>
        public override bool Equals(object obj)
        {
            PointF otherPoint;
            try
            {
                otherPoint = obj as PointF;
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
