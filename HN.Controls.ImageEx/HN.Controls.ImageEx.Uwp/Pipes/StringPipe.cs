using System;
using System.Threading;
using System.Threading.Tasks;

namespace HN.Pipes
{
    public class StringPipe<TResult> : PipeBase<TResult> where TResult : class
    {
        public override Task InvokeAsync(LoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is string source)
            {
                context.Current = ToUriSource(source);
            }
            return next(context, cancellationToken);
        }

        private static Uri ToUriSource(string source)
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