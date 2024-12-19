namespace Minesweeper.Core
{
    public interface IWindow
    {
        /// <summary>
        /// 打开窗体
        /// </summary>
        public void OpenPW(string windowName);

        /// <summary>
        /// 关闭窗体
        /// </summary>
        public void ClosePW(string windowName);
    }
}
