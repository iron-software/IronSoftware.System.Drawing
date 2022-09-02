![Nuget](https://img.shields.io/nuget/v/IronDrawing?color=informational&label=latest)  ![Installs](https://img.shields.io/nuget/dt/IronDrawing?color=informational&label=installs&logo=nuget)  ![Passed](https://img.shields.io/badge/build-%20%E2%9C%93%20258%20tests%20passed%20(0%20failed)%20-107C10?logo=visualstudio)  ![windows](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=windows)  ![macOS](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=apple)  ![linux](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=linux&logoColor=white)  ![docker](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=docker&logoColor=white)  ![aws](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=amazonaws)  ![microsoftazure](https://img.shields.io/badge/%E2%80%8E%20-%20%E2%9C%93-107C10?logo=microsoftazure)
​
# IronDrawing - Image, Color, Rectangle, and Font class for .NET Applications
​
IronDrawing is a library developed and maintained by Iron Software that helps C# Software Engineers to replace System.Drawing.Common in .NET projects.
​
### IronDrawing Features and Capabilities:
- AnyBitmap: A universally compatible Bitmap class. Implicit casting between `IronDrawing.AnyBitmap` and the following:
  - `System.Drawing.Bitmap`
  - `System.Drawing.Image`
  - `SkiaSharp.SKBitmap`
  - `SkiaSharp.SKImage`
  - `SixLabors.ImageSharp`
  - `Microsoft.Maui.Graphics.Platform.PlatformImage`
- Color: A universally compatible Color class. Implicit casting between `IronDrawing.Color` and the following:
  - `System.Drawing.Color`
  - `SkiaSharp.SKColor`
  - `SixLabors.ImageSharp.Color`
  - `SixLabors.ImageSharp.PixelFormats`
- CropRectangle: A universally compatible Rectangle class. Implicit casting between `IronDrawing.CropRectangle` and the following:
  - `System.Drawing.Rectangle`
  - `SkiaSharp.SKRect`
  - `SkiaSharp.SKRectI`
  - `SixLabors.ImageSharp.Rectangle`
- Font: A universally compatible Font class. Implicit casting between `IronDrawing.Font` and the following:
  - `System.Drawing.Font`
  - `SkiaSharp.SKFont`
  - `SixLabors.Fonts.Font`
​
### IronDrawing has cross platform support compatibility with:
- .NET 7, .NET 6, .NET 5, .NET Core, Standard, and Framework
- Windows, macOS, Linux, Docker, Azure, and AWS
​
## Using IronDrawing
​
Installing the IronDrawing NuGet package is quick and easy, please install the package like this:
```
PM> Install-Package IronDrawing
```
Once installed, you can get started by adding `using IronDrawing` to the top of your C# code.
### `AnyBitmap` Code Example
```csharp
using IronDrawing;
​
// Create a new AnyBitmap object
var bitmap = AnyBitmap.FromFile("FILE_PATH");
bitmap.SaveAs("result.jpg");
​
var bytes = bitmap.ExportBytes();
​
var resultExport = new System.IO.MemoryStream();
bimtap.ExportStream(resultExport, AnyBitmap.ImageFormat.Jpeg, 100);
​
// Casting between System.Drawing.Bitmap to IronDrawing.AnyBitmap
System.Drawing.Bitmap image = new System.Drawing.Bitmap("FILE_PATH");
var anyBitmap = image;
anyBitmap.SaveAs("result-from-casting.png");
```
### `Color` Code Example
```csharp
using IronDrawing;
​
// Create a new Color object
Color fromHex = new Color("#191919");
Color fromRgb = new Color(255, 255, 0);
Color fromEnum = Color.Crimson;
​
// Casting between System.Drawing.Color to IronDrawing.Color
System.Drawing.Color drawingColor = System.Drawing.Color.Red;
IronDrawing.Color ironDrawingColor = drawingColor;
​
ironDrawingColor.A;
ironDrawingColor.R;
ironDrawingColor.G;
ironDrawingColor.B;
​
// Luminance is a value from 0 (black) to 100 (white) where 50 is the perceptual "middle grey"
ironDrawingColor.GetLuminance();
```
### `CropRectangle` Code Example
```csharp
using IronDrawing;
​
// Create a new CropRectangle object
CropRectangle cropRectangle = new CropRectangle(5, 5, 50, 50);
​
// Casting between System.Drawing.Rectangle to IronDrawing.CropRectangle
System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(10, 10, 150, 150);
CropRectangle ironDrawingCropRectangle = rectangle;
​
ironDrawingCropRectangle.Width;
ironDrawingCropRectangle.Height;
ironDrawingCropRectangle.X;
ironDrawingCropRectangle.Y;
```
### `Font` Code Example
```csharp
using IronDrawing;
​
// Create a new Font object
Font font = new Font("Times New Roman", FontStyle.Italic | FontStyle.Bold, 30);
​
// Casting between System.Drawing.Font to IronDrawing.Font
System.Drawing.Font drawingFont = new System.Drawing.Font("Courier New", 30);
IronDrawing.Font ironDrawingFont = drawingFont;
​
ironDrawingFont.FamilyName;
ironDrawingFont.Style;
ironDrawingFont.Size;
ironDrawingFont.Italic;
ironDrawingFont.Bold;
```
​
## Support Available
​
For more information on Iron Software please visit: [https://ironsoftware.com/](https://ironsoftware.com/)
​
​
For general support and technical inquiries, please email us at: developers@ironsoftware.com