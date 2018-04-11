using System;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HN.Controls
{
    public class ImageEx : Control
    {
        public static readonly DependencyProperty LoadingTemplateProperty = DependencyProperty.Register(nameof(LoadingTemplate), typeof(DataTemplate), typeof(ImageEx), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register(nameof(NineGrid), typeof(Thickness), typeof(ImageEx), new PropertyMetadata(default(Thickness)));
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ImageEx), new PropertyMetadata(default(object), OnSourceChanged));

        private Image _image;

        public ImageEx()
        {
            DefaultStyleKey = typeof(ImageEx);
        }

        public DataTemplate LoadingTemplate
        {
            get => (DataTemplate)GetValue(LoadingTemplateProperty);
            set => SetValue(LoadingTemplateProperty, value);
        }

        public Thickness NineGrid
        {
            get => (Thickness)GetValue(NineGridProperty);
            set => SetValue(NineGridProperty, value);
        }

        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public CompositionBrush GetAlphaMask()
        {
            ApplyTemplate();
            return _image?.GetAlphaMask();
        }

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = e.NewValue;

            await obj.SetSource(value);
        }

        private async Task SetSource(object source)
        {
            throw new NotImplementedException();
        }
    }
}