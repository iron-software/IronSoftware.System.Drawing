namespace SVGSharpie
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGPathSegArcAbs interface corresponds to an "absolute arcto" (A) path data command.
    /// </summary>
    public sealed class SvgPathSegArcAbs : SvgPathSeg
    {
        /// <summary>
        /// Gets the x-axis radius of the ellipse
        /// </summary>
        public float RadiusX { get; set; }

        /// <summary>
        /// Gets the y-axis radius of the ellipse
        /// </summary>
        public float RadiusY { get; set; }

        /// <summary>
        /// Gets the x-axis coordinate of the point to draw the curve to
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets the y-axis coordinate of the point to draw the curve to
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the rotation angle in degrees for the ellipse's x-axis relative to the x-axis of the user coordinate system.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether one of the two larger arc sweeps will be chosen.  Of the four candidate 
        /// arc sweeps, two will represent an arc sweep of greater than or equal to 180 degrees (the "large-arc"), 
        /// and two will represent an arc sweep of less than or equal to 180 degrees (the "small-arc").
        /// </summary>
        public bool LargeArcFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the arc will be drawn in a "positive-angle" direction
        /// </summary>
        public bool SweepFlag { get; set; }

        public SvgPathSegArcAbs(float rx, float ry, float angle, bool largeArcFlag, bool sweepFlag, float x, float y)
        {
            RadiusX = rx;
            RadiusY = ry;
            Angle = angle;
            LargeArcFlag = largeArcFlag;
            SweepFlag = sweepFlag;
            X = x;
            Y = y;
        }

        /// <inheritdoc cref="SvgPathSeg.PathSegType"/>
        public override SvgPathSegType PathSegType => SvgPathSegType.ArcAbs;

        /// <inheritdoc cref="SvgPathSeg.PathSegTypeAsLetter"/>
        public override string PathSegTypeAsLetter => "A";

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override void Accept(SvgPathSegVisitor visitor) => visitor.VisitArcAbs(this);

        /// <inheritdoc cref="SvgPathSeg.Accept"/>
        public override TResult Accept<TResult>(SvgPathSegVisitor<TResult> visitor) => visitor.VisitArcAbs(this);

        /// <inheritdoc cref="SvgPathSeg.DeepClone"/>
        public override SvgPathSeg DeepClone()
            => new SvgPathSegArcAbs(RadiusX, RadiusY, Angle, LargeArcFlag, SweepFlag, X, Y);

        public override string ToString()
            => $"{PathSegTypeAsLetter}{RadiusX} {RadiusY} {Angle} {(LargeArcFlag ? "1" : "0")} {(SweepFlag ? "1" : "0")} {X} {Y}";
    }
}