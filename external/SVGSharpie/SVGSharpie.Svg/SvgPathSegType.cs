namespace SVGSharpie
{
    /// <summary>
    /// Path Segment Types
    /// </summary>
    /// <remarks>
    /// https://www.w3.org/TR/SVG11/paths.html (8.5.1 Interface SVGPathSeg)
    /// </remarks>
    public enum SvgPathSegType
    {
        /// <summary>
        /// PATHSEG_CLOSEPATH, Corresponds to a "closepath" (z) path data command.
        /// </summary>
        ClosePath = 1,
        /// <summary>
        /// PATHSEG_MOVETO_ABS, Corresponds to a "absolute moveto" (M) path data command.
        /// </summary>
        MoveToAbs = 2,
        /// <summary>
        /// PATHSEG_MOVETO_REL, Corresponds to a "relative moveto" (m) path data command.
        /// </summary>
        MoveToRel = 3,
        /// <summary>
        /// PATHSEG_LINETO_ABS, Corresponds to a "absolute lineto" (L) path data command.
        /// </summary>
        LineToAbs = 4,
        /// <summary>
        /// PATHSEG_LINETO_REL, Corresponds to a "relative lineto" (l) path data command.
        /// </summary>
        LineToRel = 5,
        /// <summary>
        /// PATHSEG_CURVETO_CUBIC_ABS, Corresponds to a "absolute cubic Bézier curveto" (C) path data command.
        /// </summary>
        CurvetoCubicAbs = 6,
        /// <summary>
        /// PATHSEG_CURVETO_CUBIC_REL, Corresponds to a "relative cubic Bézier curveto" (c) path data command.
        /// </summary>
        CurvetoCubicRel = 7,
        /// <summary>
        /// PATHSEG_CURVETO_QUADRATIC_ABS, Corresponds to a "absolute quadratic Bézier curveto" (Q) path data command.
        /// </summary>
        CurvetoQuadraticAbs = 8,
        /// <summary>
        /// PATHSEG_CURVETO_QUADRATIC_REL, Corresponds to a "relative quadratic Bézier curveto" (q) path data command.
        /// </summary>
        CurvetoQuadraticRel = 9,
        /// <summary>
        /// PATHSEG_ARC_ABS, Corresponds to a "absolute arcto" (A) path data command.
        /// </summary>
        ArcAbs = 10,
        /// <summary>
        /// PATHSEG_ARC_REL, Corresponds to a "relative arcto" (a) path data command.
        /// </summary>
        ArcRel = 11,
        /// <summary>
        /// PATHSEG_LINETO_HORIZONTAL_ABS, Corresponds to a "absolute horizontal lineto" (H) path data command.
        /// </summary>
        LinetoHorizontalAbs = 12,
        /// <summary>
        /// PATHSEG_LINETO_HORIZONTAL_REL, Corresponds to a "relative horizontal lineto" (h) path data command.
        /// </summary>
        LinetoHorizontalRel = 13,
        /// <summary>
        /// PATHSEG_LINETO_VERTICAL_ABS, Corresponds to a "absolute vertical lineto" (V) path data command.
        /// </summary>
        LinetoVerticalAbs = 14,
        /// <summary>
        /// PATHSEG_LINETO_VERTICAL_REL, Corresponds to a "relative vertical lineto" (v) path data command.
        /// </summary>
        LinetoVerticalRel = 15,
        /// <summary>
        /// PATHSEG_CURVETO_CUBIC_SMOOTH_ABS, Corresponds to a "absolute smooth cubic curveto" (S) path data command.
        /// </summary>
        CurvetoCubicSmoothAbs = 16,
        /// <summary>
        /// PATHSEG_CURVETO_CUBIC_SMOOTH_REL, Corresponds to a "relative smooth cubic curveto" (s) path data command.
        /// </summary>
        CurvetoCubicSmoothRel = 17,
        /// <summary>
        /// PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS, Corresponds to a "absolute smooth quadratic curveto" (T) path data command.
        /// </summary>
        CurvetoQuadraticSmoothAbs = 18,
        /// <summary>
        /// PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL, Corresponds to a "relative smooth quadratic curveto" (t) path data command.
        /// </summary>
        CurvetoQuadraticSmoothRel = 19
    }
}