using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;
using SVGSharpie.Css;

namespace SVGSharpie
{
    internal static class SvgElementStyleDataDeserializer
    {
        private static readonly SvgLength[] EmptyLengths = new SvgLength[0];
        private static readonly char[] StrokeDashArraySplitChars = { ' ', ',', '\t' };

        private static readonly Dictionary<string, Action<SvgElementStyleData, CssStylePropertyValue>> StylePropertyParserSetters = new Dictionary<string, Action<SvgElementStyleData, CssStylePropertyValue>>
        {
            ["color"] = CreateParserAndSetterForNullable(SvgColorTranslator.FromSvgColorCode, s => s.Color),
            ["clip-path"] = CreateParserAndSetter(SvgFuncIRI.Parse, s => s.ClipPath),
            ["display"] = CreateXmlEnumParserAndSetter(s => s.Display),
            ["fill"] = CreateParserAndSetter(SvgPaint.Parse, s => s.Fill),
            ["fill-rule"] = CreateXmlEnumParserAndSetter(s => s.FillRule),
            ["fill-opacity"] = CreateParserAndSetterForInheritableNumber(s => s.FillOpacity),
            ["stroke"] = CreateParserAndSetter(SvgPaint.Parse, s => s.Stroke),
            ["stroke-width"] = CreateParserAndSetterForNullable(v => new SvgLength(v, SvgLengthContext.Null), s => s.StrokeWidth),
            ["stroke-linecap"] = CreateXmlEnumParserAndSetter(s => s.StrokeLineCap),
            ["stroke-linejoin"] = CreateXmlEnumParserAndSetter(s => s.StrokeLineJoin),
            ["stroke-miterlimit"] = CreateParserAndSetterForNullable(float.Parse, s => s.StrokeMiterLimit),
            ["visibility"] = CreateXmlEnumParserAndSetter(s => s.Visibility),
            ["stroke-opacity"] = CreateParserAndSetterForInheritableNumber(s => s.StrokeOpacity),
            ["stroke-dasharray"] = CreateParserAndSetter(v =>
            {
                if (v == "none") return EmptyLengths;
                return
                    v.Split(StrokeDashArraySplitChars, StringSplitOptions.RemoveEmptyEntries)
                        .Select(i => new SvgLength(i, SvgLengthContext.Null)).ToArray();
            }, s => s.StrokeDashArray),
            ["stroke-dashoffset"] = CreateParserAndSetterForNullable(v => new SvgLength(v, SvgLengthContext.Null), s => s.StrokeDashOffset),

            ["shape-rendering"] = (s, v) => { /* nop */ },
            ["font-size"] = CreateParserAndSetterForNullable(v => new SvgLength(v, SvgLengthContext.Null), s => s.FontSize),
            ["font-family"] = (s, v) =>
            {
                if (v.IsInherit)
                {
                    s.FontFamily = StyleProperty.Create(new string[0]);
                }
                else
                {
                    s.FontFamily = StyleProperty.Create(v.Value.Split(new[] { ',' }).Select(x => x.Trim(' ', '\'', '"')).ToArray());
                }
            },
            ["text-anchor"] = CreateXmlEnumParserAndSetter(c => c.TextAnchor),
            ["opacity"] = (s, v) =>
            {
                StyleProperty<float>? value;
                if (v.IsInherit)
                {
                    value = null;
                }
                else
                {
                    var parsed = Math.Max(0, Math.Min(1, float.Parse(v.Value)));
                    value = new StyleProperty<float>(parsed, v.IsImportant);
                }
                s.FillOpacity = value;
                s.StrokeOpacity = value;
            }
        };

        public static SvgElementStyleData Populate(SvgElementStyleData result, string styleStr)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (styleStr == null) throw new ArgumentNullException(nameof(styleStr));
            var reader = new CssStringStreamReader(styleStr);
            if (!CssStyleRulesParser.TryParseStyleRuleBodyProperties(reader, out var properties))
            {
                throw new CssParserException(reader, "Failed to parse style body");
            }
            foreach (var property in properties)
            {
                if (!TryPopulateProperty(result, property.Key, property.Value))
                {
                    // skip unrecognised
                    // throw new Exception($"Unknown style property '{property.Key}' with value '{property.Value}'");
                }
            }
            return result;
        }

        public static bool TryPopulateProperty(SvgElementStyleData result, string name, CssStylePropertyValue value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (!StylePropertyParserSetters.TryGetValue(name, out var parserSetter))
            {
                return false;
            }

            try
            {
                parserSetter(result, value);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set style property '{name}' to css value '{value}'", e);
            }
            return true;
        }

        private static Action<SvgElementStyleData, CssStylePropertyValue> CreateXmlEnumParserAndSetter<T>(Expression<Func<SvgElementStyleData, StyleProperty<T>?>> propertyResolver)
            where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Expected '{typeof(T)}' to be an enum");
            var values = typeof(T).GetMembers(BindingFlags.Static | BindingFlags.Public);
            var mapping = values.ToDictionary(i =>
                    i.GetCustomAttribute<XmlEnumAttribute>()?.Name ?? throw new Exception($"Expected member '{i.Name}' of '{typeof(T)}' to have '{nameof(XmlEnumAttribute)}'"),
                i => (T)((FieldInfo)i).GetValue(null));
            var propertyInfo = GetPropertyInfo(propertyResolver);

            void Result(SvgElementStyleData style, CssStylePropertyValue value)
            {
                if (!mapping.TryGetValue(value.Value, out var enumValue))
                {
                    throw new Exception($"Value '{value.Value}' is an invalid value for '{typeof(T)}'");
                }

                propertyInfo.SetValue(style, new StyleProperty<T>(enumValue, value.IsImportant));
            }

            return Result;
        }

        private static Action<SvgElementStyleData, CssStylePropertyValue> CreateParserAndSetterForInheritableNumber(Expression<Func<SvgElementStyleData, StyleProperty<float>?>> propertyResolver)
        {
            var propertyInfo = GetPropertyInfo(propertyResolver);
            void Result(SvgElementStyleData style, CssStylePropertyValue value)
            {
                if (value.IsInherit)
                {
                    propertyInfo.SetValue(style, null);
                }
                else
                {
                    var parsedValue = float.Parse(value.Value);
                    StyleProperty<float>? propertyValue = new StyleProperty<float>(parsedValue, isImportant: value.IsImportant);
                    propertyInfo.SetValue(style, propertyValue);
                }
            }
            return Result;
        }

        private static Action<SvgElementStyleData, CssStylePropertyValue> CreateParserAndSetterForNullable<T>(Func<string, T> parser, Expression<Func<SvgElementStyleData, StyleProperty<T>?>> propertyResolver)
            where T : struct => CreateParserAndSetter(parser, GetPropertyInfo(propertyResolver));

        private static Action<SvgElementStyleData, CssStylePropertyValue> CreateParserAndSetter<T>(Func<string, T> parser, Expression<Func<SvgElementStyleData, StyleProperty<T>>> propertyResolver)
            where T : class => CreateParserAndSetter(parser, GetPropertyInfo(propertyResolver));

        private static Action<SvgElementStyleData, CssStylePropertyValue> CreateParserAndSetter<T>(Func<string, T> parser, PropertyInfo propertyInfo)
        {
            void Result(SvgElementStyleData style, CssStylePropertyValue value)
            {
                var parsedValue = value.IsInherit ? default(T) : parser(value.Value);
                var propertyValue = new StyleProperty<T>(parsedValue, isImportant: value.IsImportant);
                propertyInfo.SetValue(style, propertyValue);
            }
            return Result;
        }

        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<SvgElementStyleData, T>> expression)
        {
            if (expression.Body is UnaryExpression unary)
            {
                if (unary.Operand is MemberExpression member)
                {
                    return (PropertyInfo)member.Member;
                }
            }
            else if (expression.Body is MemberExpression member)
            {
                return (PropertyInfo)member.Member;
            }
            throw new ArgumentException();
        }
    }
}