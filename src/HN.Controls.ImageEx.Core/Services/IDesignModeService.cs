namespace HN.Services
{
    /// <summary>
    /// 设计模式服务。
    /// </summary>
    public interface IDesignModeService
    {
        /// <summary>
        /// 获取当前是否在设计模式下。
        /// </summary>
        bool IsInDesignMode { get; }
    }
}
