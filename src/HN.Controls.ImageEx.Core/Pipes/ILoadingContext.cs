namespace HN.Pipes
{
    /// <summary>
    /// 加载上下文。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public interface ILoadingContext<TResult> where TResult : class
    {
        /// <summary>
        /// 当前管道的值。
        /// </summary>
        object Current { get; set; }

        /// <summary>
        /// 需求的高度。
        /// </summary>
        double? DesiredHeight { get; }

        /// <summary>
        /// 需求的宽度。
        /// </summary>
        double? DesiredWidth { get; }

        /// <summary>
        /// 若管道处理过程中涉及 HTTP 传输，则存放 HTTP 响应的内容在此。
        /// </summary>
        byte[] HttpResponseBytes { get; set; }

        /// <summary>
        /// 原始输入的数据源。
        /// </summary>
        object OriginSource { get; }

        /// <summary>
        /// 处理结果。
        /// </summary>
        TResult Result { get; set; }
    }
}
