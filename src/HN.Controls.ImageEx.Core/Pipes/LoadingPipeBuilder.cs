using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Pipes
{
    /// <summary>
    /// 该类用于构建加载管道。
    /// </summary>
    public static class LoadingPipeBuilder
    {
        /// <summary>
        /// 构建加载管道。
        /// </summary>
        /// <typeparam name="TResult">加载目标的类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>管道调用的委托。</returns>
        public static LoadingPipeDelegate<TResult> Build<TResult>(IServiceCollection services) where TResult : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            LoadingPipeDelegate<TResult> end = (context, cancellationToken) =>
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
            var pipes = serviceProvider.GetServices<ILoadingPipe<TResult>>();
            foreach (var pipe in pipes.Reverse())
            {
                Func<LoadingPipeDelegate<TResult>, LoadingPipeDelegate<TResult>> handler = next =>
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
