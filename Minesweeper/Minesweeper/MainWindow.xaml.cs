using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MvvmLight.Messaging;

namespace Minesweeper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public sealed partial class MainWindow : XPWindow
    {
        private readonly IDictionary<string, Window> _windows;

        public MainWindow()
        {
            _windows = new Dictionary<string, Window>();
            Messenger.Default.Register<string>(this, "CloseWindowToken", ClosePW);

            InitializeComponent();
            Focusable = true;
            Loaded += MainWindow_Loaded;
            SizeChanged += Window_SizeChanged;

            _windows.Add("About", new AboutWindow());
            _windows.Add("Custom", new MineCustomWindow());
            _windows.Add("NickName", new NickNameWindow());
            _windows.Add("HeroRank", new HeroWindow());
            DataContext = new ViewModel.MainViewModel();

            AboutGame.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ShowAbout), true);
            CustomGame.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ShowCustom), true);
            SetName.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ShowName), true);
            ShowHeroRank.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ShowRank), true);

            listbox.AddHandler(ListBox.MouseLeaveEvent, new MouseEventHandler(ListBoxMouseLeave), true);

            PreviewKeyDown += (s, e) =>
            {
                if (DataContext is ViewModel.MainViewModel viewModel)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        if (Keyboard.IsKeyDown(Key.F) && Keyboard.IsKeyDown(Key.A))
                        {
                            viewModel.MarkAllFlagCommand.Execute(true);
                        }
                        else if (Keyboard.IsKeyDown(Key.F) && Keyboard.IsKeyDown(Key.C))
                        {
                            viewModel.MarkAllFlagCommand.Execute(false);
                        }
                        else if (Keyboard.IsKeyDown(Key.B))
                        {
                            viewModel.ApplyGameLevelCommand.Execute(Model.GameLevel.Primary);
                        }
                        else if (Keyboard.IsKeyDown(Key.I))
                        {
                            viewModel.ApplyGameLevelCommand.Execute(Model.GameLevel.Intermediate);
                        }
                        else if (Keyboard.IsKeyDown(Key.E))
                        {
                            viewModel.ApplyGameLevelCommand.Execute(Model.GameLevel.Advanced);
                        }
                        else if (Keyboard.IsKeyDown(Key.H))
                        {
                            viewModel.UseMark = !viewModel.UseMark;
                        }
                        else if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.A))
                        {
                            e.Handled = true;
                            OpenPW("About");
                        }
                        else if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.C))
                        {
                            e.Handled = true;
                            OpenPW("Custom");
                        }
                        else if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.T))
                        {
                            e.Handled = true;
                            OpenPW("HeroRank");
                        }
                        else if (Keyboard.IsKeyDown(Key.O) && Keyboard.IsKeyDown(Key.A))
                        {
                            e.Handled = true;
                            OpenPW("NickName");
                        }
                        e.Handled = true;
                    }
                }
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Messenger.Default.Unregister(this);

            base.OnClosing(e);
            Application.Current.Shutdown();
            //Environment.Exit(0);
        }

        protected override void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("是否退出程序？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //DependencyObject @do = VisualTreeHelper.GetChild(this, 0);
            //if(LogicalTreeHelper.FindLogicalNode(@do, "CloseButton") is Button button)
            //{
            //    button.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(CloseApp), true);
            //}

            //WIN32.GetTaskbarState();

            Dispatcher.Invoke(new Action(() =>
            {
                Messenger.Default.Send(ValueTuple.Create(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height, ActualWidth, ActualHeight), "ResolutionToken");

            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = (SystemParameters.WorkArea.Width / 2) - (ActualWidth / 2);
            Top = (SystemParameters.WorkArea.Height / 2) - (ActualHeight / 2);
        }

        private void ListBoxMouseLeave(object sender, MouseEventArgs e)
        {
            listbox.SelectedIndex = -1;
            listbox.SelectedItem = null;
        }

        private void ShowCustom(object sender, MouseButtonEventArgs e)
        {
            OpenPW("Custom");
        }

        private void ShowName(object sender, MouseButtonEventArgs e)
        {
            OpenPW("NickName");
        }

        private void ShowRank(object sender, MouseButtonEventArgs e)
        {
            OpenPW("HeroRank");
        }

        private void ShowAbout(object sender, MouseButtonEventArgs e)
        {
            OpenPW("About");
        }

        private void PathVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //bool value1 = bool.Parse(e.NewValue.ToString());
            //bool value2 = bool.Parse(e.OldValue.ToString());
            //if (value1 && !value2)
            //{
            //    Path path = sender as Path;
            //    //RotateTransform rotation = path.RenderTransform. as RotateTransform;
            //    double initangle = Convert.ToDouble(path.RenderTransform.GetValue(RotateTransform.AngleProperty));
            //    TransformGroup transform = path.GetValue(RenderTransformProperty) as TransformGroup;
            //    path.ClearValue(UIElement.RenderTransformProperty);
            //    storyboard.Children.Clear();
            //    RotateTransform rt_FanRotate = new()
            //    {
            //        Angle = initangle
            //    };
            //    path.RenderTransform = rt_FanRotate;

            //    DoubleAnimation animation1 = new()
            //    {
            //        From = initangle,
            //        To = 1110,
            //        Duration = new Duration(TimeSpan.FromMilliseconds(1000))
            //    };
            //    DependencyProperty[] propertyChain = new DependencyProperty[]
            //    {
            //        Path.RenderTransformProperty,
            //        RotateTransform.AngleProperty
            //    };
            //    Storyboard.SetTarget(animation1, path);
            //    Storyboard.SetTargetProperty(animation1, new PropertyPath("(0).(1)", propertyChain));
            //    storyboard.Children.Add(animation1);
            //    //storyboard.RepeatBehavior = RepeatBehavior.Forever;
            //    storyboard.FillBehavior = FillBehavior.Stop;
            //    storyboard.Begin(path);
            //}
        }

        private void OpenPW(string windowName)
        {
            if (_windows.ContainsKey(windowName))
            {
                double newtop = Top + Height / 2;
                double newleft = Left + Width / 2;
                _windows[windowName].Top = newtop - _windows[windowName].Height / 2;
                _windows[windowName].Left = newleft - _windows[windowName].Width / 2;
                _windows[windowName].ShowDialog();
            }
        }

        private void ClosePW(string windowName)
        {
            if (_windows.ContainsKey(windowName))
            {
                _windows[windowName].Close();
            }
        }
    }
}
