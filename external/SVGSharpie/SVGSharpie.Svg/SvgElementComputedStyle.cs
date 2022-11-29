using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SVGSharpie
{
    /// <summary>
    /// Represents the style computed for an SVG element based on a set of matching style data
    /// </summary>
    public sealed class SvgElementComputedStyle : ISvgElementComputedStyle
    {
        /// <summary>
        /// Gets or sets the parent element from which to lookup the style property values from if they are not 
        /// defined in any of the <see cref="StyleDatas"/>
        /// </summary>
        public ISvgElementComputedStyle Parent
        {
            get => _parent;
            set => _parent = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Gets or sets the ordered collection of style datas to lookup values from, if multiple styles define a 
        /// value for the same property the first matching value will be returned.  If a value is not defined for a 
        /// property the lookup will be deferred to the <see cref="Parent"/> computed style
        /// </summary>
        public SvgElementStyleData[] StyleDatas
        {
            get => _styleDatas;
            set => _styleDatas = value ?? throw new ArgumentNullException();
        }

        public StyleProperty<SvgColor> Color => Select(Parent.Color, x => x.Color);
        public StyleProperty<CssDisplayType> Display => Select(Parent.Display, x => x.Display, i => i == CssDisplayType.Inherit);
        public StyleProperty<SvgPaint> Fill => Select(Parent.Fill, x => x.Fill);
        public StyleProperty<float> FillOpacity => Select(Parent.FillOpacity, x => x.FillOpacity);
        public StyleProperty<SvgFillRule> FillRule => Select(Parent.FillRule, x => x.FillRule, i => i == SvgFillRule.Inherit);
        public StyleProperty<SvgPaint> Stroke => Select(Parent.Stroke, x => x.Stroke);
        public StyleProperty<SvgLength> StrokeWidth => Select(Parent.StrokeWidth, x => x.StrokeWidth);
        public StyleProperty<SvgStrokeLineCap> StrokeLineCap => Select(Parent.StrokeLineCap, x => x.StrokeLineCap, i => i == SvgStrokeLineCap.Inherit);
        public StyleProperty<SvgStrokeLineJoin> StrokeLineJoin => Select(Parent.StrokeLineJoin, x => x.StrokeLineJoin, i => i == SvgStrokeLineJoin.Inherit);
        public StyleProperty<float> StrokeMiterLimit => Select(Parent.StrokeMiterLimit, x => x.StrokeMiterLimit);
        public StyleProperty<float> StrokeOpacity => Select(Parent.StrokeOpacity, x => x.StrokeOpacity);
        public StyleProperty<SvgLength[]> StrokeDashArray => Select(Parent.StrokeDashArray, x => x.StrokeDashArray);
        public StyleProperty<SvgLength> StrokeDashOffset => Select(Parent.StrokeDashOffset, x => x.StrokeDashOffset);

        public StyleProperty<CssVisibilityType> Visibility => Select(Parent.Visibility, x => x.Visibility, i => i == CssVisibilityType.Inherit);

        public StyleProperty<SvgLength> FontSize => Select(Parent.FontSize, x => x.FontSize);

        public StyleProperty<string[]> FontFamily => Select(Parent.FontFamily, x => x.FontFamily, x => x == null || x.Length == 0);

        public StyleProperty<CssTextAnchorType> TextAnchor => Select(Parent.TextAnchor, x => x.TextAnchor, x => x == CssTextAnchorType.Inherit);

        public SvgElementComputedStyle(SvgElementStyleData data, ISvgElementComputedStyle parent)
            : this(new[] { data ?? throw new ArgumentNullException() }, parent)
        {
        }

        public SvgElementComputedStyle(SvgElementStyleData[] styles, ISvgElementComputedStyle parent)
        {
            StyleDatas = styles ?? throw new ArgumentNullException(nameof(styles));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        private StyleProperty<T> Select<T>(StyleProperty<T> parentProperty, Expression<Func<SvgElementStyleData, StyleProperty<T>>> selector, Predicate<T> shouldInheritValue = null)
            where T : class
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;
            return Select(parentProperty, propertyInfo, v => v.Value != null, shouldInheritValue);
        }

        private StyleProperty<T> Select<T>(StyleProperty<T> parentProperty, Expression<Func<SvgElementStyleData, StyleProperty<T>?>> selector, Predicate<T> shouldInheritValue = null)
            where T : struct
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;
            return Select(parentProperty, propertyInfo, isValueSet: null, shouldInheritValue: shouldInheritValue);
        }

        private StyleProperty<T> Select<T>(StyleProperty<T> parentProperty, PropertyInfo propertyInfo, Predicate<StyleProperty<T>> isValueSet, Predicate<T> shouldInheritValue = null)
        {
            // if the parent property was specified as !important, return it without checking the descendant properties

            if (parentProperty.IsImportant)
            {
                return parentProperty;
            }

            StyleProperty<T>? candidate = null;

            foreach (var data in StyleDatas)
            {
                var value = propertyInfo.GetMethod.Invoke(data, null);
                if (!(value is StyleProperty<T> valueAsProperty) || (isValueSet != null && !isValueSet(valueAsProperty)))
                {
                    // property has no value, default to 'inherit' so skip it
                    continue;
                }

                var result = (StyleProperty<T>)value;
                if (shouldInheritValue != null && shouldInheritValue(result.Value))
                {
                    // property is specified as 'inherit' so skip it
                    continue;
                }

                if (result.IsImportant)
                {
                    // property is specified as !important so return it, we don't need to check other candidates
                    return result;
                }

                if (candidate == null)
                {
                    // candidate property unless another descendant property is marked as !important
                    candidate = result;
                }
            }

            return candidate ?? parentProperty;
        }

        private ISvgElementComputedStyle _parent;
        private SvgElementStyleData[] _styleDatas;
    }
}