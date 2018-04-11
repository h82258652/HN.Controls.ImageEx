using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace HN.Media
{
    public class ImageBrushEx : XamlCompositionBrushBase
    {
        public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.Register(nameof(AlignmentX), typeof(AlignmentX), typeof(ImageBrushEx), new PropertyMetadata(AlignmentX.Center, OnAlignmentXChanged));
        public static readonly DependencyProperty AlignmentYProperty = DependencyProperty.Register(nameof(AlignmentY), typeof(AlignmentY), typeof(ImageBrushEx), new PropertyMetadata(AlignmentY.Center, OnAlignmentYChanged));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(object), typeof(ImageBrushEx), new PropertyMetadata(default(object), OnImageSourceChanged));
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageBrushEx), new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

        public AlignmentX AlignmentX
        {
            get => (AlignmentX)GetValue(AlignmentXProperty);
            set => SetValue(AlignmentXProperty, value);
        }

        public AlignmentY AlignmentY
        {
            get => (AlignmentY)GetValue(AlignmentYProperty);
            set => SetValue(AlignmentYProperty, value);
        }

        public object ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        protected override void OnConnected()
        {
            base.OnConnected();
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
        }

        private static void OnAlignmentXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushEx)d;
            var value = (AlignmentX)e.NewValue;

            if (!Enum.IsDefined(typeof(AlignmentX), value))
            {
                throw new ArgumentOutOfRangeException(nameof(AlignmentX));
            }

            var brush = (CompositionSurfaceBrush)obj.CompositionBrush;
            brush.HorizontalAlignmentRatio = (float)value * 0.5f;
        }

        private static void OnAlignmentYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushEx)d;
            var value = (Stretch)e.NewValue;

            if (!Enum.IsDefined(typeof(Stretch), value))
            {
                throw new ArgumentOutOfRangeException(nameof(Stretch));
            }

            var brush = (CompositionSurfaceBrush)obj.CompositionBrush;
            brush.Stretch = (CompositionStretch)value;
        }
    }
}