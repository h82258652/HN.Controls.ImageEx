using System;
using System.Windows;
using System.Windows.Media.Animation;
using HN.Animation;
using SkiaSharp;

namespace HN.Models
{
    internal class MultiplyFrameImageExDisplaySource : IMultiplyFrameImageExDisplaySource
    {
        private readonly ObjectAnimationUsingKeyFrames _animation;
        private readonly AnimationBridge<SKBitmap> _animationFrameBridge;
        private readonly ImageExFrame[] _frames;
        private readonly Storyboard _storyboard;

        internal MultiplyFrameImageExDisplaySource(ImageExSource source, RepeatBehavior repeatBehavior)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _frames = source.Frames;
            if (_frames == null || _frames.Length <= 1)
            {
                throw new ArgumentException("This source is a single frame source.");
            }

            Width = source.Width;
            Height = source.Height;

            _animationFrameBridge = new AnimationBridge<SKBitmap>();
            _animationFrameBridge.ValueChanged += AnimationFrameBridge_ValueChanged;

            var storyboard = new Storyboard();
            var animation = new ObjectAnimationUsingKeyFrames();
            if (repeatBehavior == default)
            {
                var repetitionCount = source.RepetitionCount;
                if (repetitionCount == -1)
                {
                    animation.RepeatBehavior = RepeatBehavior.Forever;
                }
                else if (repetitionCount > 0)
                {
                    animation.RepeatBehavior = new RepeatBehavior(repetitionCount);
                }
            }
            else
            {
                animation.RepeatBehavior = repeatBehavior;
            }

            var frameCount = _frames.Length;

            var totalDuration = 0;
            for (var frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                var frame = _frames[frameIndex];
                var bitmap = frame.Bitmap;
                var duration = frame.Duration;

                animation.KeyFrames.Add(new DiscreteObjectKeyFrame
                {
                    Value = bitmap,
                    KeyTime = TimeSpan.FromMilliseconds(totalDuration)
                });

                totalDuration += duration;
            }

            animation.Duration = TimeSpan.FromMilliseconds(totalDuration);
            Storyboard.SetTarget(animation, _animationFrameBridge);
            Storyboard.SetTargetProperty(animation, new PropertyPath(nameof(_animationFrameBridge.Value)));
            storyboard.Children.Add(animation);

            _animation = animation;
            _storyboard = storyboard;
        }

        public event EventHandler<SKBitmap>? CurrentChanged;

        public SKBitmap Current => _animationFrameBridge.Value ?? _frames[0].Bitmap;

        public int CurrentFrame
        {
            get
            {
                var current = Current;
                for (var i = 0; i < _frames.Length; i++)
                {
                    if (current == _frames[i].Bitmap)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        public int FrameCount => _frames.Length;

        public int Height { get; }

        public bool IsDisposed { get; private set; }

        public double SpeedRatio
        {
            get => _animation.SpeedRatio;
            set => _animation.SpeedRatio = value;
        }

        public int Width { get; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(MultiplyFrameImageExDisplaySource));
            }

            IsDisposed = true;
            _animationFrameBridge.ValueChanged -= AnimationFrameBridge_ValueChanged;
            foreach (var frame in _frames)
            {
                frame.Bitmap.Dispose();
            }
        }

        public void GotoFrame(int index)
        {
            var offset = 0;
            var frameCount = _frames.Length;
            for (var i = 0; i < index && i < frameCount; i++)
            {
                offset += _frames[i].Duration;
            }

            _storyboard.Seek(TimeSpan.FromMilliseconds(offset), TimeSeekOrigin.BeginTime);
        }

        public void Pause()
        {
            _storyboard.Pause();
        }

        public void Play()
        {
            var storyboard = _storyboard;
            var clockState = ClockState.Stopped;
            try
            {
                clockState = storyboard.GetCurrentState();
            }
            catch (InvalidOperationException)
            {
            }
            if (clockState == ClockState.Stopped)
            {
                storyboard.Begin();
            }
            else
            {
                storyboard.Resume();
            }
        }

        private void AnimationFrameBridge_ValueChanged(object? sender, SKBitmap e)
        {
            CurrentChanged?.Invoke(this, e);
        }
    }
}
