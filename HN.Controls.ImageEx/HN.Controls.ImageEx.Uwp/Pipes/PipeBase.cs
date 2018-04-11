using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace HN.Pipes
{
    public abstract class PipeBase<TResult> : IDisposable where TResult : class
    {
        protected bool IsInDesignMode => DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled;

        public virtual void Dispose()
        {
        }

        public abstract Task InvokeAsync([NotNull]LoadingContext<TResult> context, [NotNull]PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken));
    }
}