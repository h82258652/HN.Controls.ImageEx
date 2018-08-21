using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    public class ByteArrayPipe<TResult> : PipeBase<TResult> where TResult : class
    {
        public ByteArrayPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        public override Task InvokeAsync(ILoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is byte[] bytes)
            {
                context.Current = new MemoryStream(bytes);
            }

            return next(context, cancellationToken);
        }
    }
}