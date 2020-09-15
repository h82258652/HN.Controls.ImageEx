using Windows.UI.Xaml;

namespace UwpDemo.Views
{
    public sealed partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AnimationView));
        }

        private void CornerRadiusButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CornerRadiusView));
        }

        private void LazyLoadingButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LazyLoadingView));
        }

        private void MemoryUsageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MemoryUsageView));
        }

        private void ProgressButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProgressView));
        }

        private void ShadowButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShadowView));
        }
    }
}
