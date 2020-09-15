using System;
using Windows.UI.Xaml;

namespace HN.Animation
{
    internal class AnimationBridge<T> : DependencyObject
    {
        internal static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(T), typeof(AnimationBridge<T>), new PropertyMetadata(default(T), OnValueChanged));
        private readonly Action<T>? _callback;

        internal AnimationBridge()
        {
        }

        internal AnimationBridge(Action<T>? callback)
        {
            _callback = callback;
        }

        internal event EventHandler<T>? ValueChanged;

        internal T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (AnimationBridge<T>)d;
            var value = (T)e.NewValue;

            obj._callback?.Invoke(value);
            obj.ValueChanged?.Invoke(obj, value);
        }
    }
}
