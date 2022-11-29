namespace SVGSharpie
{
    public enum SvgLengthType
    {
        /// <summary>
        /// SVG_LENGTHTYPE_NUMBER, No unit type was provided
        /// </summary>
        Number = 1,
        /// <summary>
        /// SVG_LENGTHTYPE_PERCENTAGE, A percentage value was specified.
        /// </summary>
        Percentage = 2,
        /// <summary>
        /// SVG_LENGTHTYPE_EMS, A value was specified using the em units defined in CSS2.
        /// </summary>
        ems = 3,
        /// <summary>
        /// SVG_LENGTHTYPE_EXS, A value was specified using the ex units defined in CSS2.
        /// </summary>
        exs = 4,
        /// <summary>
        /// SVG_LENGTHTYPE_PX, A value was specified using the px units defined in CSS2.
        /// </summary>
        px = 5,
        /// <summary>
        /// SVG_LENGTHTYPE_CM, A value was specified using the cm units defined in CSS2.
        /// </summary>
        cm = 6,
        /// <summary>
        /// SVG_LENGTHTYPE_MM, A value was specified using the mm units defined in CSS2.
        /// </summary>
        mm = 7,
        /// <summary>
        /// SVG_LENGTHTYPE_IN, A value was specified using the in units defined in CSS2.
        /// </summary>
        @in = 8,
        /// <summary>
        /// SVG_LENGTHTYPE_PT, A value was specified using the pt units defined in CSS2.
        /// </summary>
        pt = 9,
        /// <summary>
        /// SVG_LENGTHTYPE_PC, A value was specified using the pc units defined in CSS2.
        /// </summary>
        pc = 10
    }
}