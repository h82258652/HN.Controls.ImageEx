using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HN.Controls
{
    public partial class ImageEx
    {
        /// <summary>
        /// 标识 <see cref="LazyLoadingEnabled" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="LazyLoadingEnabled" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty LazyLoadingEnabledProperty = DependencyProperty.Register(nameof(LazyLoadingEnabled), typeof(bool), typeof(ImageEx), new PropertyMetadata(default(bool), OnLazyLoadingEnabledChanged));

        /// <summary>
        /// 标识 <see cref="LazyLoadingThreshold" /> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty LazyLoadingThresholdProperty = DependencyProperty.Register(nameof(LazyLoadingThreshold), typeof(double), typeof(ImageEx), new PropertyMetadata(default(double), OnLazyLoadingThresholdChanged));

        private bool _isInViewport;

        /// <summary>
        /// 获取或设置是否启用延迟加载。
        /// </summary>
        /// <returns>
        /// 是否启用延迟加载。
        /// </returns>
        public bool LazyLoadingEnabled
        {
            get => (bool)GetValue(LazyLoadingEnabledProperty);
            set => SetValue(LazyLoadingEnabledProperty, value);
        }

        /// <summary>
        /// 获取或设置开启延迟加载时，距离可视区域的距离。
        /// </summary>
        /// <returns>
        ///开启延迟加载时，距离可视区域的距离。
        /// </returns>
        public double LazyLoadingThreshold
        {
            get => (double)GetValue(LazyLoadingThresholdProperty);
            set => SetValue(LazyLoadingThresholdProperty, value);
        }

        private static void OnLazyLoadingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;

            obj.InvalidateLazyLoading();
        }

        private static void OnLazyLoadingThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = (double)e.NewValue;

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(LazyLoadingThreshold));
            }

            obj.InvalidateLazyLoading();
        }

        private FrameworkElement? GetHostElement()
        {
            FrameworkElement hostElement = this;
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(hostElement) as FrameworkElement;
                if (parent == null)
                {
                    break;
                }

                if (parent is ScrollViewer)
                {
                    return parent;
                }

                hostElement = parent;
            }

            if (ReferenceEquals(hostElement, this))
            {
                return null;
            }

            return hostElement;
        }

        private async void InvalidateLazyLoading()
        {
            if (!IsLoaded)
            {
                _isInViewport = false;
                return;
            }

            var hostElement = GetHostElement();
            if (hostElement == null)
            {
                _isInViewport = false;
                return;
            }

            var controlRect = TransformToVisual(hostElement)
                .TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
            var lazyLoadingThreshold = LazyLoadingThreshold;
            var hostRect = new Rect(
                0 - lazyLoadingThreshold,
                0 - lazyLoadingThreshold,
                hostElement.ActualWidth + 2 * lazyLoadingThreshold,
                hostElement.ActualHeight + 2 * lazyLoadingThreshold);

            if (controlRect.IntersectsWith(hostRect))
            {
                _isInViewport = true;

                if (_lazyLoadingSource != null)
                {
                    var source = _lazyLoadingSource;
                    _lazyLoadingSource = null;
                    await SetSourceAsync(source);
                }
            }
            else
            {
                _isInViewport = false;
            }
        }
    }
}
