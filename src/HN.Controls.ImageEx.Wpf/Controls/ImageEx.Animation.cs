using System;
using System.Windows;
using System.Windows.Media.Animation;
using HN.Models;

namespace HN.Controls
{
    public partial class ImageEx
    {
        /// <summary>
        /// 标识 <see cref="AutoStart" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="AutoStart" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(ImageEx), new PropertyMetadata(true));

        /// <summary>
        /// 标识 <see cref="FadeInDuration" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="FadeInDuration" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty FadeInDurationProperty = DependencyProperty.Register(nameof(FadeInDuration), typeof(TimeSpan), typeof(ImageEx), new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// 标识 <see cref="RepeatBehavior" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="RepeatBehavior" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.Register(nameof(RepeatBehavior), typeof(RepeatBehavior), typeof(ImageEx), new PropertyMetadata(default(RepeatBehavior)));

        /// <summary>
        /// 标识 <see cref="SpeedRatio" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="SpeedRatio" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty SpeedRatioProperty = DependencyProperty.Register(nameof(SpeedRatio), typeof(double), typeof(ImageEx), new PropertyMetadata(1d, OnSpeedRatioChanged));

        /// <summary>
        /// 获取或设置一个值，指示在成功加载动画图像源时，是否立刻播放。
        /// </summary>
        /// <returns>
        /// 默认值为 <see langword="true" />。
        /// </returns>
        public bool AutoStart
        {
            get => (bool)GetValue(AutoStartProperty);
            set => SetValue(AutoStartProperty, value);
        }

        /// <summary>
        /// 获取当前显示的是第几帧。
        /// </summary>
        /// <returns>
        /// 当前显示的是第几帧。如果图像源没加载，则返回 -1。
        /// </returns>
        public int CurrentFrame
        {
            get
            {
                if (_displaySource == null)
                {
                    return -1;
                }

                if (_displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
                {
                    return multiplyFrameDisplaySource.CurrentFrame;
                }

                return 0;
            }
        }

        /// <summary>
        /// 获取或设置成功加载图像时，淡入的时长。
        /// </summary>
        /// <returns>
        /// 成功加载图像时，淡入的时长。
        /// </returns>
        public TimeSpan FadeInDuration
        {
            get => (TimeSpan)GetValue(FadeInDurationProperty);
            set => SetValue(FadeInDurationProperty, value);
        }

        /// <summary>
        /// 获取当前图像源总共有多少帧。
        /// </summary>
        /// <returns>
        /// 当前图像源总共有多少帧。如果图像源没加载，则返回 0。
        /// </returns>
        public int FrameCount
        {
            get
            {
                if (_displaySource == null)
                {
                    return 0;
                }

                if (_displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
                {
                    return multiplyFrameDisplaySource.FrameCount;
                }

                return 1;
            }
        }

        /// <summary>
        /// 获取或设置动画的重复行为。如果设置为默认值，则使用图像源中的元数据。仅在成功加载图像源前设置有效。
        /// </summary>
        /// <returns>
        /// 动画的重复行为。
        /// </returns>
        public RepeatBehavior RepeatBehavior
        {
            get => (RepeatBehavior)GetValue(RepeatBehaviorProperty);
            set => SetValue(RepeatBehaviorProperty, value);
        }

        /// <summary>
        /// 获取或设置动画的播放速率。
        /// </summary>
        /// <returns>
        /// 动画的播放速率。
        /// 默认值为 1。
        /// </returns>
        public double SpeedRatio
        {
            get => (double)GetValue(SpeedRatioProperty);
            set => SetValue(SpeedRatioProperty, value);
        }

        /// <summary>
        /// 跳转到指定帧。
        /// </summary>
        /// <param name="index">需要跳转的帧数。</param>
        public void GotoFrame(int index)
        {
            if (_displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
            {
                multiplyFrameDisplaySource.GotoFrame(index);
            }
        }

        /// <summary>
        /// 暂停动画。
        /// </summary>
        public void Pause()
        {
            if (_displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
            {
                multiplyFrameDisplaySource.Pause();
            }
        }

        /// <summary>
        /// 开始或恢复动画。
        /// </summary>
        public void Play()
        {
            if (_displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
            {
                multiplyFrameDisplaySource.Play();
            }
        }
        
        private static void OnSpeedRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = (double)e.NewValue;

            if (obj._displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
            {
                multiplyFrameDisplaySource.SpeedRatio = value;
            }
        }

        private void PlayFadeInAnimation()
        {
            if (_loadingContentHost == null ||
                _canvas == null)
            {
                return;
            }

            var fadeInDuration = FadeInDuration;
            if (fadeInDuration <= TimeSpan.Zero)
            {
                return;
            }

            var storyboard = new Storyboard();
            {
                var animation = new ObjectAnimationUsingKeyFrames();
                animation.KeyFrames.Add(new DiscreteObjectKeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(0),
                    Value = Visibility.Visible
                });
                animation.KeyFrames.Add(new DiscreteObjectKeyFrame
                {
                    KeyTime = fadeInDuration,
                    Value = Visibility.Collapsed
                });
                Storyboard.SetTarget(animation, _loadingContentHost);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Visibility"));
                storyboard.Children.Add(animation);
            }
            {
                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = fadeInDuration,
                    EasingFunction = new CubicEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    }
                };
                Storyboard.SetTarget(animation, _canvas);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
                storyboard.Children.Add(animation);
            }
            {
                var animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = fadeInDuration,
                    EasingFunction = new CubicEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    }
                };
                Storyboard.SetTarget(animation, _loadingContentHost);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
                storyboard.Children.Add(animation);
            }
            storyboard.Begin();
        }
    }
}
