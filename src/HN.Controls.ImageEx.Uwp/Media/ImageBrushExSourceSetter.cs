using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace HN.Media
{
    /// <inheritdoc />
    public class ImageBrushExSourceSetter : IImageBrushExSourceSetter
    {
        /// <inheritdoc />
        public void SetSource(Action<CompositionBrush> host, ICompositionSurface? source)
        {
            CompositionBrush brush = null;
            if (source != null)
            {
                var compositor = Window.Current.Compositor;
                brush = compositor.CreateSurfaceBrush(source);
            }

            host(brush);
        }
    }
}
