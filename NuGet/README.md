[![NuGet](https://img.shields.io/nuget/v/IronSoftware.System.Drawing?color=informational&label=latest&logo=nuget)](https://www.nuget.org/packages/IronSoftware.System.Drawing/) [![Installs](https://img.shields.io/nuget/dt/IronSoftware.System.Drawing?color=informational&label=installs&logo=nuget)](https://www.nuget.org/packages/IronSoftware.System.Drawing/) [![GitHub Latest Commit](https://img.shields.io/github/last-commit/iron-software/IronSoftware.Drawing.Common?color=informational&logo=github)](https://github.com/iron-software/IronSoftware.Drawing.Common) [![GitHub Contributors](https://img.shields.io/github/contributors/iron-software/IronSoftware.Drawing.Common?color=informational&logo=github)](https://github.com/iron-software/IronSoftware.Drawing.Common) [![GitHub Issue Shield](https://img.shields.io/github/issues/iron-software/IronSoftware.System.Drawing?logo=GitHub&style=flat-square)](https://github.com/iron-software/IronSoftware.System.Drawing/issues)

# IronSoftware.Drawing - Image, Color, Rectangle, Font, Point, and Size classes for .NET Applications

**IronSoftware.Drawing** is an free and open-source library originally developed by Iron Software that replaces System.Drawing.Common in .NET projects.

If you would like to contribute to this open-source project, please visit the public GitHub and open a branch [here](https://github.com/iron-software/IronSoftware.System.Drawing/).

## Cross platform support compatibility with:
- .NET 8, .NET 7, .NET 6, .NET 5, .NET Core, Standard, and Framework
- Windows, macOS, Linux, Docker, Azure, and AWS

## IronSoftware.Drawing Features:
- **AnyBitmap**: A universally compatible Bitmap class. Implicit casting between `IronSoftware.Drawing.AnyBitmap` and the following supported:
  - `System.Drawing.Bitmap`
  - `System.Drawing.Image`
  - `SkiaSharp.SKBitmap`
  - `SkiaSharp.SKImage`
  - `SixLabors.ImageSharp`
  - `Microsoft.Maui.Graphics.Platform.PlatformImage`
- **Color**: A universally compatible Color class. Implicit casting between `IronSoftware.Drawing.Color` and the following supported:
  - `System.Drawing.Color`
  - `SkiaSharp.SKColor`
  - `SixLabors.ImageSharp.Color`
  - `SixLabors.ImageSharp.PixelFormats`
- **Rectangle** and **RectangleF**: A universally compatible Rectangle class. Implicit casting between `IronSoftware.Drawing.Rectangle`and `IronSoftware.Drawing.RectangleF` and the following supported:
  - `System.Drawing.Rectangle`
  - `System.Drawing.RectangleF`
  - `SkiaSharp.SKRect`
  - `SkiaSharp.SKRectI`
  - `SixLabors.ImageSharp.Rectangle`
  - `SixLabors.ImageSharp.RectangleF`
- **Size** and **SizeF**: A universally compatible Size class. Implicit casting between `IronSoftware.Drawing.Size` and `IronSoftware.Drawing.SizeF` and the following supported:
  - `System.Drawing.Size`
  - `System.Drawing.SizeF`
  - `SkiaSharp.SKSize`
  - `SkiaSharp.SKSizeI`
  - `SixLabors.ImageSharp.Size`
  - `SixLabors.ImageSharp.SizeF`
  - `Microsoft.Maui.Graphics.Size`
  - `Microsoft.Maui.Graphics.SizeF`
- **Font**: A universally compatible Font class. Implicit casting between `IronSoftware.Drawing.Font` and the following supported:
  - `System.Drawing.Font`
  - `SkiaSharp.SKFont`
  - `SixLabors.Fonts.Font`
  - `IronPdf.Font.FontTypes`
- **Point** and **PointF**: Universally compatible Point classes. Implicit casting between `IronSoftware.Drawing.Point` and `IronSoftware.Drawing.PointF` and the following supported:
  - `System.Drawing.Point`
  - `SixLabors.ImageSharp.Point`
  - `SixLabors.ImageSharp.PointF`
  - `Microsoft.Maui.Graphics.Point`
  - `Microsoft.Maui.Graphics.PointF`
  - `SkiaSharp.SKPoint`
  - `SkiaSharp.SKPointI`

### Code Samples
A full list of code examples can be found on our [Get Started](https://github.com/iron-software/IronSoftware.System.Drawing#using-ironsoftwaredrawing) on GitHub.

## Support Available

To report an issue with IronSoftware.System.Drawing please raise them on the [GitHub Issues Page](https://github.com/iron-software/IronSoftware.System.Drawing/issues).

For more information about Iron Software please visit our website: [https://ironsoftware.com/](https://ironsoftware.com/)

For general support and technical inquiries, please email us at: support@ironsoftware.com
