using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HN.Pipes
{
    public abstract class PipeBase<TResult> : IDisposable where TResult : class
    {
        protected bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        public virtual void Dispose()
        {
        }

        public abstract Task InvokeAsync([NotNull]LoadingContext<TResult> context, [NotNull]PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken));
    }
}