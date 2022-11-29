namespace SVGSharpie
{
    /// <summary>
    /// Indicates whether to force uniform scaling and, if so, the alignment method to use in case the aspect ratio
    /// </summary>
    public enum SvgPreserveAspectRatioAlign
    {
        /// <summary>
        /// Do not force uniform scaling. Scale the graphic content of the given element non-uniformly if necessary 
        /// such that the element's bounding box exactly matches the viewport rectangle.
        /// </summary>
        None,
        /// <summary>
        /// Force uniform scaling.
        /// Align the min-x of the element's ‘viewBox’ with the smallest X value of the viewport.
        /// Align the min-y of the element's ‘viewBox’ with the smallest Y value of the viewport.
        /// </summary>
        XMinYMin,
        /// <summary>
        /// Force uniform scaling.
        /// Align the midpoint X value of the element's ‘viewBox’ with the midpoint X value of the viewport.
        /// Align the min-y of the element's ‘viewBox’ with the smallest Y value of the viewport.
        /// </summary>
        XMidYMin,
        /// <summary>
        /// Force uniform scaling.
        /// Align the min-x + width of the element's ‘viewBox’ with the maximum X value of the viewport.
        /// Align the min-y of the element's ‘viewBox’ with the smallest Y value of the viewport.
        /// </summary>
        XMaxYMin,
        /// <summary>
        /// Force uniform scaling.
        /// Align the min-x of the element's ‘viewBox’ with the smallest X value of the viewport.
        /// Align the midpoint Y value of the element's ‘viewBox’ with the midpoint Y value of the viewport.
        /// </summary>
        XMinYMid,
        /// <summary>
        /// Force uniform scaling. (the default)
        /// Align the midpoint X value of the element's ‘viewBox’ with the midpoint X value of the viewport.
        /// Align the midpoint Y value of the element's ‘viewBox’ with the midpoint Y value of the viewport.
        /// </summary>
        XMidYMid,
        /// <summary>
        /// Force uniform scaling.
        /// Align the min-x + width of the element's ‘viewBox’ with the maximum X value of the viewport.
        /// Align the midpoint Y value of the element's ‘viewBox’ with the midpoint Y value of the viewport.
        /// </summary>
        XMaxYMid,
        /// <summary>
        /// Force uniform scaling.
        /// Align the min-x of the element's ‘viewBox’ with the smallest X value of the viewport.
        /// Align the min-y + height of the element's ‘viewBox’ with the maximum Y value of the viewport.
        /// </summary>
        XMinYMax,
        /// <summary>
        /// Force uniform scaling.
        /// Align the midpoint X value of the element's ‘viewBox’ with the midpoint X value of the viewport.
        /// Align the min-y + height of the element's ‘viewBox’ with the maximum Y value of the viewport.
        /// </summary>
        XMidYMax,
        /// <summary>
        /// Force uniform scaling.
        /// Align the min-x + width of the element's ‘viewBox’ with the maximum X value of the viewport.
        /// Align the min-y + height of the element's ‘viewBox’ with the maximum Y value of the viewport.
        /// </summary>
        XMaxYMax
    }
}