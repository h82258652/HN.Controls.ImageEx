using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace HN.Pipes
{
    public delegate Task PipeDelegate<TResult>([NotNull]LoadingContext<TResult> context, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
}