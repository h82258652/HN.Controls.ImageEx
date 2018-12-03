using System;
using Windows.UI.Xaml.Media;

namespace HN.Media
{
    public class ImageSurfaceFailedStatusException : Exception
    {
        public ImageSurfaceFailedStatusException(LoadedImageSourceLoadStatus status)
        {
            if (!Enum.IsDefined(typeof(LoadedImageSourceLoadStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            Status = status;
        }

        public LoadedImageSourceLoadStatus Status { get; }
    }
}
