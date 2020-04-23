using System;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    /// <inheritdoc />
    public class ImageExOptions : IImageExOptions
    {
        private int _maxHttpDownloadCount;

        /// <summary>
        /// 初始化 <see cref="ImageExOptions" /> 类的新实例。
        /// </summary>
        public ImageExOptions()
        {
            MaxHttpDownloadCount = 5;
        }

        /// <inheritdoc />
        public int MaxHttpDownloadCount
        {
            get => _maxHttpDownloadCount;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "必须大于 0");
                }

                _maxHttpDownloadCount = value;
            }
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; } = new ServiceCollection();
    }

    /// <inheritdoc cref="ImageExOptions" />
    public class ImageExOptions<TSource> : ImageExOptions, IImageExOptions<TSource> where TSource : class
    {
    }
}
