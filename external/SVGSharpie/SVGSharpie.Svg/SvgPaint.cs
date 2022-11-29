using System;

namespace SVGSharpie
{
    public sealed class SvgPaint
    {
        private readonly SvgColor _color;
        private readonly string _iriReference;

        public static readonly SvgPaint None = new SvgPaint(SvgPaintType.None, SvgColor.Black);

        public SvgPaintType PaintType { get; }

        /// <summary>
        /// Gets a value indicating whether no paint is applied
        /// </summary>
        public bool IsNone => PaintType == SvgPaintType.None;
        
        /// <summary>
        /// Gets a value indicating whether an explicity color is specified
        /// </summary>
        public bool HasColor => PaintType == SvgPaintType.ExplicitColor;

        /// <summary>
        /// Gets the explicit color specified or throws an exception
        /// </summary>
        public SvgColor Color => HasColor ? _color : throw new InvalidOperationException();

        /// <summary>
        /// Gets the IRI reference if the paint is of type <see cref="SvgPaintType.IRIReference"/>
        /// </summary>
        public string Reference => PaintType == SvgPaintType.IRIReference ? _iriReference : throw new InvalidOperationException();
        
        public SvgPaint(SvgPaintType paintType, SvgColor color)
        {
            PaintType = paintType;
            _color = color;
        }

        public SvgPaint(string iriReference)
        {
            PaintType = SvgPaintType.IRIReference;
            _iriReference = iriReference ?? throw new ArgumentNullException(nameof(iriReference));
        }

        public static SvgPaint Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var trimmedValue = value.Trim();
            if (trimmedValue.StartsWith("url", StringComparison.OrdinalIgnoreCase))
            {
                var open = trimmedValue.IndexOf('(') + 1;
                var close = trimmedValue.IndexOf(')', open);
                var refValue = trimmedValue.Substring(open, close - open);
                return new SvgPaint(refValue.Trim());
            }
            switch (trimmedValue.ToLowerInvariant())
            {
                case "none":
                    return new SvgPaint(SvgPaintType.None, SvgColor.Black);
                case "currentcolor":
                    return new SvgPaint(SvgPaintType.CurrentColor, SvgColor.Black);
                default:
                    return new SvgPaint(SvgPaintType.ExplicitColor, SvgColorTranslator.FromSvgColorCode(value));
            }
        }

        public override string ToString()
        {
            switch (PaintType)
            {
                case SvgPaintType.None:
                    return "none";
                case SvgPaintType.CurrentColor:
                    return "currentColor";
                case SvgPaintType.ExplicitColor:
                    return Color.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}