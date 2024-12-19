using System.Windows;

namespace Minesweeper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
            MvvmLight.Threading.DispatcherHelper.Initialize();
        }
    }
}
