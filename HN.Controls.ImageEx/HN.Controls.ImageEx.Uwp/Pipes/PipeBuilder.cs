using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HN.Pipes
{
    internal static class PipeBuilder
    {
        private static readonly IDictionary<Type, Func<object>> PipeServices = new Dictionary<Type, Func<object>>();

        internal static void AddService<T, TInterface>(Func<T> serviceFactory) where T : TInterface
        {
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }

            PipeServices[typeof(TInterface)] = () => serviceFactory();
        }

        internal static PipeDelegate<TResult> Build<TResult>(IEnumerable<Type> pipes) where TResult : class
        {
            PipeDelegate<TResult> end = (context, cancellationToken) =>
            {
                if (context.Result == null)
                {
                    throw new NotSupportedException();
                }

                return Task.CompletedTask;
            };

            foreach (var pipeType in pipes.Reverse())
            {
                Func<PipeDelegate<TResult>, PipeDelegate<TResult>> handler = next =>
                {
                    return (context, cancellationToken) =>
                    {
                        using (var pipe = CreatePipe<TResult>(pipeType))
                        {
                            return pipe.InvokeAsync(context, next, cancellationToken);
                        }
                    };
                };
                end = handler(end);
            }

            return end;
        }

        private static PipeBase<TResult> CreatePipe<TResult>(Type pipeType) where TResult : class
        {
            var constructorInfo = pipeType.GetConstructors().Single();
            var parameterInfos = constructorInfo.GetParameters();
            var parameterCount = parameterInfos.Length;
            var parameters = new object[parameterCount];
            for (var i = 0; i < parameterCount; i++)
            {
                var parameterInfo = parameterInfos[i];
                var parameterType = parameterInfo.ParameterType;
                if (!PipeServices.TryGetValue(parameterType, out var serviceFactory))
                {
                    throw new InvalidOperationException($"can not create {pipeType.FullName} instance, service type {parameterType.FullName} is not registered.");
                }

                parameters[i] = serviceFactory();
            }

            return (PipeBase<TResult>)constructorInfo.Invoke(parameters);
        }
    }
}