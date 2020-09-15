using System.Windows;

namespace WpfDemo.Views
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            new AnimationView().ShowDialog();
        }

        private void CornerRadiusButton_Click(object sender, RoutedEventArgs e)
        {
            new CornerRadiusView().ShowDialog();
        }

        private void LazyLoadingButton_Click(object sender, RoutedEventArgs e)
        {
            new LazyLoadingView().ShowDialog();
        }

        private void MemoryUsageButton_Click(object sender, RoutedEventArgs e)
        {
            new MemoryUsageView().ShowDialog();
        }

        private void ProgressButton_Click(object sender, RoutedEventArgs e)
        {
            new ProgressView().ShowDialog();
        }

        private void ShadowButton_Click(object sender, RoutedEventArgs e)
        {
            new ShadowView().ShowDialog();
        }

        private void StretchButton_Click(object sender, RoutedEventArgs e)
        {
            new StretchView().ShowDialog();
        }
    }
}
