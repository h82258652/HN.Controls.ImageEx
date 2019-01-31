using System;

namespace HN.Media
{
    public class ImageBrushExFailedEventArgs : EventArgs
    {
        public ImageBrushExFailedEventArgs(object source, Exception failedException)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Exception = failedException ?? throw new ArgumentNullException(nameof(failedException));
        }

        public Exception Exception { get; }

        public object Source { get; }
    }
}
