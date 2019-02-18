using System;

namespace HN.Extensions
{
    /// <summary>
    /// <see cref="Uri" /> 扩展类。
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// 获取 <see cref="Uri" /> 实例是否是 Http 协议。
        /// </summary>
        /// <param name="uri"><see cref="Uri" /> 实例。</param>
        /// <returns><see cref="Uri" /> 实例是否是 Http 协议。</returns>
        /// <exception cref="ArgumentNullException">uri 为 <see langword="null"/>。</exception>
        public static bool IsHttp(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var scheme = uri.Scheme;
            return uri.IsAbsoluteUri &&
                   (string.Equals(scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
        }
    }
}
