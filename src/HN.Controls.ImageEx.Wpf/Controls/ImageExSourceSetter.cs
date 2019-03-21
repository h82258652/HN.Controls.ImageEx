using System.Windows.Controls;
using System.Windows.Media;

namespace HN.Controls
{
    /// <inheritdoc />
    public class ImageExSourceSetter : IImageExSourceSetter
    {
        /// <inheritdoc />
        public void SetSource(Image host, ImageSource source)
        {
            host.Source = source;
        }
    }
}
