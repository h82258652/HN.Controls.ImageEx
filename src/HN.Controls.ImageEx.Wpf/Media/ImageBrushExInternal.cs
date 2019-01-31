using System.Windows;

namespace HN.Media
{
    public class ImageBrushExInternal : DependencyObject
    {
        internal ImageBrushExInternal(ImageBrushExExtension owner)
        {
            Owner = owner;
        }

        internal ImageBrushExExtension Owner { get; }
    }
}
