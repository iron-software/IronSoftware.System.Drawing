using System;

namespace SVGSharpie
{
    /// <summary>
    /// Defines a 3×3 arithmetic matrix
    /// 
    /// [a c e]
    /// [b d f]
    /// [0 0 1]
    /// 
    /// </summary>
    /// <remarks>
    /// Many SVG graphics operations use 2×3 matrices. When you need a matrix for matrix arithmetic, you can expand a 
    /// 2×3 matrix into a 3×3 matrix equivalent by adding a third row of [0 0 1].
    /// </remarks>
    public sealed class SvgMatrix
    {
        /// <summary>
        /// Gets or sets the a entry of the matrix
        /// </summary>
        public float A { get; set; }
        /// <summary>
        /// Gets or sets the b entry of the matrix
        /// </summary>
        public float B { get; set; }
        /// <summary>
        /// Gets or sets the c entry of the matrix
        /// </summary>
        public float C { get; set; }
        /// <summary>
        /// Gets or sets the d entry of the matrix
        /// </summary>
        public float D { get; set; }
        /// <summary>
        /// Gets or sets the e entry of the matrix
        /// </summary>
        public float E { get; set; }
        /// <summary>
        /// Gets or sets the f entry of the matrix
        /// </summary>
        public float F { get; set; }

        public static SvgMatrix Identity => new SvgMatrix(1, 0, 0, 1, 0, 0);

        public SvgMatrix(float[] values)
        {
            if (values.Length != 6)
            {
                throw new ArgumentException("Expected array of 6 values", nameof(values));
            }
            A = values[0];
            B = values[1];
            C = values[2];
            D = values[3];
            E = values[4];
            F = values[5];
        }

        public SvgMatrix(float a, float b, float c, float d, float e, float f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }

        public override string ToString() => $"{A} {B} {C} {D} {E} {F}";

        public static SvgMatrix CreateTranslate(float x, float y) => new SvgMatrix(1, 0, 0, 1, x, y);

        public static SvgMatrix CreateScale(float x, float y) => new SvgMatrix(x, 0, 0, y, 0, 0);

        public static SvgMatrix operator *(SvgMatrix a, SvgMatrix b) => Multiply(a, b);

        /// <summary>
        /// Creates a rotation matrix about a given point.
        ///
        /// If optional parameters <see cref="x"/> and <see cref="y"/> are not supplied the operation corresponds to 
        /// the matrix [cos(a) sin(a) -sin(a) cos(a) 0 0].
        ///
        /// If optional parameters <see cref="x"/> and <see cref="y"/>, the rotate is about the point(x, y) and the operation 
        /// represents the equivalent of the following specification: 
        ///  translate(x, y) rotate(angle) translate(-x, -y).
        /// </summary>
        public static SvgMatrix CreateRotate(float angleInDegrees, float? x = null, float? y = null)
        {
            if (x.HasValue != y.HasValue)
            {
                throw new ArgumentException();
            }
            var radians = angleInDegrees * Math.PI / 180f;
            var cosA = (float)Math.Cos(radians);
            var sinA = (float)Math.Sin(radians);
            var rotMatrix = new SvgMatrix(cosA, sinA, -sinA, cosA, 0, 0);
            if (x != null)
            {
                var translate1 = CreateTranslate(x.Value, y.Value);
                var translate2 = CreateTranslate(-x.Value, -y.Value);
                return Multiply(translate1, Multiply(rotMatrix, translate2));
            }
            return rotMatrix;
        }

        public static SvgMatrix Multiply(SvgMatrix mat1, SvgMatrix mat2)
        {
            float
                a1 = mat1.A,
                b1 = mat1.B,
                c1 = mat1.C,
                d1 = mat1.D,
                e1 = mat1.E,
                f1 = mat1.F,
                a2 = mat2.A,
                b2 = mat2.C,
                c2 = mat2.B,
                d2 = mat2.D,
                e2 = mat2.E,
                f2 = mat2.F;

            var a = a2 * a1 + c2 * c1;
            var c = b2 * a1 + d2 * c1;
            var b = a2 * b1 + c2 * d1;
            var d = b2 * b1 + d2 * d1;
            var e = e1 + e2 * a1 + f2 * c1;
            var f = f1 + e2 * b1 + f2 * d1;

            return new SvgMatrix(a, b, c, d, e, f);
        }

        public static void Multiply(SvgMatrix mat, float x, float y, out float resultX, out float resultY)
        {
            resultX = mat.A * x + mat.C * y + mat.E;
            resultY = mat.B * x + mat.D * y + mat.F;
        }
    }
}