using System;
using SkiaSharp;

namespace HN.Models
{
    internal interface IMultiplyFrameImageExDisplaySource : IImageExDisplaySource
    {
        event EventHandler<SKBitmap>? CurrentChanged;

        int CurrentFrame { get; }

        int FrameCount { get; }

        double SpeedRatio { get; set; }

        void GotoFrame(int index);

        void Pause();

        void Play();
    }
}
