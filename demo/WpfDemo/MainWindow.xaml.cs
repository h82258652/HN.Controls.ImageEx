using System;
using System.IO;
using System.Windows;
using HN.Cache;

namespace WpfDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var diskImageSource = Path.Combine(AppContext.BaseDirectory, "disk_image.png");
            DiskImage.Source = diskImageSource;
            DiskImageBrush.ImageSource = diskImageSource;
        }

        private async void ClearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            var diskCache = new DiskCache();
            await diskCache.DeleteAllAsync();
            MessageBox.Show("done");
        }

        private void LazyLoadingButton_Click(object sender, RoutedEventArgs e)
        {
            new LazyLoadingWindow().ShowDialog();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            CustomImage.Source = UrlTextBox.Text;
        }

        private void MemoryUsageButton_Click(object sender, RoutedEventArgs e)
        {
            new MemoryUsageWindow().ShowDialog();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var diskCache = new DiskCache();
            var cacheSize = await diskCache.CalculateAllSizeAsync();
            CacheSizeTextBlock.Text = cacheSize.ToString();
        }
    }
}
