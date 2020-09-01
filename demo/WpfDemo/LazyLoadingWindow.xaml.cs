using System;
using System.Windows;

namespace WpfDemo
{
    public partial class LazyLoadingWindow
    {
        public LazyLoadingWindow()
        {
            InitializeComponent();
        }

        private void ImageEx_ImageOpened(object sender, EventArgs e)
        {
            MessageBox.Show("Image Opened");
        }
    }
}
