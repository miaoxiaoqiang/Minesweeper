using System.ComponentModel;

namespace Minesweeper.Model
{
    /// <summary>
    /// 游戏难度
    /// </summary>
    public enum GameLevel
    {
        /// <summary>
        /// 初级
        /// </summary>
        [Description("初级")]
        Primary,
        /// <summary>
        /// 中级
        /// </summary>
        [Description("中级")]
        Intermediate,
        /// <summary>
        /// 高级
        /// </summary>
        [Description("高级")]
        Advanced,
        /// <summary>
        /// 自定义难度
        /// </summary>
        [Description("自定义")]
        Custom
    }
}
