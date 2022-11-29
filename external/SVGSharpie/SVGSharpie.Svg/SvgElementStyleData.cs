using System;
using System.Reflection;
using SVGSharpie.Css;

namespace SVGSharpie
{
    /// <summary>
    /// Represents a collection of CSS property-value pairs for an <see cref="SvgElement"/>
    /// See https://www.w3.org/TR/SVG11/propidx.html
    /// </summary>
    public sealed class SvgElementStyleData
    {
        /// <summary>
        /// Gets or sets the potential indirect value (<see cref="SvgPaintType.CurrentColor"/>) for the <see cref="Fill"/>, 
        /// <see cref="Stroke"/>, 'stop-color', 'flood-color' and 'lighting-color' properties
        /// </summary>
        public StyleProperty<SvgColor>? Color { get; set; }

        /// <summary>
        /// Gets or sets the reference to the clip path
        /// </summary>
        public StyleProperty<SvgFuncIRI> ClipPath { get; set; }

        /// <summary>
        /// Gets or sets the display value which controls visibility of an element
        /// </summary>
        public StyleProperty<CssDisplayType>? Display { get; set; }

        /// <summary>
        /// Gets or sets the paint to paint the interior of the element.  The area to be painted consists of any areas 
        /// inside the outline of the shape. To determine the inside of the shape, all subpaths are considered, and the 
        /// interior is determined according to the rules associated with the current value of the ‘fill-rule’ property
        /// </summary>
        public StyleProperty<SvgPaint> Fill { get; set; }

        /// <summary>
        /// Gets or sets the opacity of the painting operation used to paint the interior the current object
        /// </summary>
        public StyleProperty<float>? FillOpacity { get; set; }

        /// <summary>
        /// Gets or sets the algorithm to be used to determine what parts of the canvas are included inside the shape.
        /// </summary>
        public StyleProperty<SvgFillRule>? FillRule { get; set; }

        /// <summary>
        /// Gets or sets the paint to paint along the outline of the element
        /// </summary>
        public StyleProperty<SvgPaint> Stroke { get; set; }

        /// <summary>
        /// Gets or sets the opacity of the painting operation used to stroke the current object
        /// </summary>
        public StyleProperty<float>? StrokeOpacity { get; set; }

        /// <summary>
        /// Gets or sets the width of the stroke on the current object. If a percentage is used, the value represents 
        /// a percentage of the current viewport.
        /// </summary>
        public StyleProperty<SvgLength>? StrokeWidth { get; set; }

        /// <summary>
        /// Gets or sets the shape to be used at the end of open subpaths when they are stroked
        /// </summary>
        public StyleProperty<SvgStrokeLineCap>? StrokeLineCap { get; set; }

        /// <summary>
        /// Gets or sets the shape to be used at the corners of paths or basic shapes when they are stroked
        /// </summary>
        public StyleProperty<SvgStrokeLineJoin>? StrokeLineJoin { get; set; }

        /// <summary>
        /// Gets or sets the limit on the ratio of the miter length to the <see cref="StrokeWidth"/>. The value must 
        /// be greater than or equal to 1.
        /// </summary>
        public StyleProperty<float>? StrokeMiterLimit { get; set; }

        /// <summary>
        /// Gets or sets the array of lengths which control the pattern of dashes and gaps used to stroke paths.  
        /// Contains a list of values that specify the lengths alternating dashes and gaps.
        /// </summary>
        /// <remarks>
        /// If an odd number of values is provided, then the list of values is repeated to yield an even number of values. 
        /// Thus, stroke-dasharray: 5,3,2 is equivalent to stroke-dasharray: 5,3,2,5,3,2.
        /// </remarks>
        public StyleProperty<SvgLength[]> StrokeDashArray { get; set; }

        /// <summary>
        /// Gets or sets the distance into the dash pattern to start the dash.
        /// </summary>
        public StyleProperty<SvgLength>? StrokeDashOffset { get; set; }

        /// <summary>
        /// Gets or sets the visibility value which controls visibility of an element
        /// </summary>
        public StyleProperty<CssVisibilityType>? Visibility { get; set; }

        /// <summary>
        /// Gets or sets the current font size. If a percentage is used, the value represents 
        /// a percentage of the current viewport.
        /// </summary>
        public StyleProperty<SvgLength>? FontSize { get; set; }

        /// <summary>
        /// Gets or sets the current font family.
        /// </summary>
        public StyleProperty<string[]> FontFamily { get; set; }

        public StyleProperty<CssTextAnchorType>? TextAnchor { get; set; }

        public void Populate(string styleStr) => SvgElementStyleDataDeserializer.Populate(this, styleStr);

        public bool TryPopulateProperty(string name, string value) =>
            TryPopulateProperty(name, new CssStylePropertyValue(value));

        public bool TryPopulateProperty(string name, CssStylePropertyValue value) =>
            SvgElementStyleDataDeserializer.TryPopulateProperty(this, name, value);

        public void CopyTo(SvgElementStyleData other)
        {
            // todo: reflection, optimize

            foreach (var property in StyleProperties.Value)
            {
                var value = property.GetValue(this);
                if (value != null)
                {
                    property.SetValue(other, value);
                }
            }
        }

        private static readonly Lazy<PropertyInfo[]> StyleProperties = new Lazy<PropertyInfo[]>(() =>
            typeof(SvgElementStyleData).GetProperties(BindingFlags.Instance | BindingFlags.Public));
    }
}