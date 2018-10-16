using System;
using System.Runtime.InteropServices.WindowsRuntime;
using HN.Cache;
using HN.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace UwpDemo
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ClearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            var diskCache = new DiskCache();
            await diskCache.DeleteAllAsync();
            await new MessageDialog("done").ShowAsync();
        }

        private void LazyLoadingButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LazyLoadingPage));
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            CustomImage.Source = UrlTextBox.Text;
        }

        private async void LoadDiskFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".png");
            var file = await fileOpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                var buffer = await FileIO.ReadBufferAsync(file);
                var bytes = buffer.ToArray();
                DiskImage.Source = bytes;
                DiskEllipse.Fill = new ImageBrushEx()
                {
                    ImageSource = bytes
                };
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var diskCache = new DiskCache();
            var cacheSize = await diskCache.CalculateAllSizeAsync();
            CacheSizeTextBlock.Text = cacheSize.ToString();
        }
    }
}