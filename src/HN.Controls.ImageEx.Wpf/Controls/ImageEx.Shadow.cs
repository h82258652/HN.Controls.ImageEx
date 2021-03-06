﻿using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace HN.Controls
{
    [TemplatePart(Name = ShadowExpandBorderTemplateName, Type = typeof(Border))]
    public partial class ImageEx
    {
        /// <summary>
        /// 标识 <see cref="DropShadow" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="DropShadow" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty DropShadowProperty = DependencyProperty.Register(nameof(DropShadow), typeof(ImageExShadow), typeof(ImageEx), new PropertyMetadata(default(ImageExShadow), OnShadowChanged));

        private const string ShadowExpandBorderTemplateName = "PART_ShadowExpandBorder";
        private Border? _shadowExpandBorder;
        private double _shadowXExpand;
        private double _shadowYExpand;

        /// <summary>
        /// 获取或设置阴影。
        /// </summary>
        /// <returns>
        /// 阴影。
        /// </returns>
        [CanBeNull]
        public ImageExShadow? DropShadow
        {
            get => (ImageExShadow?)GetValue(DropShadowProperty);
            set => SetValue(DropShadowProperty, value);
        }

        internal void UpdateShadowExpandBorder()
        {
            if (_shadowExpandBorder == null)
            {
                return;
            }

            var shadow = DropShadow;
            if (shadow == null)
            {
                _shadowXExpand = 0;
                _shadowYExpand = 0;
                _shadowExpandBorder.Margin = new Thickness();
                return;
            }

            var offsetX = shadow.OffsetX;
            var offsetY = shadow.OffsetY;
            var blurRadius = shadow.BlurRadius;

            var xExpand = Math.Abs(offsetX) + blurRadius + 4;
            var yExpand = Math.Abs(offsetY) + blurRadius + 4;

            _shadowXExpand = xExpand;
            _shadowYExpand = yExpand;

            // if use shadow, at least expand 20 px.
            xExpand = Math.Max(xExpand, 20);
            yExpand = Math.Max(yExpand, 20);

            var margin = new Thickness(-xExpand, -yExpand, -xExpand, -yExpand);
            _shadowExpandBorder.Margin = margin;
        }

        private static void OnShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var oldValue = (ImageExShadow?)e.OldValue;
            var newValue = (ImageExShadow?)e.NewValue;

            if (oldValue != null)
            {
                oldValue.Owner = null;
            }

            if (newValue != null)
            {
                newValue.Owner = obj;
            }

            obj.UpdateShadowExpandBorder();

            obj.InvalidateCanvas();
        }

        private void SetPaintShadow(SKPaint paint)
        {
            var shadow = DropShadow;
            if (shadow == null)
            {
                return;
            }

            var shadowColor = shadow.Color.ToSKColor();
            var shadowOpacity = Math.Max(0, Math.Min(shadow.Opacity, 1));
            shadowColor = shadowColor.WithAlpha((byte)(shadowColor.Alpha * shadowOpacity));

            paint.ImageFilter = SKImageFilter.CreateDropShadow(
                (float)shadow.OffsetX,
                (float)shadow.OffsetY,
                (float)shadow.BlurRadius,
                (float)shadow.BlurRadius,
                shadowColor);
        }
    }
}
