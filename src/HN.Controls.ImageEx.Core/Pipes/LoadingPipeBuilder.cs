using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        /// <typeparam name="TSource">加载源目标的类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>管道调用的委托。</returns>
        [NotNull]
        public static LoadingPipeDelegate<TSource> Build<TSource>([NotNull] IServiceCollection services) where TSource : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            LoadingPipeDelegate<TSource> end = (context, cancellationToken) =>
            {
                if (!(context.Current is TSource))
                {
                    throw new NotSupportedException();
                }

                return Task.CompletedTask;
            };

            var serviceProvider = services.BuildServiceProvider();
            var pipes = serviceProvider.GetServices<ILoadingPipe<TSource>>();
            foreach (var pipe in pipes.Reverse())
            {
                Func<LoadingPipeDelegate<TSource>, LoadingPipeDelegate<TSource>> handler = next =>
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
