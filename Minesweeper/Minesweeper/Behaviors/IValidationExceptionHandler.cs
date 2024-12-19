namespace Minesweeper.Behaviors
{
    /// <summary>
    /// 输入校验接口
    /// </summary>
    public interface IValidationExceptionHandler
    {
        /// <summary>
        /// 是否有校验异常
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// 异常提示
        /// </summary>
        string Message { get; set; }
    }
}
