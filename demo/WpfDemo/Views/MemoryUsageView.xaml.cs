using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace WpfDemo.Views
{
    public partial class MemoryUsageView
    {
        private readonly Stopwatch _stopwatch;
        private readonly DispatcherTimer _timer;

        public MemoryUsageView()
        {
            InitializeComponent();

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            ListView.ItemsSource = Enumerable.Range(0, 100).Select(temp => "https://www.baidu.com/img/bd_logo1.png?t=" + Guid.NewGuid()).ToList();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.2)
            };
            _timer.Tick += Timer_Tick;
        }

        private void MemoryUsageView_Loaded(object sender, RoutedEventArgs e)
        {
            _stopwatch.Stop();
            LoadTimeTextBlock.Text = $"Load time: {_stopwatch.ElapsedMilliseconds}ms";

            _timer.Start();
        }

        private void MemoryUsageView_Unloaded(object sender, RoutedEventArgs e)
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
