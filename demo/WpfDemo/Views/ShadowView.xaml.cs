using System.Windows;
using HN.Controls;

namespace WpfDemo.Views
{
    public partial class ShadowView
    {
        public ShadowView()
        {
            InitializeComponent();
        }

        private void IsShadowEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ImageEx.DropShadow = (ImageExShadow)Resources["ImageExShadow"];
        }

        private void IsShadowEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageEx.DropShadow = null;
        }
    }
}
