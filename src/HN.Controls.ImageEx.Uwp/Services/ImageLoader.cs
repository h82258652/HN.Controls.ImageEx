using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Pipes;

namespace HN.Services
{
    /// <inheritdoc />
    public class ImageLoader : IImageLoader
    {
        /// <inheritdoc />
        public async Task<byte[]> GetByteArrayAsync(object source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var context = new LoadingContext<byte[]>(SynchronizationContext.Current, source, null, null);
            var pipeDelegate = ImageExService.GetHandler<byte[]>();
            await pipeDelegate.Invoke(context, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return context.Result;
        }
    }
}
