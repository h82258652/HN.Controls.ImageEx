using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Pipes
{
    public interface IPipe<TResult> : IDisposable where TResult : class
    {
        bool IsInDesignMode { get; }

        Task InvokeAsync([NotNull]ILoadingContext<TResult> context, [NotNull]PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken));
    }
}