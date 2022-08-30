# IronDrawing
![Nuget](https://img.shields.io/nuget/v/IronDrawing?color=informational&label=latest)  ![Installs](https://img.shields.io/nuget/dt/IronDrawing?color=informational&label=installs&logo=nuget)  ![Passed](https://img.shields.io/badge/build-%20%E2%9C%93%20258%20tests%20passed%20(0%20failed)%20-107C10?logo=visualstudio)  ![windows](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=windows)  ![macOS](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=apple)  ![linux](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=linux&logoColor=white)  ![docker](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=docker&logoColor=white)  ![aws](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=amazonaws)  ![microsoftazure](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=microsoftazure)  ![livechat](https://img.shields.io/badge/Live%20Chat-Active-purple?logo=googlechat&logoColor=white)

# IronDrawing - Image, Color, Rectangle, and Font class for .NET Applications

IronDrawing is a library developed and maintained by Iron Software that helps C# Software Engineers to replace System.Drawing.Common in .NET projects.
 

### IronDrawing Features and Capabilities:
- AnyBitmap: A universally compatible Bitmap class. Implicit casting between System.Drawing.Bitmap, System.Drawing.Image, SkiaSharp.SKBitmap, SkiaSharp.SKImage, SixLabors.ImageSharp, Microsoft.Maui.Graphics.Platform.PlatformImage to IronDrawing.AnyBitmap
- Color: A universally compatible Color class. Implicit casting between System.Drawing.Color, SkiaSharp.SKColor, SixLabors.ImageSharp.Color, SixLabors.ImageSharp.PixelFormats to IronDrawing.Color
- CropRectangle: A universally compatible Rectangle class. Implicit casting between System.Drawing.Rectangle, SkiaSharp.SKRect, SkiaSharp.SKRectI, SixLabors.ImageSharp.Rectangle to IronDrawing.CropRectangle
- Font: A universally compatible Font class. Implicit casting between System.Drawing.Font, SkiaSharp.SKFont, SixLabors.Fonts.Font to IronDrawing.Font

### IronDrawing has cross platform support compatibility with:
- .NET7, .NET 6, .NET 5, .NET Core, Standard, and Framework
- Windows, macOS, Linux, Docker, Azure, and AWS

## Using IronDrawing

Installing the IronDrawing NuGet package is quick and easy, please install the package like this:
```
PM> Install-Package IronDrawing
```
Once installed, you can get started by adding `using IronDrawing` to the top of your C# code. Here is is sample HTML to PDF example to get started:
```
// IronDrawing.AnyBitmap
using IronDrawing;

// Created new AnyBitmap object
var bitmap = AnyBitmap.FromFile("FILE_PATH");
bitmap.SaveAs("result.jpg");

var bytes = bitmap.ExportBytes();

var resultExport = new System.IO.MemoryStream();
bimtap.ExportStream(resultExport, AnyBitmap.ImageFormat.Jpeg, 100);

// Casting between System.Drawing.Bitmap to IronDrawing.AnyBitmap
System.Drawing.Bitmap image = new new System.Drawing.Bitmap("FILE_PATH");
var anyBitmap = image;
anyBitmap.SaveAs("result-from-casting.png");

```
## Licensing & Support Available
For more information on Iron Software please visit: [https://ironsoftware.com/](https://ironsoftware.com/)

For more support and inquiries, please email us at: developers@ironsoftware.com