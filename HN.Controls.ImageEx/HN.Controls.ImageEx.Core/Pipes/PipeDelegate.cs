using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Pipes
{
    public delegate Task PipeDelegate<TResult>([NotNull] ILoadingContext<TResult> context, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
}