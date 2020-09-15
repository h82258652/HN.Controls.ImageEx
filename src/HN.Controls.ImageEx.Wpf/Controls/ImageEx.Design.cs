using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HN.Pipes;
using HN.Services;
using Polly;

namespace HN.Controls
{
    public partial class ImageEx
    {
        private const string ImageTemplateName = "PART_Image";
        private readonly bool _isInDesignMode = new DesignModeService().IsInDesignMode;
        private Image? _image;

        private void AttachDesignSource(ImageSource? source)
        {
            if (_image != null)
            {
                _image.Source = source;
            }
        }

        private async Task SetDesignSourceAsync(object? source)
        {
            if (_image == null)
            {
                return;
            }

            _lastLoadCts?.Cancel();
            if (source == null)
            {
                AttachDesignSource(null);
                VisualStateManager.GoToState(this, EmptyStateName, true);
                return;
            }

            var loadCts = new CancellationTokenSource();
            _lastLoadCts = loadCts;
            try
            {
                IsLoading = true;

                VisualStateManager.GoToState(this, LoadingStateName, true);

                // 开始 Loading，重置 DownloadProgress
                DownloadProgress = default;

                var context = new LoadingContext<ImageSource>(_uiContext, source, AttachDesignSource, ActualWidth, ActualHeight);
                context.DownloadProgressChanged += (sender, progress) =>
                {
                    if (_uiContext != null)
                    {
                        _uiContext.Post(state =>
                        {
                            DownloadProgress = progress;
                        }, null);
                    }
                    else
                    {
                        DownloadProgress = progress;
                    }
                };

                var pipeDelegate = ImageExService.GetHandler<ImageSource>();
                var retryCount = RetryCount;
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(retryCount, count => retryDelay, (ex, delay) =>
                    {
                        context.Reset();
                    });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, loadCts.Token));

                if (!loadCts.IsCancellationRequested)
                {
                    VisualStateManager.GoToState(this, OpenedStateName, true);
                    if (_uiContext != null)
                    {
                        _uiContext.Post(state =>
                        {
                            ImageOpened?.Invoke(this, EventArgs.Empty);
                        }, null);
                    }
                    else
                    {
                        ImageOpened?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!loadCts.IsCancellationRequested)
                {
                    AttachDesignSource(null);
                    VisualStateManager.GoToState(this, FailedStateName, true);
                    if (_uiContext != null)
                    {
                        _uiContext.Post(state =>
                        {
                            ImageFailed?.Invoke(this, new ImageExFailedEventArgs(source, ex));
                        }, null);
                    }
                    else
                    {
                        ImageFailed?.Invoke(this, new ImageExFailedEventArgs(source, ex));
                    }
                }
            }
            finally
            {
                if (!loadCts.IsCancellationRequested)
                {
                    IsLoading = false;
                }
            }
        }
    }
}
