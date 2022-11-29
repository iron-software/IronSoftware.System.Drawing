namespace SVGSharpie
{
    public struct SvgColor
    {
        public static readonly SvgColor Black = new SvgColor(0, 0, 0);

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public SvgColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public SvgColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString() => $"#{R:x2}{G:x2}{B:x2}{A:x2}";
    }
}