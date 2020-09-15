using System.Windows;
using SkiaSharp;

namespace HN.Controls
{
    public partial class ImageEx
    {
        /// <summary>
        /// 标识 <see cref="NineGrid" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="NineGrid" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register(nameof(NineGrid), typeof(Thickness), typeof(ImageEx), new PropertyMetadata(default(Thickness), OnNineGridChanged));

        /// <summary>
        /// 获取或设置控制图像小大调整方式的九格形式的值。九网格形式使你可以将图像的边缘和角拉伸成与其中心不同的形状。
        /// </summary>
        /// <returns>
        /// 为九网格大小调整比喻设置 **Left**、**Top**、**Right**、**Bottom** 量化指标的 Thickness 值。
        /// </returns>
        public Thickness NineGrid
        {
            get => (Thickness)GetValue(NineGridProperty);
            set => SetValue(NineGridProperty, value);
        }

        private static void OnNineGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;

            obj.InvalidateCanvas();
        }

        private void DrawBitmapWithNineGrid(SKCanvas canvas, SKBitmap bitmap, SKRect dest, SKPaint? paint = null)
        {
            var nineGrid = NineGrid;
            var center = new SKRectI((int)nineGrid.Left, (int)nineGrid.Top, (int)nineGrid.Right, (int)nineGrid.Bottom);
            canvas.DrawBitmapNinePatch(bitmap, center, dest, paint);
        }
    }
}
