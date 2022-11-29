namespace SVGSharpie
{
    public enum SvgTransformType
    {
        /// <summary>
        /// A matrix(…) transformation, SVG_TRANSFORM_MATRIX
        /// </summary>
        Matrix = 1,
        /// <summary>
        /// A translate(…) transformation, SVG_TRANSFORM_TRANSLATE
        /// </summary>
        Translate = 2,
        /// <summary>
        /// A scale(…) transformation, SVG_TRANSFORM_SCALE
        /// </summary>
        Scale = 3,
        /// <summary>
        /// A rotate(…) transformation, SVG_TRANSFORM_ROTATE
        /// </summary>
        Rotate = 4,
        /// <summary>
        /// A skewx(…) transformation, SVG_TRANSFORM_SKEWX
        /// </summary>
        Skewx = 5,
        /// <summary>
        /// A skewy(…) transformation, SVG_TRANSFORM_SKEWY
        /// </summary>
        Skewy = 6,
    }
}