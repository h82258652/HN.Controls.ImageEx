using System.Windows;

namespace WpfDemo.Views
{
    public partial class ProgressView
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
