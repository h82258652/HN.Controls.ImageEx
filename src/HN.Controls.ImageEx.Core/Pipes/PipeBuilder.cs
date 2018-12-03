using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Pipes
{
    public static class PipeBuilder
    {
        public static PipeDelegate<TResult> Build<TResult>(IServiceCollection services) where TResult : class
        {
            PipeDelegate<TResult> end = (context, cancellationToken) =>
            {
                if (context.Result == null)
                {
                    context.Result = context.Current as TResult;
                }

                if (context.Result == null)
                {
                    throw new NotSupportedException();
                }

                return Task.CompletedTask;
            };

            var serviceProvider = services.BuildServiceProvider();
            var pipes = serviceProvider.GetServices<IPipe<TResult>>();
            foreach (var pipe in pipes.Reverse())
            {
                Func<PipeDelegate<TResult>, PipeDelegate<TResult>> handler = next =>
                {
                    return (context, cancellationToken) =>
                    {
                        using (pipe)
                        {
                            return pipe.InvokeAsync(context, next, cancellationToken);
                        }
                    };
                };
                end = handler(end);
            }

            return end;
        }
    }
}
