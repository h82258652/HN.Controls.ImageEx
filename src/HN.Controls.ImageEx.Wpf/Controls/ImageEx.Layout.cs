using System.Windows;

namespace HN.Controls
{
    [TemplatePart(Name = RootTemplateName, Type = typeof(UIElement))]
    public partial class ImageEx
    {
        private const string RootTemplateName = "PART_Root";
        private UIElement? _root;

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (_displaySource == null || _root == null)
            {
                return base.ArrangeOverride(arrangeBounds);
            }

            var size = CalculateDisplaySourceStretchSize(arrangeBounds);
            _root.Arrange(new Rect(size));
            return size;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (_displaySource == null)
            {
                return base.MeasureOverride(constraint);
            }

            return CalculateDisplaySourceStretchSize(constraint);
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
