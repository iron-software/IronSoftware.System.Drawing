using System;

namespace SVGSharpie
{
    /// <summary>
    /// A length is a distance measurement, given as a number along with a unit which may be optional.
    /// </summary>
    public struct SvgLength
    {
        private readonly SvgLengthContext _context;

        /// <summary>
        /// Gets the unit type of the length
        /// </summary>
        public SvgLengthType LengthType { get; }

        /// <summary>
        /// Gets the value as a floating point value, in user units
        /// </summary>
        public float Value
        {
            get
            {
                float? total = null;
                if (LengthType == SvgLengthType.Percentage)
                {
                    total = _context.ComputeTotalLength();
                }
                return GetAbsoluteValue(total);
            }
        }

        /// <summary>
        /// Gets the value as a floating point value, in the units expressed by <see cref="LengthType"/>
        /// </summary>
        public float ValueInSpecifiedUnits { get; }

        internal SvgLength(float value)
        {
            LengthType = SvgLengthType.Number;
            ValueInSpecifiedUnits = value;
            _context = SvgLengthContext.Null;
        }
        
        internal SvgLength(string value, SvgElement element, SvgLengthDirection direction)
            : this(value, new SvgLengthElementContext(element, direction))
        {
        }

        internal SvgLength(string value, SvgLengthContext context)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            value = value.Trim();
            var unit = value.IndexOfAny(new[] { '%', 'e', 'p', 'c', 'm', 'i' });
            if (unit < 0)
            {
                LengthType = SvgLengthType.Number;
                ValueInSpecifiedUnits = float.Parse(value);
            }
            else
            {
                ValueInSpecifiedUnits = float.Parse(value.Substring(0, unit));
                switch (value.Substring(unit).ToLowerInvariant())
                {
                    case "%":
                        LengthType = SvgLengthType.Percentage;
                        break;
                    case "ems":
                        LengthType = SvgLengthType.ems;
                        break;
                    case "exs":
                        LengthType = SvgLengthType.exs;
                        break;
                    case "px":
                        LengthType = SvgLengthType.px;
                        break;
                    case "cm":
                        LengthType = SvgLengthType.cm;
                        break;
                    case "mm":
                        LengthType = SvgLengthType.mm;
                        break;
                    case "in":
                        LengthType = SvgLengthType.@in;
                        break;
                    case "pt":
                        LengthType = SvgLengthType.pt;
                        break;
                    case "pc":
                        LengthType = SvgLengthType.pc;
                        break;
                    default:
                        throw new ArgumentException($"Invalid unit specified on length '{value}'", nameof(value));
                }
            }
        }

        /// <summary>
        /// Gets the value as a floating point value, in user units
        /// </summary>
        public float GetAbsoluteValue(float? total)
        {
            const float dpi = 90;

            // see https://www.w3.org/TR/SVG/coords.html#Units

            switch (LengthType)
            {
                case SvgLengthType.Number:
                case SvgLengthType.px:
                    return ValueInSpecifiedUnits;
                case SvgLengthType.cm:
                    return ValueInSpecifiedUnits * dpi / 2.54f;
                case SvgLengthType.mm:
                    return ValueInSpecifiedUnits * dpi / 25.4f;
                case SvgLengthType.@in:
                    return ValueInSpecifiedUnits * dpi;
                case SvgLengthType.pt:
                    return ValueInSpecifiedUnits * (dpi / 72);
                case SvgLengthType.pc:
                    return ValueInSpecifiedUnits * 12 * (dpi / 72);
                case SvgLengthType.Percentage:
                    if (!total.HasValue)
                    {
                        throw new NotSupportedException($"{ToString()} can't be converted to an absolute value");
                    }
                    return (ValueInSpecifiedUnits / 100) * total.Value;
                case SvgLengthType.exs:
                case SvgLengthType.ems:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            string suffix;
            switch (LengthType)
            {
                case SvgLengthType.Percentage:
                    suffix = "%";
                    break;
                case SvgLengthType.ems:
                    suffix = "ems";
                    break;
                case SvgLengthType.exs:
                    suffix = "exs";
                    break;
                case SvgLengthType.px:
                    suffix = "px";
                    break;
                case SvgLengthType.cm:
                    suffix = "cm";
                    break;
                case SvgLengthType.mm:
                    suffix = "mm";
                    break;
                case SvgLengthType.@in:
                    suffix = "in";
                    break;
                case SvgLengthType.pt:
                    suffix = "pt";
                    break;
                case SvgLengthType.pc:
                    suffix = "pc";
                    break;
                case SvgLengthType.Number:
                    suffix = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return $"{ValueInSpecifiedUnits}{suffix}";
        }
    }
}