using System;
using HN.Services;

namespace HN.Pipes
{
    public class StringPipe<TResult> : StringPipeBase<TResult> where TResult : class
    {
        public StringPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        protected override Uri ToUriSource(string source)
        {
            Uri uriSource;
            if (Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out uriSource))
            {
                if (!uriSource.IsAbsoluteUri)
                {
                    Uri.TryCreate("ms-appx:///" + (source.StartsWith("/") ? source.Substring(1) : source), UriKind.Absolute, out uriSource);
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
