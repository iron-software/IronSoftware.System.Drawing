using System;

namespace SVGSharpie.Css
{
    public struct CssStylePropertyValue
    {
        /// <summary>
        /// Gets the value of the CSS Property
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets a value indicating whether the value was declared with "!important"
        /// </summary>
        public bool IsImportant { get; }

        /// <summary>
        /// Gets a value indicating whether the value is 'inherit'
        /// </summary>
        public bool IsInherit => string.Equals(Value, "inherit", StringComparison.OrdinalIgnoreCase);

        public CssStylePropertyValue(string value)
        {
            Value = value?.Trim() ?? throw new ArgumentNullException(nameof(value));

            // todo: whitespace can exist between the '!' and 'important', not checking for that currently...

            const string important = "!important";

            if (Value.EndsWith(important))
            {
                Value = Value.Substring(0, value.Length - important.Length - 1).Trim();
                IsImportant = true;
            }
            else
            {
                IsImportant = false;
            }
        }

        public override string ToString() => IsImportant ? $"{Value} !important" : Value;
    }
}