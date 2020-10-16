using System;
using Windows.System;
using Windows.UI.Xaml;

namespace UwpDemo.Views
{
    public sealed partial class RootView
    {
        private readonly DispatcherTimer _timer;

        public RootView()
        {
            InitializeComponent();
            RootFrame.Navigate(typeof(MainView));

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.2)
            };
            _timer.Tick += Timer_Tick;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
            }
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void RootView_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, object e)
        {
            var appMemoryUsage = MemoryManager.AppMemoryUsage;
            var memoryUsage = Microsoft.Toolkit.Converters.ToFileSizeString((long)appMemoryUsage);
            MemoryUsageTextBlock.Text = memoryUsage;
        }
    }
}
