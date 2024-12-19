using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Minesweeper
{
    /// <summary>
    /// XPWindow.xaml 的交互逻辑
    /// </summary>
    public partial class XPWindow : Window
    {
        public XPWindow()
        {
            Icon = Utils.Helper.ExeIcon;

            //System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName);
            //if (icon != null)
            //{
            //    BitmapSource source = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //    Icon = source;
            //}

            Style = Application.Current.Resources["BaseWindowStyle"] as Style;

            Loaded += WindowLoaded;
        }

        protected bool IsLocked
        {
            get;
            set;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject @do = VisualTreeHelper.GetChild(this, 0);
            if (LogicalTreeHelper.FindLogicalNode(@do, "TitleBarGrid") is Grid grid)
            {
                grid.AddHandler(Grid.MouseLeftButtonDownEvent, new MouseButtonEventHandler(TitleBarMove), true);
            }
            if (LogicalTreeHelper.FindLogicalNode(@do, "CloseButton") is Button button)
            {
                button.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(CloseWindow), true);
            }
        }

        private void TitleBarMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsLocked)
            {
                DragMove();
            }
        }

        protected virtual void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            base.Close();
        }
    }
}
