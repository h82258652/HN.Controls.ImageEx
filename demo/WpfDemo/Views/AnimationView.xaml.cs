using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfDemo.Views
{
    public partial class AnimationView
    {
        public AnimationView()
        {
            InitializeComponent();
        }

        private void FrameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FrameComboBox.SelectedItem != null)
            {
                var index = int.Parse(FrameComboBox.SelectedItem.ToString());
                ImageEx.GotoFrame(index);
            }
        }

        private void ImageEx_ImageOpened(object sender, EventArgs e)
        {
            FrameComboBox.ItemsSource = Enumerable.Range(0, ImageEx.FrameCount).ToList();
            FrameComboBox.SelectedIndex = 0;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            ImageEx.Pause();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ImageEx.Play();
        }

        private void SpeedRatioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ImageEx.SpeedRatio = SpeedRatioSlider.Value;
        }
    }
}
