using System.Windows.Media;

namespace HN.Media
{
    /// <inheritdoc />
    public class ImageBrushExSourceSetter : IImageBrushExSourceSetter
    {
        /// <inheritdoc />
        public void SetSource(ImageBrush host, ImageSource source)
        {
            host.ImageSource = source;
        }
    }
}
