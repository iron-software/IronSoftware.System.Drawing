using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SVGSharpie;
using System.Numerics;

namespace IronSoftware.Drawing
{
    internal static class Extensions
    {
        public static Matrix3x2 AsMatrix3x2(this SvgMatrix matrix)
        {
            return new Matrix3x2(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
        }

        public static TPixel As<TPixel>(this SvgColor value, float opactiy) where TPixel : unmanaged, IPixel<TPixel>
        {
            var colorRgb = new Rgba32(value.R, value.G, value.B, (byte)(opactiy * value.A));

            var color = default(TPixel);
            color.FromRgba32(colorRgb);

            return color;
        }

        public static HorizontalAlignment AsHorizontalAlignment(this CssTextAnchorType textalign)
        {
            switch (textalign)
            {
                case CssTextAnchorType.End: // shouldn't really be called should be transformed based on text direction???
                    return HorizontalAlignment.Right;
                case CssTextAnchorType.Middle:
                    return HorizontalAlignment.Center;
                case CssTextAnchorType.Start:
                default:
                    return HorizontalAlignment.Left;
            }
        }

        public static JointStyle AsJointStyle(this StyleProperty<SvgStrokeLineJoin> join) => join.Value.AsJointStyle();

        public static JointStyle AsJointStyle(this SvgStrokeLineJoin join)
        {
            switch (join)
            {
                case SvgStrokeLineJoin.Miter:
                    return JointStyle.Miter;
                case SvgStrokeLineJoin.Round:
                    return JointStyle.Round;
                case SvgStrokeLineJoin.Bevel:
                    return JointStyle.Square;
                case SvgStrokeLineJoin.Inherit:
                default:
                    return JointStyle.Miter;
            }
        }
        public static EndCapStyle AsEndCapStyle(this StyleProperty<SvgStrokeLineCap> join) => join.Value.AsEndCapStyle();

        public static EndCapStyle AsEndCapStyle(this SvgStrokeLineCap join)
        {
            switch (join)
            {
                case SvgStrokeLineCap.Butt:
                    return EndCapStyle.Butt;
                case SvgStrokeLineCap.Round:
                    return EndCapStyle.Round;
                case SvgStrokeLineCap.Square:
                    return EndCapStyle.Square;
                case SvgStrokeLineCap.Inherit:
                default:
                    return EndCapStyle.Butt;
            }
        }
    }
}