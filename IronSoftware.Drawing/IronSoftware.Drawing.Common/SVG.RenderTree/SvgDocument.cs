using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SVGSharpie;

namespace IronSoftware.Drawing
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private Matrix3x2 activeMatrix = Matrix3x2.Identity;
        private readonly Vector2 size;
        private readonly IImageProcessingContext image;

        public SvgDocumentRenderer(SizeF size, IImageProcessingContext image)
        {
            this.size = size;
            this.image = image;
        }

        public override void VisitSvgElement(SvgSvgElement element)
        {
            activeMatrix = element.CalculateViewboxFit(size.X, size.Y).AsMatrix3x2();

            base.VisitSvgElement(element);
        }

        private Matrix3x2 CalulateUpdatedMatrix(SvgGraphicsElement elm)
        {
            var matrix = activeMatrix;
            foreach (var t in elm.Transform)
            {
                matrix = matrix * t.Matrix.AsMatrix3x2();
            }
            return matrix;
        }

        public override void VisitGElement(SvgGElement element)
        {
            var oldMatrix = activeMatrix;
            activeMatrix = CalulateUpdatedMatrix(element);
            base.VisitGElement(element);

            activeMatrix = oldMatrix;
        }
    }
}
