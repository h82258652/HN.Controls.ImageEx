using System;

namespace HN.Controls
{
    public class ImageExFailedEventArgs : EventArgs
    {
        public ImageExFailedEventArgs(object source, Exception failedException)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Exception = failedException ?? throw new ArgumentNullException(nameof(failedException));
        }

        public Exception Exception { get; }

        public object Source { get; }
    }
}