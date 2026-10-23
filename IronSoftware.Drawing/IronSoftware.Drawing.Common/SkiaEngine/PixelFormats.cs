#if NETFRAMEWORK
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// .NET Framework image engine (SkiaSharp + LibTiff), used instead of SixLabors.ImageSharp, which is
// end-of-life for .NET Framework. These internal types mirror the subset of the ImageSharp API that
// AnyBitmap uses, so AnyBitmap's logic is shared between the ImageSharp and SkiaSharp builds.
// Byte layouts match ImageSharp's pixel structs (AnyBitmap copies raw pixel bytes).
namespace IronSoftware.Drawing.SkiaEngine
{
    internal interface IPixel<TSelf> where TSelf : unmanaged, IPixel<TSelf>
    {
        Rgba32 ToRgba32();
        void FromRgba32(Rgba32 source);
        Rgba64 ToRgba64();
        void FromRgba64(Rgba64 source);
    }

    internal static class PixelMath
    {
        // Same rounding as ImageSharp's ColorNumerics.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte To8Bit(ushort component) => (byte)(((component * 255) + 32895) >> 16);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort To16Bit(byte component) => (ushort)(component * 257);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Luminance8(byte r, byte g, byte b) => (byte)((r * .2126F) + (g * .7152F) + (b * .0722F) + 0.5F);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Luminance16(ushort r, ushort g, ushort b) => (ushort)((r * .2126F) + (g * .7152F) + (b * .0722F) + 0.5F);

        public static int BitsPerPixel<TPixel>() where TPixel : unmanaged, IPixel<TPixel> => Unsafe.SizeOf<TPixel>() * 8;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rgba32 : IPixel<Rgba32>
    {
        public byte R, G, B, A;
        public Rgba32(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }
        public Rgba32 ToRgba32() => this;
        public void FromRgba32(Rgba32 s) => this = s;
        public Rgba64 ToRgba64() => new(PixelMath.To16Bit(R), PixelMath.To16Bit(G), PixelMath.To16Bit(B), PixelMath.To16Bit(A));
        public void FromRgba64(Rgba64 s) => this = s.ToRgba32();
        public static implicit operator Color(Rgba32 p) => new(p.A, p.R, p.G, p.B);
        public static implicit operator Rgba32(Color c) => new(c.R, c.G, c.B, c.A);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Bgra32 : IPixel<Bgra32>
    {
        public byte B, G, R, A;
        public Rgba32 ToRgba32() => new(R, G, B, A);
        public void FromRgba32(Rgba32 s) { R = s.R; G = s.G; B = s.B; A = s.A; }
        public Rgba64 ToRgba64() => ToRgba32().ToRgba64();
        public void FromRgba64(Rgba64 s) => FromRgba32(s.ToRgba32());
        public static implicit operator Color(Bgra32 p) => new(p.A, p.R, p.G, p.B);
        public static implicit operator Bgra32(Color c) => new() { R = c.R, G = c.G, B = c.B, A = c.A };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Abgr32 : IPixel<Abgr32>
    {
        public byte A, B, G, R;
        public Rgba32 ToRgba32() => new(R, G, B, A);
        public void FromRgba32(Rgba32 s) { R = s.R; G = s.G; B = s.B; A = s.A; }
        public Rgba64 ToRgba64() => ToRgba32().ToRgba64();
        public void FromRgba64(Rgba64 s) => FromRgba32(s.ToRgba32());
        public static implicit operator Color(Abgr32 p) => new(p.A, p.R, p.G, p.B);
        public static implicit operator Abgr32(Color c) => new() { R = c.R, G = c.G, B = c.B, A = c.A };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Argb32 : IPixel<Argb32>
    {
        public byte A, R, G, B;
        public Rgba32 ToRgba32() => new(R, G, B, A);
        public void FromRgba32(Rgba32 s) { R = s.R; G = s.G; B = s.B; A = s.A; }
        public Rgba64 ToRgba64() => ToRgba32().ToRgba64();
        public void FromRgba64(Rgba64 s) => FromRgba32(s.ToRgba32());
        public static implicit operator Color(Argb32 p) => new(p.A, p.R, p.G, p.B);
        public static implicit operator Argb32(Color c) => new() { R = c.R, G = c.G, B = c.B, A = c.A };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rgb24 : IPixel<Rgb24>
    {
        public byte R, G, B;
        public Rgba32 ToRgba32() => new(R, G, B, 255);
        public void FromRgba32(Rgba32 s) { R = s.R; G = s.G; B = s.B; }
        public Rgba64 ToRgba64() => ToRgba32().ToRgba64();
        public void FromRgba64(Rgba64 s) => FromRgba32(s.ToRgba32());
        public static implicit operator Color(Rgb24 p) => new(255, p.R, p.G, p.B);
        public static implicit operator Rgb24(Color c) => new() { R = c.R, G = c.G, B = c.B };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Bgr24 : IPixel<Bgr24>
    {
        public byte B, G, R;
        public Rgba32 ToRgba32() => new(R, G, B, 255);
        public void FromRgba32(Rgba32 s) { R = s.R; G = s.G; B = s.B; }
        public Rgba64 ToRgba64() => ToRgba32().ToRgba64();
        public void FromRgba64(Rgba64 s) => FromRgba32(s.ToRgba32());
        public static implicit operator Color(Bgr24 p) => new(255, p.R, p.G, p.B);
        public static implicit operator Bgr24(Color c) => new() { R = c.R, G = c.G, B = c.B };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct L8 : IPixel<L8>
    {
        public byte PackedValue;
        public Rgba32 ToRgba32() => new(PackedValue, PackedValue, PackedValue, 255);
        public void FromRgba32(Rgba32 s) => PackedValue = PixelMath.Luminance8(s.R, s.G, s.B);
        public Rgba64 ToRgba64() { ushort v = PixelMath.To16Bit(PackedValue); return new(v, v, v, ushort.MaxValue); }
        public void FromRgba64(Rgba64 s) => PackedValue = PixelMath.To8Bit(PixelMath.Luminance16(s.R, s.G, s.B));
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct L16 : IPixel<L16>
    {
        public ushort PackedValue;
        public Rgba32 ToRgba32() => ToRgba64().ToRgba32();
        public void FromRgba32(Rgba32 s) => FromRgba64(s.ToRgba64());
        public Rgba64 ToRgba64() => new(PackedValue, PackedValue, PackedValue, ushort.MaxValue);
        public void FromRgba64(Rgba64 s) => PackedValue = PixelMath.Luminance16(s.R, s.G, s.B);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct La16 : IPixel<La16>
    {
        public byte L, A;
        public Rgba32 ToRgba32() => new(L, L, L, A);
        public void FromRgba32(Rgba32 s) { L = PixelMath.Luminance8(s.R, s.G, s.B); A = s.A; }
        public Rgba64 ToRgba64() => ToRgba32().ToRgba64();
        public void FromRgba64(Rgba64 s) => FromRgba32(s.ToRgba32());
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct La32 : IPixel<La32>
    {
        public ushort L, A;
        public Rgba32 ToRgba32() => ToRgba64().ToRgba32();
        public void FromRgba32(Rgba32 s) => FromRgba64(s.ToRgba64());
        public Rgba64 ToRgba64() => new(L, L, L, A);
        public void FromRgba64(Rgba64 s) { L = PixelMath.Luminance16(s.R, s.G, s.B); A = s.A; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct A8 : IPixel<A8>
    {
        public byte PackedValue;
        public Rgba32 ToRgba32() => new(0, 0, 0, PackedValue);
        public void FromRgba32(Rgba32 s) => PackedValue = s.A;
        public Rgba64 ToRgba64() => new(0, 0, 0, PixelMath.To16Bit(PackedValue));
        public void FromRgba64(Rgba64 s) => PackedValue = PixelMath.To8Bit(s.A);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rgb48 : IPixel<Rgb48>
    {
        public ushort R, G, B;
        public Rgba32 ToRgba32() => ToRgba64().ToRgba32();
        public void FromRgba32(Rgba32 s) => FromRgba64(s.ToRgba64());
        public Rgba64 ToRgba64() => new(R, G, B, ushort.MaxValue);
        public void FromRgba64(Rgba64 s) { R = s.R; G = s.G; B = s.B; }

        // Same conversions as Color's ImageSharp operators (Color.cs).
        public static implicit operator Color(Rgb48 p) => new(255, (byte)(p.R >> 8), (byte)(p.G >> 8), (byte)(p.B >> 8));
        public static implicit operator Rgb48(Color c) { var r = new Rgb48(); r.FromRgba64(((Rgba32)c).ToRgba64()); return r; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rgba64 : IPixel<Rgba64>
    {
        public ushort R, G, B, A;
        public Rgba64(ushort r, ushort g, ushort b, ushort a) { R = r; G = g; B = b; A = a; }
        public Rgba32 ToRgba32() => new(PixelMath.To8Bit(R), PixelMath.To8Bit(G), PixelMath.To8Bit(B), PixelMath.To8Bit(A));
        public void FromRgba32(Rgba32 s) => this = s.ToRgba64();
        public Rgba64 ToRgba64() => this;
        public void FromRgba64(Rgba64 s) => this = s;

        // Same conversions as Color's ImageSharp operators (Color.cs): the 16-bit values are passed
        // to Color's (int, int, int, int) constructor, which truncates them to bytes.
        public static implicit operator Color(Rgba64 p) => new(p.A, p.R, p.G, p.B);
        public static implicit operator Rgba64(Color c) => ((Rgba32)c).ToRgba64();
    }
}
#endif
