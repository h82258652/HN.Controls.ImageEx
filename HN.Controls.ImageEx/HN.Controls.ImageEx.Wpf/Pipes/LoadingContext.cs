using System;

namespace HN.Pipes
{
    public class LoadingContext<TResult>
    {
        private byte[] _httpResponseBytes;
        private TResult _result;

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

        public object OriginSource { get; set; }

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
    }
}