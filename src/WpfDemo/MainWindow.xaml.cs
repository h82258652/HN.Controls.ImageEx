using HN.Cache;
using System.IO;
using System.Windows;

namespace WpfDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DiskImage.Source = Path.Combine(System.Windows.Forms.Application.StartupPath, "disk_image.png");
        }

        private async void ClearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            var diskCache = new DiskCache();
            await diskCache.DeleteAllAsync();
            MessageBox.Show("done");
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            CustomImage.Source = UrlTextBox.Text;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var diskCache = new DiskCache();
            var cacheSize = await diskCache.CalculateAllSizeAsync();
            CacheSizeTextBlock.Text = cacheSize.ToString();
        }
    }
}