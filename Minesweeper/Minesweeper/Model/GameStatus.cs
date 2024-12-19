namespace Minesweeper.Model
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// 继续游戏
        /// </summary>
        /// <remarks>
        /// 表示挖开一个雷，但未触雷或已挖出所有安全方格导致游戏胜利
        /// </remarks>
        Continue,
        /// <summary>
        /// 游戏未开始
        /// </summary>
        Stop,
        /// <summary>
        /// 游戏开始
        /// </summary>
        Start,
        /// <summary>
        /// 失败
        /// </summary>
        Fail,
        /// <summary>
        /// 胜利
        /// </summary>
        Win
    }
}
