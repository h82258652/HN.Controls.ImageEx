using System;
using SkiaSharp;

namespace HN.Models
{
    internal class SingleFrameImageExDisplaySource : ISingleFrameImageExDisplaySource
    {
        internal SingleFrameImageExDisplaySource(SKBitmap bitmap, int width, int height)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            Current = bitmap;
            Width = width;
            Height = height;
        }

        public SKBitmap Current { get; }
        
        public int Height { get; }

        public bool IsDisposed { get; private set; }
        
        public int Width { get; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(SingleFrameImageExDisplaySource));
            }

            IsDisposed = true;
            Current.Dispose();
        }
    }
}
