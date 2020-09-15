using System.Windows;

namespace WpfDemo.Views
{
    public partial class CornerRadiusView
    {
        public CornerRadiusView()
        {
            InitializeComponent();

            UpdateCornerRadius();
        }

        private void BottomLeftSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateCornerRadius();
        }

        private void BottomRightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateCornerRadius();
        }

        private void TopLeftSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateCornerRadius();
        }

        private void TopRightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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
