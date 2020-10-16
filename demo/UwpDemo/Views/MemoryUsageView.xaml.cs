using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;

namespace UwpDemo.Views
{
    public sealed partial class MemoryUsageView
    {
        private readonly Stopwatch _stopwatch;

        public MemoryUsageView()
        {
            InitializeComponent();

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            GridView.ItemsSource = Enumerable.Range(0, 100).Select(temp => "https://www.baidu.com/img/bd_logo1.png?t=" + Guid.NewGuid()).ToList();
        }

        private void MemoryUsageView_Loaded(object sender, RoutedEventArgs e)
        {
            _stopwatch.Stop();
            LoadTimeTextBlock.Text = $"Load time: {_stopwatch.ElapsedMilliseconds}ms";
        }
    }
}
