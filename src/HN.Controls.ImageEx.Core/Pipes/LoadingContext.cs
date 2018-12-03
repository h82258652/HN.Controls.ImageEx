using System;
using JetBrains.Annotations;

namespace HN.Pipes
{
    public class LoadingContext<TResult> : ILoadingContext<TResult> where TResult : class
    {
        private byte[] _httpResponseBytes;

        private TResult _result;

        public LoadingContext([NotNull]object source, double? desiredWidth, double? desiredHeight)
        {
            Current = OriginSource = source ?? throw new ArgumentNullException(nameof(source));
            DesiredWidth = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        public object Current { get; set; }

        public double? DesiredHeight { get; }

        public double? DesiredWidth { get; }

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
            Result = null;
            _httpResponseBytes = null;
        }
    }
}
