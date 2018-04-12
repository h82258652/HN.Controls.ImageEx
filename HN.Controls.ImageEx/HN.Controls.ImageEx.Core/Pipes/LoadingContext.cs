using System;

namespace HN.Pipes
{
    public class LoadingContext<TResult> where TResult : class
    {
        private byte[] _httpResponseBytes;
        private TResult _result;

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

        public TResult Result
        {
            get => _result;
            set
            {
                if (_result != null)
                {
                    throw new InvalidOperationException("value has been set.");
                }

                _result = value;
            }
        }

        public void Reset()
        {
            Current = OriginSource;
            _result = null;
            _httpResponseBytes = null;
        }
    }
}