namespace SVGSharpie
{
    public struct StyleProperty<T>
    {
        public T Value { get; }

        public bool IsImportant { get; }

        public StyleProperty(T value, bool isImportant)
        {
            Value = value;
            IsImportant = isImportant;
        }

        public static implicit operator T (StyleProperty<T> property) => property.Value;
    }

    public static class StyleProperty
    {
        public static StyleProperty<T> Create<T>(T value, bool isImportant = false)
        {
            return new StyleProperty<T>(value, isImportant);
        }
    }
}