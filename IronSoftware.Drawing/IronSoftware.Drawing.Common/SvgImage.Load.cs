using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using SixLabors.ImageSharp.Processing;

namespace IronSoftware.Drawing
{
    public static class SvgImageRenderer
    {

        public static Image<TPixel> LoadFromString<TPixel>(string content)
            where TPixel : unmanaged, IPixel<TPixel>
            => LoadFromStringInner<TPixel>(content, null, null);

        public static Image<TPixel> LoadFromString<TPixel>(string content, int width, int height)
            where TPixel : unmanaged, IPixel<TPixel>
            => LoadFromStringInner<TPixel>(content, width, height);

        private static Image<TPixel> LoadFromStringInner<TPixel>(string content, int? targetWidth, int? targetHeight)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var doc = SVGSharpie.SvgDocument.Parse(content);

            float? width = targetWidth ?? doc.RootElement.Width ?? doc.RootElement.ViewWidth;
            float? height = targetHeight ?? doc.RootElement.Height ?? doc.RootElement.ViewWidth;

            if (!width.HasValue || !height.HasValue)
            {
                throw new Exception("Svg does not specify a size set one.");
            }

            var image = new Image<TPixel>((int)Math.Ceiling(width.Value), (int)Math.Ceiling(height.Value));

            image.Mutate(x =>
            {
                var renderer = new SvgDocumentRenderer<TPixel>(image.Size(), x);
                doc.RootElement.Accept(renderer);
            });



            return image;
        }
    }
}
