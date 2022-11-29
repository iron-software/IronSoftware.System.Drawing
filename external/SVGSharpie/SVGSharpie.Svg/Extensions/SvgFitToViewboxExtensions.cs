using System;

namespace SVGSharpie
{
    public static class SvgFitToViewboxExtensions
    {
        /// <summary>
        /// Calculates the matrix to transform the content by to fit into the specified viewport dimensions
        /// </summary>
        public static SvgMatrix CalculateViewboxFit(this ISvgFitToViewbox fitToViewbox, float viewportWidth, float viewportHeight)
        {
            CalculateViewboxFit(fitToViewbox, viewportWidth, viewportHeight, out var scaleX, out var scaleY, out var offsetX, out var offsetY);
            var translateMatrix = SvgMatrix.CreateTranslate(offsetX, offsetY);
            var scaleMatrix = SvgMatrix.CreateScale(scaleX, scaleY);
            return SvgMatrix.Multiply(translateMatrix, scaleMatrix);
        }

        /// <summary>
        /// Calculates the scale and offset values to transform the content by to fit into the specified viewport dimensions
        /// </summary>
        public static void CalculateViewboxFit(this ISvgFitToViewbox fitToViewbox, float viewportWidth, float viewportHeight, out float scaleX, out float scaleY, out float offsetX, out float offsetY)
        {
            if (fitToViewbox == null) throw new ArgumentNullException(nameof(fitToViewbox));
            var viewBox = fitToViewbox.ViewBox ?? new SvgRect(0, 0, viewportWidth, viewportHeight);
            
            var preserveAspectRatio = fitToViewbox.PreserveAspectRatio;
            var meetOrSliceFlag = preserveAspectRatio?.MeetOrSlice ?? SvgPreserveAspectRatioMeetOrSlice.Meet;
            var alignFlag = preserveAspectRatio?.Align ?? SvgPreserveAspectRatioAlign.XMidYMid;

            scaleX = viewportWidth / viewBox.Width;
            scaleY = viewportHeight / viewBox.Height;
            float uniformScale;

            switch (meetOrSliceFlag)
            {
                case SvgPreserveAspectRatioMeetOrSlice.Meet:
                    uniformScale = Math.Min(scaleX, scaleY);
                    break;
                case SvgPreserveAspectRatioMeetOrSlice.Slice:
                    uniformScale = Math.Max(scaleX, scaleY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (alignFlag != SvgPreserveAspectRatioAlign.None)
            {
                scaleX = scaleY = uniformScale;
            }
            
            var xMin = -viewBox.X;
            var yMin = -viewBox.Y;
            var xMid = xMin + (viewportWidth - viewBox.Width * scaleX) * 0.5f;
            var yMid = yMin + (viewportHeight - viewBox.Height * scaleY) * 0.5f;
            var xMax = xMin + (viewportWidth - viewBox.Width * scaleX);
            var yMax = yMin + (viewportHeight - viewBox.Height * scaleY);

            switch (alignFlag)
            {
                case SvgPreserveAspectRatioAlign.None:
                    offsetX = offsetY = 0;
                    break;
                case SvgPreserveAspectRatioAlign.XMinYMin:
                    offsetX = xMin;
                    offsetY = yMin;
                    break;
                case SvgPreserveAspectRatioAlign.XMidYMin:
                    offsetX = xMid;
                    offsetY = yMin;
                    break;
                case SvgPreserveAspectRatioAlign.XMaxYMin:
                    offsetX = xMax;
                    offsetY = yMin;
                    break;
                case SvgPreserveAspectRatioAlign.XMinYMid:
                    offsetX = xMin;
                    offsetY = yMid;
                    break;
                case SvgPreserveAspectRatioAlign.XMidYMid:
                    offsetX = xMid;
                    offsetY = yMid;
                    break;
                case SvgPreserveAspectRatioAlign.XMaxYMid:
                    offsetX = xMax;
                    offsetY = yMid;
                    break;
                case SvgPreserveAspectRatioAlign.XMinYMax:
                    offsetX = xMin;
                    offsetY = yMax;
                    break;
                case SvgPreserveAspectRatioAlign.XMidYMax:
                    offsetX = xMid;
                    offsetY = yMax;
                    break;
                case SvgPreserveAspectRatioAlign.XMaxYMax:
                    offsetX = xMax;
                    offsetY = yMax;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
