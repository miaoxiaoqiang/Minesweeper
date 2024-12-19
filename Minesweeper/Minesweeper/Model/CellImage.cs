namespace Minesweeper.Model
{
    /// <summary>
    /// 方格状态图像
    /// </summary>
    public enum CellImage
    {
        /// <summary>
        /// 空的，不是雷或数字。
        /// </summary>
        /// <remarks>
        /// 表示已打开的或用于鼠标(左键或左右键)按住不放呈现效果
        /// </remarks>
        Blank = 0,
        /// <summary>
        /// 数字 1
        /// </summary>
        Num1,
        /// <summary>
        /// 数字 2
        /// </summary>
        Num2,
        /// <summary>
        /// 数字 3
        /// </summary>
        Num3,
        /// <summary>
        /// 数字 4
        /// </summary>
        Num4,
        /// <summary>
        /// 数字 5
        /// </summary>
        Num5,
        /// <summary>
        /// 数字 6
        /// </summary>
        Num6,
        /// <summary>
        /// 数字 7
        /// </summary>
        Num7,
        /// <summary>
        /// 数字 8
        /// </summary>
        Num8,
        /// <summary>
        /// 踩雷时，游戏失败。踩到的雷会红色高亮
        /// </summary>
        StepOnMine,
        /// <summary>
        /// 踩雷时，游戏失败。标错的雷（插在安全格上的旗子）叉叉标识
        /// </summary>
        MarkedErrorMine,
        /// <summary>
        /// 踩雷时，游戏失败。未标出的雷会正常显示出来
        /// </summary>
        NotMarkedMine,
        /// <summary>
        /// 正常，未打开的
        /// </summary>
        NotOpened,
        /// <summary>
        /// 标记旗子（称为标雷）
        /// </summary>
        Flag,
        /// <summary>
        /// 标记问号
        /// </summary>
        Question,
        /// <summary>
        /// 后门进入
        /// </summary>
        Cheat,
        /// <summary>
        /// 已标记问号，鼠标左键按住不放呈现效果
        /// </summary>
        QuestionClicked
    }
}
