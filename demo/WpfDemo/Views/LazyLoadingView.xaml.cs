using System;
using System.Windows;

namespace WpfDemo.Views
{
    public partial class LazyLoadingView
    {
        public LazyLoadingView()
        {
            InitializeComponent();
        }

        private void ImageEx_ImageOpened(object sender, EventArgs e)
        {
            MessageBox.Show("Image Opened");
        }
    }
}
