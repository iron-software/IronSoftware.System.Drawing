using SVGSharpie.Utils;

namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGGeometryElement interface represents SVG elements whose rendering is defined by geometry with an equivalent path, 
    /// and which can be filled and stroked. This includes paths and the basic shapes.
    /// </summary>
    public abstract class SvgGeometryElement : SvgGraphicsElement
    {
        internal SvgGeometryElement()
        {
        }

        /// <summary>
        /// Converts the element to an equivalent path segment list representation
        /// </summary>
        /// <returns>path segment list representing this element</returns>
        public SvgPathSegList ConvertToPathSegList()
            => Accept(new SvgShapeElementToPathSegListConverter());
    }
}