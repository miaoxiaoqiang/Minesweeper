using System.ComponentModel;

namespace Minesweeper
{
    /// <summary>
    /// NickNameWindow.xaml 的交互逻辑
    /// </summary>
    public sealed partial class NickNameWindow : XPWindow
    {
        public NickNameWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel.NickNameViewModel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
