using System.ComponentModel;
using System.Windows;

namespace HN.Services
{
    public class DesignModeService : IDesignModeService
    {
        public bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
    }
}
