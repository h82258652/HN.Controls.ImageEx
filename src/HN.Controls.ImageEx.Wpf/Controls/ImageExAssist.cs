using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HN.Controls
{
    public class ImageExAssist : ContentControl
    {
        public static readonly DependencyProperty SourceProperty;
        private static readonly DependencyPropertyKey SourcePropertyKey = DependencyProperty.RegisterReadOnly(nameof(Source), typeof(ImageEx), typeof(ImageExAssist), new PropertyMetadata(default(ImageEx)));

        static ImageExAssist()
        {
            SourceProperty = SourcePropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageExAssist), new FrameworkPropertyMetadata(typeof(ImageExAssist)));
        }

        public ImageExAssist()
        {
            Loaded += ImageExAssist_Loaded;
            Unloaded += ImageExAssist_Unloaded;
        }

        public ImageEx Source
        {
            get => (ImageEx)GetValue(SourceProperty);
            private set => SetValue(SourcePropertyKey, value);
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
