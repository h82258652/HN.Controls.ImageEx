using System.IO;
using System.Threading.Tasks;
using HN.Controls.ImageEx.Core.Tests.Services;
using HN.Pipes;
using Xunit;

namespace HN.Controls.ImageEx.Core.Tests.Pipes
{
    public class ByteArrayPipeTests
    {
        [Fact]
        public async Task InvokeAsyncTest()
        {
            // Arrange
            var source = new byte[0];
            var pipe = new ByteArrayPipe<object>(new TestDesignModeService());
            var context = new LoadingContext<object>(null, source, null, null, null);
            LoadingPipeDelegate<object> next = (_, __) => Task.CompletedTask;

            // Act
            await pipe.InvokeAsync(context, next);

            // Assert
            Assert.IsAssignableFrom<Stream>(context.Current);
        }
    }
}
