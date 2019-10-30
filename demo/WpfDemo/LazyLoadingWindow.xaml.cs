using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfDemo
{
    public partial class LazyLoadingWindow
    {
        public LazyLoadingWindow()
        {
            InitializeComponent();
        }

        private async void ImageEx_ImageOpened(object sender, EventArgs e)
        {
            // if you disable lazy loading, you should add this code.
            // await Task.Yield();
            MessageBox.Show("Image Opened");
        }
    }
}
