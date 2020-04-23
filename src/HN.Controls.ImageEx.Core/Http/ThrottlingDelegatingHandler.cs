using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Http
{
    /// <summary>
    /// 限制同时并发 Http 请求处理器。
    /// </summary>
    public class ThrottlingDelegatingHandler : DelegatingHandler
    {
        private readonly SemaphoreSlim _throttler;

        /// <summary>
        /// 初始化 <see cref="ThrottlingDelegatingHandler" /> 类的新实例。
        /// </summary>
        /// <param name="throttler"><see cref="SemaphoreSlim" /> 类实例。</param>
        public ThrottlingDelegatingHandler([NotNull] SemaphoreSlim throttler)
        {
            _throttler = throttler ?? throw new ArgumentNullException(nameof(throttler));
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _throttler.WaitAsync(cancellationToken);
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                _throttler.Release();
            }
        }
    }
}
