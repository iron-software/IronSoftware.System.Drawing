// See https://aka.ms/new-console-template for more information

using IronSoftware.Drawing;
using SkiaSharp;

//ReadOnlySpan<byte> bytes = File.ReadAllBytes("test.bmp");

//for (int i=0; i<10000; i++)
//{
//    using AnyBitmap bmp = new AnyBitmap(bytes);
//    var bin = bmp.GetBytes();
//    Console.WriteLine(bin.Length);
//}

AnyBitmap bmp = new AnyBitmap(100, 100);
SKBitmap skbmp = bmp;
Console.WriteLine("Done!");