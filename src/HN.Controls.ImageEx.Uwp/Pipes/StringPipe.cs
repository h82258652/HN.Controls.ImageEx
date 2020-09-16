using System;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    public class StringPipe<TSource> : StringPipeBase<TSource> where TSource : class
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="StringPipe{TSource}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public StringPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        protected override Uri ToUriSource(string source)
        {
            Uri? uriSource;
            if (Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out uriSource))
            {
                if (!uriSource.IsAbsoluteUri)
                {
                    Uri.TryCreate((source.StartsWith('/') ? "ms-appx://" : "ms-appx:///") + source, UriKind.Absolute, out uriSource);
                }
            }

            if (uriSource == null)
            {
                throw new NotSupportedException();
            }

            return uriSource;
        }
    }
}
