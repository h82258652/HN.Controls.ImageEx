using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    public abstract class PipeBase<TResult> : IPipe<TResult> where TResult : class
    {
        private readonly IDesignModeService _designModeService;

        protected PipeBase(IDesignModeService designModeService)
        {
            _designModeService = designModeService ?? throw new ArgumentNullException(nameof(designModeService));
        }

        public bool IsInDesignMode => _designModeService.IsInDesignMode;

        public virtual void Dispose()
        {
        }

        public abstract Task InvokeAsync(ILoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken));
    }
}