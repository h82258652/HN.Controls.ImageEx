using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HN.Pipes
{
    public class ByteArrayPipe<TResult> : PipeBase<TResult> where TResult : class
    {
        public override Task InvokeAsync(LoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is byte[] bytes)
            {
                context.Current = new MemoryStream(bytes);
            }
            return next(context, cancellationToken);
        }
    }
}