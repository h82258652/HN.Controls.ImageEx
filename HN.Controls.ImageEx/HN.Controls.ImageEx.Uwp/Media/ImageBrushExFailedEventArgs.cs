using System;

namespace HN.Media
{
    public class ImageBrushExFailedEventArgs : EventArgs
    {
        public ImageBrushExFailedEventArgs(object source, Exception failedException)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (failedException == null)
            {
                throw new ArgumentNullException(nameof(failedException));
            }

            Source = source;
            Exception = failedException;
        }

        public Exception Exception { get; }

        public object Source { get; }
    }
}