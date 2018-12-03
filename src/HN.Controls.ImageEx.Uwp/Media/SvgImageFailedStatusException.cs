using System;
using Windows.UI.Xaml.Media.Imaging;

namespace HN.Media
{
    public class SvgImageFailedStatusException : Exception
    {
        public SvgImageFailedStatusException(SvgImageSourceLoadStatus status)
        {
            if (!Enum.IsDefined(typeof(SvgImageSourceLoadStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            Status = status;
        }

        public SvgImageSourceLoadStatus Status { get; }
    }
}
