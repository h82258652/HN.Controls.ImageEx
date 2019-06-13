using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HN.Controls
{
    public class ImageExAssist : ContentControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(ImageEx), typeof(ImageExAssist), new PropertyMetadata(default(ImageEx)));

        public ImageExAssist()
        {
            DefaultStyleKey = typeof(ImageExAssist);

            Loaded += ImageExAssist_Loaded;
            Unloaded += ImageExAssist_Unloaded;
        }

        public ImageEx Source
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
