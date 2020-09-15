using System;
using System.Windows;
using System.Windows.Media;

namespace HN.Controls
{
    /// <summary>
    /// <see cref="ImageEx" /> 阴影效果。
    /// </summary>
    public class ImageExShadow : DependencyObject
    {
        /// <summary>
        /// 标识 <see cref="BlurRadius" /> 依赖属性。
        /// </summary>
        /// <returns>
        ///<see cref="BlurRadius" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(ImageExShadow), new PropertyMetadata(5d, OnBlurRadiusChanged));

        /// <summary>
        /// 标识 <see cref="Color" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Color" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ImageExShadow), new PropertyMetadata(Colors.Black, OnColorChanged));

        /// <summary>
        /// 标识 <see cref="Direction" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Direction" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(nameof(Direction), typeof(double), typeof(ImageExShadow), new PropertyMetadata(270d, OnDirectionChanged));

        /// <summary>
        /// 标识 <see cref="OffsetX" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="OffsetX" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(ImageExShadow), new PropertyMetadata(0d, OnOffsetXChanged));
        
        /// <summary>
        /// 标识 <see cref="OffsetY" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="OffsetY" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(ImageExShadow), new PropertyMetadata(0d, OnOffsetYChanged));
      
        /// <summary>
        /// 标识 <see cref="Opacity" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Opacity" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(ImageExShadow), new PropertyMetadata(1d, OnOpacityChanged));
        
        /// <summary>
        /// 标识 <see cref="ShadowDepth" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="ShadowDepth" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty ShadowDepthProperty = DependencyProperty.Register(nameof(ShadowDepth), typeof(double), typeof(ImageExShadow), new PropertyMetadata(0d, OnShadowDepthChanged));

        private bool _isUpdatingProperty;

        /// <summary>
        /// 获取或设置一个值，该值指示阴影的模糊效果的半径。
        /// </summary>
        /// <returns>
        /// 阴影的模糊效果的半径。
        /// </returns>
        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        /// <summary>
        /// 获取或设置阴影的颜色。
        /// </summary>
        /// <returns>
        /// 阴影的颜色。
        /// </returns>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// 获取或设置阴影的方向。
        /// </summary>
        /// <returns>
        /// 阴影的方向。
        /// </returns>
        public double Direction
        {
            get => (double)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        /// <summary>
        /// 获取或设置阴影在 x 轴方向上的偏移。
        /// </summary>
        /// <returns>
        /// 阴影在 x 轴方向上的偏移。
        /// </returns>
        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        /// <summary>
        /// 获取或设置阴影在 y 轴方向上的偏移。
        /// </summary>
        /// <returns>
        /// 阴影在 y 轴方向上的偏移。
        /// </returns>
        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        /// <summary>
        /// 获取或设置阴影的不透明度。
        /// </summary>
        /// <returns>
        /// 阴影的不透明度。
        /// </returns>
        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        /// <summary>
        /// 获取或设置阴影距离中心的距离。
        /// </summary>
        /// <returns>
        /// 阴影距离中心的距离。
        /// </returns>
        public double ShadowDepth
        {
            get => (double)GetValue(ShadowDepthProperty);
            set => SetValue(ShadowDepthProperty, value);
        }

        internal ImageEx? Owner { get; set; }

        private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            obj.Owner?.UpdateShadowExpandBorder();

            obj.Owner?.InvalidateCanvas();
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            obj.Owner?.InvalidateCanvas();
        }

        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            if (obj._isUpdatingProperty)
            {
                return;
            }

            obj.UpdateOffsetByDepthAndDirection();

            obj.Owner?.UpdateShadowExpandBorder();

            obj.Owner?.InvalidateCanvas();
        }

        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            if (obj._isUpdatingProperty)
            {
                return;
            }

            obj.UpdateDepthAndDirectionByOffset();

            obj.Owner?.UpdateShadowExpandBorder();

            obj.Owner?.InvalidateCanvas();
        }

        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            if (obj._isUpdatingProperty)
            {
                return;
            }

            obj.UpdateDepthAndDirectionByOffset();

            obj.Owner?.UpdateShadowExpandBorder();

            obj.Owner?.InvalidateCanvas();
        }

        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            obj.Owner?.InvalidateCanvas();
        }

        private static void OnShadowDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageExShadow)d;

            if (obj._isUpdatingProperty)
            {
                return;
            }

            obj.UpdateOffsetByDepthAndDirection();

            obj.Owner?.UpdateShadowExpandBorder();

            obj.Owner?.InvalidateCanvas();
        }

        private void UpdateDepthAndDirectionByOffset()
        {
            if (_isUpdatingProperty)
            {
                return;
            }

            try
            {
                _isUpdatingProperty = true;

                var offsetX = OffsetX;
                var offsetY = OffsetY;
                var depth = Math.Sqrt(offsetX * offsetX + offsetY * offsetY);
                var direction = Math.Atan2(offsetY, offsetX) * 180 / Math.PI;

                SetCurrentValue(ShadowDepthProperty, depth);
                SetCurrentValue(DirectionProperty, direction);
            }
            finally
            {
                _isUpdatingProperty = false;
            }
        }

        private void UpdateOffsetByDepthAndDirection()
        {
            if (_isUpdatingProperty)
            {
                return;
            }

            try
            {
                _isUpdatingProperty = true;

                var depth = ShadowDepth;
                var direction = Direction;
                var directionRad = direction * Math.PI / 180;
                var offsetX = depth * Math.Cos(directionRad);
                var offsetY = depth * Math.Sin(directionRad);

                SetCurrentValue(OffsetXProperty, offsetX);
                SetCurrentValue(OffsetYProperty, offsetY);
            }
            finally
            {
                _isUpdatingProperty = false;
            }
        }
    }
}
