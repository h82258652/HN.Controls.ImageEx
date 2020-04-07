using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace HN.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="ImageEx" /> 助手类。
    /// </summary>
    public class ImageExAssist : ContentControl
    {
        /// <summary>
        /// 标识 <see cref="Source" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Source" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(ImageEx), typeof(ImageExAssist), new PropertyMetadata(default(ImageEx)));

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageExAssist" /> 类的新实例。
        /// </summary>
        public ImageExAssist()
        {
            DefaultStyleKey = typeof(ImageExAssist);

            Loaded += ImageExAssist_Loaded;
            Unloaded += ImageExAssist_Unloaded;
        }

        /// <summary>
        /// 获取上级的 <see cref="ImageEx" /> 对象。
        /// </summary>
        /// <returns>
        /// 上级的 <see cref="ImageEx" /> 对象。
        /// </returns>
        [CanBeNull]
        public ImageEx? Source
        {
            get => (ImageEx)GetValue(SourceProperty);
            private set => SetValue(SourceProperty, value);
        }

        private void ImageExAssist_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = this;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent is ImageEx imageEx)
                {
                    Source = imageEx;
                    return;
                }
            }
            while (parent != null);

            Source = null;
        }

        private void ImageExAssist_Unloaded(object sender, RoutedEventArgs e)
        {
            Source = null;
        }
    }
}
