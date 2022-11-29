namespace SVGSharpie
{
    /// <summary>
    /// Computed style which contains the SVG default style values
    /// </summary>
    public sealed class SvgElementDefaultStyle : ISvgElementComputedStyle
    {
        public static readonly SvgElementDefaultStyle Instance = new SvgElementDefaultStyle();

        public StyleProperty<SvgColor> Color => StyleProperty.Create(SvgColor.Black);
        public StyleProperty<CssDisplayType> Display => StyleProperty.Create(CssDisplayType.Inline);
        public StyleProperty<SvgPaint> Fill => StyleProperty.Create(DefaultFill);
        public StyleProperty<float> FillOpacity => StyleProperty.Create(1f);
        public StyleProperty<SvgFillRule> FillRule => StyleProperty.Create(SvgFillRule.NonZero);
        public StyleProperty<SvgPaint> Stroke => StyleProperty.Create(SvgPaint.None);
        public StyleProperty<SvgLength> StrokeWidth => StyleProperty.Create(new SvgLength(1));
        public StyleProperty<SvgStrokeLineCap> StrokeLineCap => StyleProperty.Create(SvgStrokeLineCap.Butt);
        public StyleProperty<SvgStrokeLineJoin> StrokeLineJoin => StyleProperty.Create(SvgStrokeLineJoin.Miter);
        public StyleProperty<float> StrokeMiterLimit => StyleProperty.Create(4f);
        public StyleProperty<float> StrokeOpacity => StyleProperty.Create(1f);
        public StyleProperty<SvgLength[]> StrokeDashArray => StyleProperty.Create(EmptyLengths);     // Equivalent to 'none'
        public StyleProperty<SvgLength> StrokeDashOffset => StyleProperty.Create(new SvgLength(0));
        public StyleProperty<CssVisibilityType> Visibility => StyleProperty.Create(CssVisibilityType.Visible);

        public StyleProperty<SvgLength> FontSize => StyleProperty.Create(new SvgLength(12));

        public StyleProperty<string[]> FontFamily => StyleProperty.Create(new[] { "serif" });

        public StyleProperty<CssTextAnchorType> TextAnchor => StyleProperty.Create(CssTextAnchorType.Start);

        private SvgElementDefaultStyle()
        {
        }

        private static readonly SvgPaint DefaultFill = new SvgPaint(SvgPaintType.ExplicitColor, SvgColor.Black);

        private static readonly SvgLength[] EmptyLengths = new SvgLength[0];
    }
}