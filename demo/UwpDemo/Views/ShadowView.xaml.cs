using HN.Controls;
using Windows.UI.Xaml;

namespace UwpDemo.Views
{
    public sealed partial class ShadowView
    {
        public ShadowView()
        {
            InitializeComponent();
        }

        private void IsShadowEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ImageEx.Shadow = (ImageExShadow)Resources["ImageExShadow"];
        }

        private void IsShadowEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageEx.Shadow = null;
        }
    }
}
