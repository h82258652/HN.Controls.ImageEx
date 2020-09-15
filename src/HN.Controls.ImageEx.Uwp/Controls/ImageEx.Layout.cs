using Windows.Foundation;
using Windows.UI.Xaml;

namespace HN.Controls
{
    [TemplatePart(Name = RootTemplateName, Type = typeof(UIElement))]
    public partial class ImageEx
    {
        private const string RootTemplateName = "PART_Root";
        private UIElement? _root;

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_displaySource == null || _root == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            var size = CalculateDisplaySourceStretchSize(finalSize);
            _root.Arrange(new Rect(new Point(), size));
            return size;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_displaySource == null)
            {
                return base.MeasureOverride(availableSize);
            }

            return CalculateDisplaySourceStretchSize(availableSize);
        }

        private Size CalculateDisplaySourceStretchSize(Size inputSize)
        {
            var displaySource = _displaySource;
            if (displaySource == null)
            {
                return Size.Empty;
            }

            var naturalSize = new Size(displaySource.Width, displaySource.Height);

            var scaleFactor = StretchHelper.CalculateScaleFactor(inputSize, naturalSize, Stretch, StretchDirection);

            return new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);
        }
    }
}
