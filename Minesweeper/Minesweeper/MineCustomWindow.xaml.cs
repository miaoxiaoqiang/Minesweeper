using System.ComponentModel;

namespace Minesweeper
{
    /// <summary>
    /// MineCustomWindow.xaml 的交互逻辑
    /// </summary>
    public sealed partial class MineCustomWindow : XPWindow
    {
        public MineCustomWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel.MineCustomViewModel();

            //MineRows.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(TextNumberInput), true);
            //MineCols.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(TextNumberInput), true);
            //MineCounts.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(TextNumberInput), true);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        //private void TextNumberInput(object sender, TextCompositionEventArgs e)
        //{
        //    e.Handled = System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled);
        //}

        //protected override void CloseWindow(object sender, MouseButtonEventArgs e)
        //{
        //    Close();

        //    //不知道还有什么好办法？比如如何通过命令或行为方式能执行窗体关闭再执行命令（不通过事件方式）
        //    //大家有想法可在本项目所在的github地址中的issue板块提出
        //    //if (sender is Button button)
        //    //{
        //    //    if (button.Command != null)
        //    //    {
        //    //        if (button.Command.CanExecute(null))
        //    //        {
        //    //            button.Command.Execute(null);
        //    //        }
        //    //    }
        //    //}

        //    //目前替代方法是：
        //    /*
        //     * 在MainWindow构造函数中，将其对应的接口对象（类型：IMineService）注入到对应的ViewModel，由接口中的方法实现打开或关闭窗口
        //     */
        //}
    }
}
