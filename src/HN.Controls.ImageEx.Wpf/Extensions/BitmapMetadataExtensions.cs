using System;
using System.Windows.Media.Imaging;

namespace HN.Extensions
{
    /// <summary>
    /// <see cref="BitmapMetadata" /> 扩展类。
    /// </summary>
    public static class BitmapMetadataExtensions
    {
        /// <summary>
        /// 尝试从位图元数据中提取值并进行类型转换。
        /// </summary>
        /// <typeparam name="T">提取值的类型。</typeparam>
        /// <param name="metadata">位图元数据。</param>
        /// <param name="query">标识要在当前 <see cref="BitmapMetadata" /> 对象中查询的字符串。</param>
        /// <returns>位于指定查询位置的元数据。</returns>
        public static T GetQueryOrDefault<T>(this BitmapMetadata metadata, string query)
        {
            if (metadata.ContainsQuery(query))
            {
                return (T)Convert.ChangeType(metadata.GetQuery(query), typeof(T));
            }

            return default;
        }
    }
}
