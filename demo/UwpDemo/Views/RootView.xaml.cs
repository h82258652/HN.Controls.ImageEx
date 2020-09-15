using Windows.UI.Xaml;

namespace UwpDemo.Views
{
    public sealed partial class RootView
    {
        public RootView()
        {
            InitializeComponent();
            RootFrame.Navigate(typeof(MainView));
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
            }
        }
    }
}
