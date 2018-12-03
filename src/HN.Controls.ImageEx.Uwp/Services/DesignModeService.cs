using Windows.ApplicationModel;

namespace HN.Services
{
    internal class DesignModeService : IDesignModeService
    {
        public bool IsInDesignMode => DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled;
    }
}
