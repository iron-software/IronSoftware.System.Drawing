using System.Collections.Generic;

namespace SVGSharpie
{
    /// <summary>
    /// Represents a paint server which radially interpolates a color through two or more color stops
    /// </summary>
    public sealed class SvgRadialGradientPaintServer : SvgGradientPaintServer
    {
        /// <summary>
        /// Gets or sets the the x- axis coordinate defining the largest (i.e., outermost) circle for the radial gradient
        /// </summary>
        public float CircleX { get; }

        /// <summary>
        /// Gets or sets the the y- axis coordinate defining the largest (i.e., outermost) circle for the radial gradient
        /// </summary>
        public float CircleY { get; }

        /// <summary>
        /// Gets or sets the the radius defining the largest (i.e., outermost) circle for the radial gradient
        /// </summary>
        /// <remarks>
        /// If the attribute is not specified, the effect is as if a value of '50%' were specified.
        /// </remarks>
        public float CircleRadius { get; }

        /// <summary>
        /// Gets or sets the the x- axis focal point of the gradient.  The gradient will be drawn such that the 0% 
        /// gradient stop is mapped to (FocalX, FocalY). 
        /// </summary>
        public float FocalX { get; }

        /// <summary>
        /// Gets or sets the the y- axis focal point of the gradient.  The gradient will be drawn such that the 0% 
        /// gradient stop is mapped to (FocalX, FocalY). 
        /// </summary>
        public float FocalY { get; }

        /// <summary>
        /// Gets the coordinate system units for the current paint server
        /// </summary>
        public SvgUnitTypes Units { get; }

        internal SvgRadialGradientPaintServer(float circleX, float circleY, float circleRadius, float focalX, float focalY, SvgUnitTypes units, IReadOnlyList<SvgGradientPaintServerColorStop> stops)
            : base(stops)
        {
            CircleX = circleX;
            CircleY = circleY;
            CircleRadius = circleRadius;
            FocalX = focalX;
            FocalY = focalY;
            Units = units;
        }

        /// <inheritdoc cref="SvgPaintServer.Accept"/>
        public override void Accept(SvgPaintServerVisitor visitor)
            => visitor.VisitRadialGradientPaintServer(this);

        /// <inheritdoc cref="SvgPaintServer.Accept"/>
        public override TResult Accept<TResult>(SvgPaintServerVisitor<TResult> visitor)
            => visitor.VisitRadialGradientPaintServer(this);
    }
}