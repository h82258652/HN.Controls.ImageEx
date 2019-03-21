using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
