using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Minesweeper
{
    /// <summary>
    /// HeroWindow.xaml 的交互逻辑
    /// </summary>
    public sealed partial class HeroWindow : XPWindow
    {
        private ScrollViewer scrollViewer;

        public HeroWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.HeroRankViewModel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(scrollViewer == null)
            {
                UIElement element = sender as UIElement;
                DependencyObject @do = VisualTreeHelper.GetChild(element, 0);
                if (VisualTreeHelper.GetChild(@do, 0) is ScrollViewer scroll)
                {
                    scrollViewer = scroll;
                }
            }

            if (scrollViewer != null)
            {
                if (e.Delta > 0)
                {
                    scrollViewer.LineUp();
                    scrollViewer.LineUp();
                }
                else
                {
                    scrollViewer.LineDown();
                    scrollViewer.LineDown();
                }
            }

            e.Handled = true;
        }
    }
}
