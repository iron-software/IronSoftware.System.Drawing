using System;

namespace SVGSharpie
{
    /// <summary>
    /// Represents an <see cref="SvgLength"/> in the context of an <see cref="SvgElement"/>
    /// </summary>
    internal sealed class SvgLengthElementContext : SvgLengthContext
    {
        private readonly SvgElement _element;

        private readonly SvgLengthDirection _dir;
        
        public SvgLengthElementContext(SvgElement element, SvgLengthDirection dir)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _dir = dir;
        }

        public override float ComputeTotalLength()
        {
            var parent = GetParentSvgElementOrThrow();

            float? parentLength;
            switch (_dir)
            {
                case SvgLengthDirection.Horizontal:
                    parentLength = parent.ViewWidth;
                    break;
                case SvgLengthDirection.Vertical:
                    parentLength = parent.ViewHeight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (parentLength == null)
            {
                throw new Exception("Parent svg does not define a size");
            }

            return parentLength.Value;
        }
        
        private SvgSvgElement GetParentSvgElementOrThrow()
        {
            var parent = _element.ParentSvg;
            if (parent == null)
            {
                throw new Exception($"Element (id={_element.Id}, type={_element.GetType().Name}) has no parent Svg element");
            }

            return parent;
        }
    }
}