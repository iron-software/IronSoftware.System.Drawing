namespace SVGSharpie
{
    public interface ISvgElementComputedStyle
    {
        /// <summary>
        /// Gets the potential indirect value (<see cref="SvgPaintType.CurrentColor"/>) for the <see cref="Fill"/>, 
        /// <see cref="Stroke"/>, 'stop-color', 'flood-color' and 'lighting-color' properties
        /// </summary>
        StyleProperty<SvgColor> Color { get; }

        /// <summary>
        /// Gets the display value which controls visibility of an element
        /// </summary>
        StyleProperty<CssDisplayType> Display { get; }

        /// <summary>
        /// Gets the paint to paint the interior of the element.  The area to be painted consists of any areas inside 
        /// the outline of the shape. To determine the inside of the shape, all subpaths are considered, and the 
        /// interior is determined according to the rules associated with the current value of the ‘fill-rule’ property
        /// </summary>
        StyleProperty<SvgPaint> Fill { get; }

        /// <summary>
        /// Gets the opacity of the painting operation used to paint the interior the current object
        /// </summary>
        StyleProperty<float> FillOpacity { get; }

        /// <summary>
        /// Gets the algorithm to be used to determine what parts of the canvas are included inside the shape.
        /// </summary>
        StyleProperty<SvgFillRule> FillRule { get; }

        /// <summary>
        /// Gets the paint to paint along the outline of the element
        /// </summary>
        StyleProperty<SvgPaint> Stroke { get; }

        /// <summary>
        /// Gets the width of the stroke on the current object. If a percentage is used, the value represents a 
        /// percentage of the current viewport.
        /// </summary>
        StyleProperty<SvgLength> StrokeWidth { get; }

        /// <summary>
        /// Gets the shape to be used at the end of open subpaths when they are stroked
        /// </summary>
        StyleProperty<SvgStrokeLineCap> StrokeLineCap { get; }

        /// <summary>
        /// Gets the shape to be used at the corners of paths or basic shapes when they are stroked
        /// </summary>
        StyleProperty<SvgStrokeLineJoin> StrokeLineJoin { get; }

        /// <summary>
        /// Gets the limit on the ratio of the miter length to the <see cref="StrokeWidth"/>. The value must be 
        /// greater than or equal to 1.
        /// </summary>
        StyleProperty<float> StrokeMiterLimit { get; }

        /// <summary>
        /// Gets the opacity of the painting operation used to stroke the current object
        /// </summary>
        StyleProperty<float> StrokeOpacity { get; }

        /// <summary>
        /// Gets the array of lengths which control the pattern of dashes and gaps used to stroke paths.  Contains a 
        /// list of values that specify the lengths alternating dashes and gaps.
        /// </summary>
        /// <remarks>
        /// If an odd number of values is provided, then the list of values is repeated to yield an even number of values. 
        /// Thus, stroke-dasharray: 5,3,2 is equivalent to stroke-dasharray: 5,3,2,5,3,2.
        /// </remarks>
        StyleProperty<SvgLength[]> StrokeDashArray { get; }

        /// <summary>
        /// Gets the distance into the dash pattern to start the dash.
        /// </summary>
        StyleProperty<SvgLength> StrokeDashOffset { get; }

        /// <summary>
        /// Gets the visibility value which controls visibility of an element
        /// </summary>
        StyleProperty<CssVisibilityType> Visibility { get; }

        /// <summary>
        /// Gets the current font size. If a percentage is used, the value represents 
        /// a percentage of the current viewport.
        /// </summary>
        StyleProperty<SvgLength> FontSize { get; }

        /// <summary>
        /// Gets the current font family.
        /// </summary>
        StyleProperty<string[]> FontFamily { get; }

        /// <summary>
        /// Gets the text alignment to use while rendering textual elements.
        /// </summary>
        StyleProperty<CssTextAnchorType> TextAnchor { get; }
    }
}