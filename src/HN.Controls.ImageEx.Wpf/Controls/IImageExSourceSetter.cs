using System.Windows.Controls;
using System.Windows.Media;

namespace HN.Controls
{
    /// <summary>
    /// 表示如何将图像数据源设置到呈现对象上。
    /// </summary>
    public interface IImageExSourceSetter
    {
        /// <summary>
        /// 将图像数据源设置到呈现对象上。
        /// </summary>
        /// <param name="host">呈现图像的对象。</param>
        /// <param name="source">图像数据源。</param>
        void SetSource(Image host, ImageSource? source);
    }
}
