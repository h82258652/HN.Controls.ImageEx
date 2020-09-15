using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace UwpDemo.Views
{
    public sealed partial class CornerRadiusView
    {
        public CornerRadiusView()
        {
            InitializeComponent();

            UpdateCornerRadius();
        }

        private void BottomLeftSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateCornerRadius();
        }

        private void BottomRightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateCornerRadius();
        }

        private void TopLeftSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateCornerRadius();
        }

        private void TopRightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateCornerRadius();
        }

        private void UpdateCornerRadius()
        {
            if (TopLeftSlider != null && TopRightSlider != null && BottomRightSlider != null && BottomLeftSlider != null)
            {
                ImageEx.CornerRadius = new CornerRadius(TopLeftSlider.Value, TopRightSlider.Value, BottomRightSlider.Value, BottomLeftSlider.Value);
            }
        }
    }
}
