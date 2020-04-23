using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace WpfDemo
{
    public partial class MemoryUsageWindow
    {
        private readonly Stopwatch _stopwatch;
        private readonly DispatcherTimer _timer;

        public MemoryUsageWindow()
        {
            InitializeComponent();

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            ListView.ItemsSource = Enumerable.Range(0, 500).Select(temp => "https://www.baidu.com/img/bd_logo1.png?t=" + Guid.NewGuid()).ToList();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.2)
            };
            _timer.Tick += Timer_Tick;
        }

        private void MemoryUsageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _stopwatch.Stop();
            LoadTimeTextBlock.Text = $"Load time: {_stopwatch.ElapsedMilliseconds}ms";

            _timer.Start();
        }

        private void MemoryUsageWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var privateMemorySize64 = Process.GetCurrentProcess().PrivateMemorySize64;
            var memoryUsage = Microsoft.Toolkit.Converters.ToFileSizeString(privateMemorySize64);
            MemoryUsageTextBlock.Text = memoryUsage;
        }
    }
}
