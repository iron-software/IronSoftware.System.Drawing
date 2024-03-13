// See https://aka.ms/new-console-template for more information

using IronSoftware.Drawing;

ReadOnlySpan<byte> bytes = File.ReadAllBytes("test.bmp");

for (int i=0; i<10000; i++)
{
    using AnyBitmap bmp = new AnyBitmap(bytes);
    var bin = bmp.GetBytes();
    Console.WriteLine(bin.Length);
}