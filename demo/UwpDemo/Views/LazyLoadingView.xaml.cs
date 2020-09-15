using System;
using Windows.UI.Popups;

namespace UwpDemo.Views
{
    public sealed partial class LazyLoadingView
    {
        public LazyLoadingView()
        {
            InitializeComponent();
        }

        private async void ImageEx_ImageOpened(object sender, EventArgs e)
        {
            await new MessageDialog("Image Opened").ShowAsync();
        }
    }
}
