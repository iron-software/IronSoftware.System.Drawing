using System;
using System.Diagnostics;

namespace SVGSharpie
{
    /// <summary>
    /// Represents rectangular geometry. Rectangles are defined as consisting of a (x,y) coordinate pair identifying a minimum X value, 
    /// a minimum Y value, and a width and height, which are usually constrained to be non-negative.
    /// </summary>
    public struct SvgRect : IEquatable<SvgRect>
    {
        /// <summary>
        /// Gets or sets the x coordinate of the rectangle, in user units.
        /// </summary>
        public float X { get; }
        /// <summary>
        /// Gets or sets the y coordinate of the rectangle, in user units.
        /// </summary>
        public float Y { get; }
        /// <summary>
        /// Gets or sets the width coordinate of the rectangle, in user units.
        /// </summary>
        public float Width { get; }
        /// <summary>
        /// Gets or sets the height coordinate of the rectangle, in user units.
        /// </summary>
        public float Height { get; }

        public float Right => X + Width;

        public float Bottom => Y + Height;

        [DebuggerStepThrough]
        public SvgRect(float width, float height)
        {
            X = Y = 0;
            Width = width;
            Height = height;
        }

        [DebuggerStepThrough]
        public SvgRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override string ToString()
            => $"{X} {Y} {Width} {Height}";

        public SvgRect Merge(SvgRect other)
        {
            var minX = Math.Min(X, other.X);
            var minY = Math.Min(Y, other.Y);
            var maxX = Math.Max(Right, other.Right);
            var maxY = Math.Max(Bottom, other.Bottom);
            return new SvgRect(minX, minY, maxX - minX, maxY - minY);
        }

        public bool Equals(SvgRect other) 
            => X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SvgRect rect && Equals(rect);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SvgRect left, SvgRect right) => left.Equals(right);

        public static bool operator !=(SvgRect left, SvgRect right) => !left.Equals(right);

        /// <summary>
        /// Parses a rectangle from the specified string (formatted as "x y width height").
        /// </summary>
        /// <param name="str">string containing the rectangle coordinates seperated by spaces</param>
        /// <returns>rectangle parsed from the string</returns>
        public static SvgRect Parse(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            var values = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 4)
            {
                throw new Exception($"Expected 4 values but got {values.Length} for '{str}'");
            }
            return new SvgRect
            (
                float.Parse(values[0]),
                float.Parse(values[1]),
                float.Parse(values[2]),
                float.Parse(values[3])
            );
        }
    }
}