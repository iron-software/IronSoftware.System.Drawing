using System;

namespace SVGSharpie
{
    /// <summary>
    /// SVGTransform is the interface for one of the component transformations within an <see cref="SvgTransformList">SVGTransformList</see>; 
    /// thus, an <see cref="SvgTransform">SVGTransform</see> object corresponds to a single component (e.g., scale(…) or matrix(…)) within 
    /// a transform attribute.
    /// </summary>
    public sealed class SvgTransform
    {
        /// <summary>
        /// Gets the type of the transform
        /// </summary>
        public SvgTransformType TransformType { get; }

        /// <summary>
        /// A convenience attribute for SVG_TRANSFORM_ROTATE, SVG_TRANSFORM_SKEWX and SVG_TRANSFORM_SKEWY. It holds the angle that was specified.
        /// For SVG_TRANSFORM_MATRIX, SVG_TRANSFORM_TRANSLATE and SVG_TRANSFORM_SCALE, angle will be zero.
        /// </summary>
        public float Angle { get; }

        /// <summary>
        /// Gets the matrix that represents this transformation. The matrix object is live, meaning that any changes made to the SVGTransform 
        /// object are immediately reflected in the matrix object and vice versa. In case the matrix object is changed directly (i.e., without 
        /// using the methods on the SVGTransform interface itself) then the type of the SVGTransform changes to SVG_TRANSFORM_MATRIX.
        /// 
        /// For SVG_TRANSFORM_MATRIX, the matrix contains the a, b, c, d, e, f values supplied by the user.
        ///     For SVG_TRANSFORM_TRANSLATE, e and f represent the translation amounts(a= 1, b= 0, c= 0 and d = 1).
        /// For SVG_TRANSFORM_SCALE, a and d represent the scale amounts(b= 0, c= 0, e= 0 and f = 0).
        /// For SVG_TRANSFORM_SKEWX and SVG_TRANSFORM_SKEWY, a, b, c and d represent the matrix which will result in the given skew(e= 0 and f = 0).
        /// For SVG_TRANSFORM_ROTATE, a, b, c, d, e and f together represent the matrix which will result in the given rotation.When the rotation is around the center point(0, 0), e and f will be zero.
        /// </summary>
        public SvgMatrix Matrix { get; }

        internal SvgTransform(SvgTransformType transformType, SvgMatrix matrix, float angle = 0)
        {
            Matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
            TransformType = transformType;
            Angle = angle;
        }

        internal SvgTransform DeepClone() => new SvgTransform(TransformType, Matrix, Angle);

        public override string ToString()
        {
            switch (TransformType)
            {
                case SvgTransformType.Matrix:
                    return $"matrix({Matrix.A},{Matrix.B},{Matrix.C},{Matrix.D},{Matrix.E},{Matrix.F})";
                case SvgTransformType.Translate:
                    return $"translate({Matrix.E},{Matrix.F})";
                case SvgTransformType.Scale:
                    return $"scale({Matrix.A},{Matrix.D})";
                case SvgTransformType.Rotate:
                case SvgTransformType.Skewx:
                case SvgTransformType.Skewy:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}