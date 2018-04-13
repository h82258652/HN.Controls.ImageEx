using System;

namespace HN.Pipes
{
    public class LoadingContext<TResult> where TResult : class
    {
        private byte[] _httpResponseBytes;

        public LoadingContext(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            OriginSource = source;
            Current = source;
        }

        public object Current { get; set; }

        public byte[] HttpResponseBytes
        {
            get => _httpResponseBytes;
            set
            {
                if (_httpResponseBytes != null)
                {
                    throw new InvalidOperationException("value has been set.");
                }

                _httpResponseBytes = value;
            }
        }

        public object OriginSource { get; }

        internal TResult Result { get; set; }

        internal void Reset()
        {
            Current = OriginSource;
            Result = null;
            _httpResponseBytes = null;
        }
    }
}