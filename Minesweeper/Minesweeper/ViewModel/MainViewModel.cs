using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using MvvmLight;
using MvvmLight.CommandWpf;
using MvvmLight.Messaging;
using Minesweeper.Core;
using Minesweeper.Model;

namespace Minesweeper.ViewModel
{
    public sealed class MainViewModel : ViewModelBase //INotifyDataErrorInfo
    {
        private readonly IReadOnlyDictionary<GameLevel, ValueTuple<int, int, int>> levelInfos;
        private readonly System.Threading.Timer _gameThreadingTimer;
        private readonly System.Diagnostics.Stopwatch sw;
        //private readonly System.Timers.Timer _clickTimer;
        private readonly Streak[] streakArray;
        private int rows;
        private int cols;
        private int minescount;
        private bool leftrightpresssed = false;

        public MainViewModel()
        {
            Messenger.Default.Register<ValueTuple<int, int, int, GameLevel>>(this, "CustomMineToken", ReceiveCustomMineData);

            streakArray = new Streak[]
            {
                new(GameLevel.Primary, 0, 0),
                new(GameLevel.Intermediate, 0, 0),
                new(GameLevel.Advanced, 0, 0),
                new(GameLevel.Custom, 0, 0),
            };
            sw = new();
            //_clickTimer = new System.Timers.Timer(200);
            //_clickTimer.Elapsed += MouseClickTimerElapsed;
            _gameThreadingTimer = new System.Threading.Timer(GameThreadTimerCallback, null, System.Threading.Timeout.Infinite, 1000);
            levelInfos = new Dictionary<GameLevel, ValueTuple<int, int, int>>()
            {
                { GameLevel.Primary, ValueTuple.Create(9, 9, 10) },
                { GameLevel.Intermediate, ValueTuple.Create(16, 16, 40) },
                { GameLevel.Advanced, ValueTuple.Create(16, 30, 99) }
            };
            Game = new MinesweeperGame();
            UseMark = true;
            ExecuteApplyGameLevel(GameLevel.Primary);
        }

        #region 通知属性
        private double Pathwidth;
        public double PathWidth
        {
            get => Pathwidth;
            set
            {
                Pathwidth = value;
                RaisePropertyChanged();
            }
        }

        private double Pathheight;
        public double PathHeight
        {
            get => Pathheight;
            set
            {
                Pathheight = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 雷区容器宽度
        /// </summary>
        private double wrappanelwidth;
        public double WrapPanelWidth
        {
            get => wrappanelwidth;
            set
            {
                wrappanelwidth = value;
                RaisePropertyChanged();
            }
        }

        private double wrappanelheight;
        public double WrapPanelHeight
        {
            get => wrappanelheight;
            set
            {
                wrappanelheight = value;
                RaisePropertyChanged();
            }
        }

        private int maxcells;
        /// <summary>
        /// 雷区方格数量（CustomRows * CustomCols）
        /// </summary>
        public int MaxCells
        {
            get => maxcells;
            set
            {
                maxcells = value;
                RaisePropertyChanged();
            }
        }

        private GameLevel level;
        public GameLevel Level
        {
            get => level;
            set
            {
                level = value;
                RaisePropertyChanged();
            }
        }

        private bool usemark;
        public bool UseMark
        {
            get => usemark;
            set
            {
                usemark = value;
                if (!usemark)
                {
                    foreach (var item in Game.Cells)
                    {
                        if (!item.IsOpened && item.CellImage == CellImage.Question)
                        {
                            item.CellImage = CellImage.NotOpened;
                        }
                    }
                }
                RaisePropertyChanged();
            }
        }

        private FaceStatus facestatus;
        public FaceStatus FaceStatus
        {
            get => facestatus;
            set
            {
                facestatus = value;
                RaisePropertyChanged();
            }
        }

        private string minedigit;
        public string MineDigit
        {
            get => minedigit;
            set
            {
                minedigit = value;
                RaisePropertyChanged();
            }
        }

        private Cell selectedcell;
        public Cell SelectedCell
        {
            get => selectedcell;
            set
            {
                selectedcell = value;
                RaisePropertyChanged();
            }
        }

        public MinesweeperGame Game
        {
            get;
        }

        private GameStatus gameresult;
        public GameStatus GameStatus
        {
            get => gameresult;
            set
            {
                gameresult = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 选择游戏难度命令（初级、中级、高级和自定义）
        private RelayCommand<GameLevel> applygamelevelcommand;
        public RelayCommand<GameLevel> ApplyGameLevelCommand
        {
            get
            {
                applygamelevelcommand ??= new RelayCommand<GameLevel>(ExecuteApplyGameLevel);
                return applygamelevelcommand;
            }
            set
            {
                applygamelevelcommand = value;
            }
        }

        public void ExecuteApplyGameLevel(GameLevel levelobj)
        {
            Level = levelobj;

            if (levelInfos.ContainsKey(Level))
            {
                ValueTuple<int, int, int> _info = levelInfos[Level];
                rows = _info.Item1;
                cols = _info.Item2;
                minescount = _info.Item3;
            }
            MaxCells = rows * cols;
            WrapPanelWidth = 24 * cols;
            WrapPanelHeight = 24 * rows;

            Reset();

            int length = MaxCells.ToString().Length;
            MineDigit = "8".PadRight(length + 1, '8');
        }
        #endregion

        #region 点击表情按钮重置游戏命令
        private RelayCommand<MouseButtonEventArgs> faceclickcommand;
        public RelayCommand<MouseButtonEventArgs> FaceClickCommand
        {
            get
            {
                faceclickcommand ??= new RelayCommand<MouseButtonEventArgs>(ExecuteFaceClick);
                return faceclickcommand;
            }
            set
            {
                faceclickcommand = value;
            }
        }

        private void ExecuteFaceClick(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                //object _ispressed = (e.Source as DependencyObject).GetValue(System.Windows.Controls.Primitives.ButtonBase.IsPressedProperty);
                //if(bool.TryParse(_ispressed.ToString(), out bool ispressed))
                //{
                //    if (ispressed)
                //    {

                //    }
                //}
                //IInputElement inputElement = (e.Source as UIElement).InputHitTest(Mouse.GetPosition(Mouse.Captured));
                System.Windows.Media.HitTestResult result = System.Windows.Media.VisualTreeHelper.HitTest(e.Source as System.Windows.Media.Visual, Mouse.GetPosition(Mouse.DirectlyOver));
                if (result != null)
                {
                    Reset();
                }
            }
        }
        #endregion

        #region 雷区方块点击选择命令
        private RelayCommand<System.Windows.Controls.SelectionChangedEventArgs> selectitemchangedcommand;
        public RelayCommand<System.Windows.Controls.SelectionChangedEventArgs> SelectItemChangedCommand
        {
            get
            {
                selectitemchangedcommand ??= new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>(ExecuteSelectItemChanged);
                return selectitemchangedcommand;
            }
            set
            {
                selectitemchangedcommand = value;
            }
        }

        private void ExecuteSelectItemChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //foreach (var item in e.AddedItems.OfType<System.Windows.Controls.ListBoxItem>())
            //{
            //   item.IsSelected = false;
            //}

            if (GameStatus == GameStatus.Win || GameStatus == GameStatus.Fail)
            {
                return;
            }

            Cell _cell = e.AddedItems.Cast<Cell>().FirstOrDefault();
            if (_cell != null)
            {
                if (!_cell.IsOpened)
                {
                    //右键标识方块状态
                    if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Pressed)
                    {
                        if (_cell.CellImage == CellImage.NotOpened)
                        {
                            _cell.CellImage = CellImage.Flag;
                            Game.FlagCount++;
                        }
                        else if (_cell.CellImage == CellImage.Flag)
                        {
                            if (UseMark)
                            {
                                _cell.CellImage = CellImage.Question;
                            }
                            else
                            {
                                _cell.CellImage = CellImage.NotOpened;
                            }

                            Game.FlagCount--;
                        }
                        else if (_cell.CellImage == CellImage.Question)
                        {
                            _cell.CellImage = CellImage.NotOpened;
                        }

                        return;
                    }
                    else
                    {
                        if (_cell.CellImage == CellImage.Flag)
                        {
                            FaceStatus = FaceStatus.Normal;
                        }
                        else if (_cell.IsBlank
                            && (_cell.CellImage == CellImage.NotOpened
                            || _cell.CellImage == CellImage.Blank
                            || _cell.CellImage == CellImage.Question))
                        {
                            FaceStatus = FaceStatus.Nervous;
                        }
                    }
                }
                else
                {
                    if (leftrightpresssed)
                    {
                        if (_cell.AroundMineNum > 0)
                        {
                            FaceStatus = FaceStatus.Nervous;
                        }
                        else
                        {
                            FaceStatus = FaceStatus.Normal;
                        }
                    }
                }
            }

            foreach (object item in e.RemovedItems)
            {
                if (item is Cell cell)
                {
                    if (!cell.IsOpened)
                    {
                        if (cell.CellImage == CellImage.Blank)
                        {
                            cell.CellImage = CellImage.NotOpened;
                        }
                        else if (cell.CellImage == CellImage.QuestionClicked)
                        {
                            cell.CellImage = CellImage.Question;
                        }
                    }
                }
            }

            Game.UpdatePressedCell(SelectedCell);

            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                FaceStatus = FaceStatus.Normal;
            }
        }

        private RelayCommand<MouseButtonEventArgs> itemmousedowncommand;
        public RelayCommand<MouseButtonEventArgs> ItemMouseDownCommand
        {
            get
            {
                itemmousedowncommand ??= new RelayCommand<MouseButtonEventArgs>(ExecuteItemMouseDown);
                return itemmousedowncommand;
            }
            set
            {
                itemmousedowncommand = value;
            }
        }

        private void ExecuteItemMouseDown(MouseButtonEventArgs e)
        {
            if (GameStatus == GameStatus.Win || GameStatus == GameStatus.Fail)
            {
                return;
            }

            //if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            //{
            //    _clickTimer.Start();
            //}
            //else if (e.ClickCount == 2)
            //{
            //    e.Handled = true;
            //    _clickTimer.Stop();
            //    System.Diagnostics.Debug.WriteLine(2);
            //}
            //else
            //{
            //    _clickTimer.Stop();
            //}
            leftrightpresssed = false;

            System.Windows.Media.HitTestResult result = System.Windows.Media.VisualTreeHelper.HitTest(e.Source as System.Windows.Media.Visual, Mouse.GetPosition(Mouse.DirectlyOver));
            if (result != null)
            {
                //_cell = result.VisualHit.GetValue(FrameworkElement.DataContextProperty) as Cell;
                if (Mouse.RightButton == MouseButtonState.Pressed && Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    leftrightpresssed = true;

                    if (SelectedCell.IsOpened && SelectedCell.AroundMineNum > 0)
                    {
                        FaceStatus = FaceStatus.Nervous;
                    }
                    Game.UpdatePressedCell(SelectedCell);
                }
            }
        }

        private RelayCommand<MouseButtonEventArgs> itemmouseupcommand;
        public RelayCommand<MouseButtonEventArgs> ItemMouseUpCommand
        {
            get
            {
                itemmouseupcommand ??= new RelayCommand<MouseButtonEventArgs>(ExecuteItemMouseUp);
                return itemmouseupcommand;
            }
            set
            {
                itemmouseupcommand = value;
            }
        }

        private void ExecuteItemMouseUp(MouseButtonEventArgs e)
        {
            if (GameStatus == GameStatus.Win || GameStatus == GameStatus.Fail)
            {
                return;
            }

            FaceStatus = FaceStatus.Normal;

            System.Windows.Media.HitTestResult result = System.Windows.Media.VisualTreeHelper.HitTest(e.Source as System.Windows.Media.Visual, e.GetPosition(Mouse.DirectlyOver));
            //IInputElement inputElement = (e.Source as UIElement).InputHitTest(e.GetPosition(Mouse.DirectlyOver));
            //_cell = result.VisualHit.GetValue(FrameworkElement.DataContextProperty) as Cell;
            if (result != null)
            {
                if (SelectedCell != null && SelectedCell.IsOpened
                    && SelectedCell.AroundMineNum > 0 && leftrightpresssed)
                {
                    GameStatus status = Game.OpenAroundCellsByFlaged(SelectedCell);
                    if (status == GameStatus.Fail || status == GameStatus.Win)
                    {
                        _gameThreadingTimer.Change(-1, System.Threading.Timeout.Infinite);
                        sw.Stop();
                        FaceStatus = (FaceStatus)((int)status - 1);

                        NotifySaveRecord(status);

                        //TODO 录像文件保存
                        GameStatus = status;
                        return;
                    }
                }

                if (Mouse.RightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left && SelectedCell != null)
                {
                    if (GameStatus != GameStatus.Start)
                    {
                        Game.StartGame(SelectedCell.Index);
                        GameStatus = GameStatus.Start;
                        sw.Start();
                        _gameThreadingTimer.Change(0, 1000);
                    }

                    if (!SelectedCell.IsOpened)
                    {
                        GameStatus status = Game.OpenCell(SelectedCell, true);
                        if (status == GameStatus.Fail || status == GameStatus.Win)
                        {
                            _gameThreadingTimer.Change(-1, System.Threading.Timeout.Infinite);
                            sw.Stop();
                            FaceStatus = (FaceStatus)((int)status - 1);

                            NotifySaveRecord(status);

                            //TODO 录像文件保存
                            GameStatus = status;
                            return;
                        }
                    }
                }
            }

            SelectedCell = null;
            Game.UpdatePressedCell(SelectedCell);
        }
        #endregion

        #region 全部插旗或全部去旗命令
        private RelayCommand<bool> markallflagcommand;
        public RelayCommand<bool> MarkAllFlagCommand
        {
            get
            {
                markallflagcommand ??= new RelayCommand<bool>(ExecuteMarkAllFlag);
                return markallflagcommand;
            }
            set
            {
                markallflagcommand = value;
            }
        }

        public void ExecuteMarkAllFlag(bool args)
        {
            if (GameStatus == GameStatus.Win || GameStatus == GameStatus.Fail)
            {
                return;
            }

            int flagcount = Game.FlagCount;
            if (args)
            {
                foreach (Cell cell in Game.Cells)
                {
                    if (cell.CellImage == CellImage.NotOpened || cell.CellImage == CellImage.Question)
                    {
                        cell.CellImage = CellImage.Flag;
                        flagcount++;
                    }
                }
            }
            else
            {
                foreach (Cell cell in Game.Cells)
                {
                    if (cell.CellImage == CellImage.Flag)
                    {
                        cell.CellImage = CellImage.NotOpened;
                        flagcount--;
                    }
                }
            }

            //为什么不用属性FlagCount而是用字段flagcount进行自增或自减呢？因为用FlagCount进行自增或自减的话,
            //每当FlagCount值变化，前台绑定此属性的元素（显示雷数变化）更新数据每次都要调用转换器。
            //因此只要把总结果重新赋值给FlagCount属性，这样转换器只调用一次
            Game.FlagCount = flagcount;
        }
        #endregion

        private void MouseClickTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //_clickTimer.Stop();
        }

        private void GameThreadTimerCallback(object state)
        {
            Game.Duration += 1;
        }

        private void Reset()
        {
            sw.Reset();
            _gameThreadingTimer.Change(-1, System.Threading.Timeout.Infinite);
            FaceStatus = FaceStatus.Normal;
            GameStatus = GameStatus.Stop;
            Game.InitGame(rows, cols, minescount);
        }

        private void ReceiveCustomMineData(ValueTuple<int, int, int, GameLevel> data)
        {
            rows = data.Item1;
            cols = data.Item2;
            minescount = data.Item3;

            ExecuteApplyGameLevel(data.Item4);
        }

        private void NotifySaveRecord(GameStatus status)
        {
            int win = 0;

            History history = null;

            ref Streak result1 = ref streakArray[(int)Level];
            ChangeValuetuple(status, ref result1);

            if (status == GameStatus.Win)
            {
                win = 1;

                history = new()
                {
                    Cols = cols,
                    Rows = rows,
                    MinesCount = minescount,
                    Date = DateTime.Now,
                    TimeCost = sw.Elapsed.TotalSeconds
                };
            }

            Messenger.Default.Send(Tuple.Create(Level, win, result1.WinningStreak, result1.LosingStreak, history), "NewRecordToken");
        }

        private void ChangeValuetuple(GameStatus status, ref Streak streak)
        {
            if (status == GameStatus.Fail)
            {
                streak.WinningStreak = 0;
                streak.LosingStreak++;
            }
            else
            {
                streak.WinningStreak++;
                streak.LosingStreak = 0;
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();

            Messenger.Default.Unregister(this);
        }
    }
}
