using Windows.UI.Xaml;

namespace UwpDemo.Views
{
    public sealed partial class ProgressView
    {
        public ProgressView()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            ImageEx.Source = "https://edmullen.net/test/rc.jpg";
        }
    }
}
