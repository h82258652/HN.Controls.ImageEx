using System;
using SkiaSharp;

namespace HN.Models
{
    internal interface IImageExDisplaySource : IDisposable
    {
        SKBitmap Current { get; }

        int Height { get; }

        bool IsDisposed { get; }
        
        int Width { get; }
    }
}
