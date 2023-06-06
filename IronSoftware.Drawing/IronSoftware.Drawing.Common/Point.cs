using System;

namespace IronSoftware.Drawing
{
    /// <summary>
    /// Represents an ordered pair of integer x- and y-coordinates that defines a point in a two-dimensional plane.
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

        /// <summary>
        /// Implicitly casts System.Drawing.Point objects to <see cref="Point"/>.  
        /// <para>When your .NET Class methods use <see cref="Point"/> as parameters or return types, you now automatically support Points as well.</para>
        /// </summary>
        /// <param name="point">System.Drawing.Point will automatically be cast to <see cref="Point"/> </param>
        public static implicit operator Point(System.Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }

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
    }
}
