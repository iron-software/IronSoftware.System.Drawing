namespace SVGSharpie
{
    public enum SvgPreserveAspectRatioMeetOrSlice
    {
        /// <summary>
        /// Scale the graphic such that:
        /// - aspect ratio is preserved
        /// - the entire ‘viewBox’ is visible within the viewport
        /// - the ‘viewBox’ is scaled up as much as possible, while still meeting the other criteria
        /// 
        /// In this case, if the aspect ratio of the graphic does not match the viewport, some of the viewport will 
        /// extend beyond the bounds of the ‘viewBox’ (i.e., the area into which the ‘viewBox’ will draw will be 
        /// smaller than the viewport).
        /// </summary>
        Meet,

        /// <summary>
        /// Scale the graphic such that:
        /// - aspect ratio is preserved
        /// - the entire viewport is covered by the ‘viewBox’
        /// - the ‘viewBox’ is scaled down as much as possible, while still meeting the other criteria
        /// 
        /// In this case, if the aspect ratio of the ‘viewBox’ does not match the viewport, some of the ‘viewBox’ will extend beyond the bounds of the viewport (i.e., the area into which the ‘viewBox’ will draw is larger than the viewport).
        /// </summary>
        Slice
    }
}