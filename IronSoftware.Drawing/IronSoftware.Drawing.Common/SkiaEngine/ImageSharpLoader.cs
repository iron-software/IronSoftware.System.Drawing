#if NETFRAMEWORK
namespace IronSoftware.Drawing.SkiaEngine
{
    /// <summary>
    /// .NET Framework counterpart of the public IronSoftware.Drawing.ImageSharpLoader, so AnyBitmap keeps one
    /// code path. TIFF goes through BitMiracle.LibTiff and everything else through SkiaSharp, neither of which
    /// has ImageSharp 2.x's BigTIFF directory loop (CVE-2026-106116).
    /// </summary>
    internal static class ImageSharpLoader
    {
        public static Image Load(byte[] data) => Image.Load(data);

        public static Image<TPixel> Load<TPixel>(byte[] data) where TPixel : unmanaged, IPixel<TPixel> => Image.Load<TPixel>(data);
    }
}
#endif
