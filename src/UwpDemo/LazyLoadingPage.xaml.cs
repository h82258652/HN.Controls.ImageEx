using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace UwpDemo
{
    public sealed partial class LazyLoadingPage
    {
        public LazyLoadingPage()
        {
            InitializeComponent();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void ImageEx_ImageOpened(object sender, EventArgs e)
        {
            await new MessageDialog("Image Opened").ShowAsync();
        }
    }
}
