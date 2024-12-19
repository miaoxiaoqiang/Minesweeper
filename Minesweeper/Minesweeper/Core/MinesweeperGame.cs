using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using MvvmLight;
using Minesweeper.Model;

namespace Minesweeper.Core
{
    public sealed class MinesweeperGame : ViewModelBase
    {
        private readonly IList<Cell> pressedCells;
        private readonly IList<int> offsets;
        private readonly Stack<int> floodFillStack;

        //private int _rows;
        private int _cols;
        private int _maxcells;

        public MinesweeperGame()
        {
            floodFillStack = new Stack<int>();
            offsets = new List<int>();
            pressedCells = new List<Cell>();
            Cells = new ObservableCollection<Cell>();
        }

        /// <summary>
        /// 获取指定索引的方块周围的雷的数量
        /// </summary>
        /// <param name="cellindex">索引</param>
        /// <returns>
        /// 指定方块周围的雷的数量
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public byte this[int cellindex]
        {
            get
            {
                if (cellindex > Cells.Count || cellindex < 0)
                {
                    throw new IndexOutOfRangeException(nameof(cellindex) + "超出了索引范围");
                }

                return Cells[cellindex].AroundMineNum;
            }
        }

        #region 通知属性
        public ObservableCollection<Cell> Cells
        {
            get;
            private set;
        }

        private int flagcount;
        public int FlagCount
        {
            get => flagcount;
            set
            {
                flagcount = value;
                RaisePropertyChanged();
            }
        }

        private int minescount;
        public int MinesCount
        {
            get => minescount;
            set
            {
                minescount = value;
                RaisePropertyChanged();
            }
        }

        private long duration;
        public long Duration
        {
            get => duration;
            set
            {
                duration = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void InitGame(int rows, int cols, int minescount)
        {
            offsets.Clear();
            pressedCells.Clear();

            //_rows = rows;
            _cols = cols;
            _maxcells = rows * cols;
            MinesCount = minescount;
            FlagCount = 0;
            Duration = 0;

            //九宫格偏移量
            offsets.Add(-cols - 1);
            offsets.Add(-cols);
            offsets.Add(-cols + 1);
            offsets.Add(-1);
            offsets.Add(0);
            offsets.Add(1);
            offsets.Add(cols - 1);
            offsets.Add(cols);
            offsets.Add(cols + 1);

            InitCells();
        }

        /// <summary>
        /// 开始游戏并随机布雷
        /// </summary>
        /// <remarks>
        /// 第一次被点击的方格不会触雷。布雷过程后，被点击的方格要么是空的要么被设置成一定的数字
        /// </remarks>
        /// <param name="firstClickedCellIndex">第一次点击的方格在雷区的索引</param>
        public void StartGame(ushort firstClickedCellIndex)
        {
            //洗牌算法
            Shuffle(Cells, firstClickedCellIndex);

            //再设置方格周围的雷数
            foreach (Cell minecell in Cells.Where(p => p.IsMine))
            {
                foreach (Cell item1 in GetAroundCells(minecell))
                {
                    if (item1.IsMine || item1.AroundMineNum > 0)
                    {
                        item1.IsBlank = false;
                        continue;
                    }

                    foreach (Cell item2 in GetAroundCells(item1))
                    {
                        if (item2.IsMine)
                        {
                            item1.AroundMineNum++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 打开一个指定的方格。参数 <paramref name="openAroundCellWhenBlank"/> 指示被挖的方格如果是空格，是否挖开周围连通的空白方格。默认值为 <see langword="false"/>
        /// </summary>
        public GameStatus OpenCell(Cell pressedCell, bool openAroundCellWhenBlank = false)
        {
            if (pressedCell == null || pressedCell.IsOpened || pressedCell.CellImage == CellImage.Flag)
            {
                return GameStatus.Continue;
            }

            if (pressedCell.IsMine)//方格是雷
            {
                pressedCell.IsOpened = true;
                pressedCell.CellImage = CellImage.StepOnMine;
                GameOver(false);

                return GameStatus.Fail;
            }

            if (!pressedCell.IsOpened)
            {
                pressedCell.IsOpened = true;

                if (pressedCell.AroundMineNum > 0)
                {
                    pressedCell.CellImage = (CellImage)pressedCell.AroundMineNum;
                }
                else if (pressedCell.IsBlank)
                {
                    pressedCell.CellImage = CellImage.Blank;

                    if (openAroundCellWhenBlank)
                    {
                        FloodFill(Cells, pressedCell.Index);
                    }
                }

                GameStatus status = CheckGame(); //方格挖开后判断
                if (status == GameStatus.Win)
                {
                    return GameStatus.Win;
                }
            }

            return GameStatus.Continue;
        }

        public GameStatus OpenAroundCellsByFlaged(Cell pressedCell)
        {
            if (pressedCell == null)
            {
                return GameStatus.Continue;
            }

            IList<Cell> cells = GetAroundCells(pressedCell);
            if (pressedCell.AroundMineNum != cells.Count(P => P.CellImage == CellImage.Flag))
            {
                return GameStatus.Continue;
            }

            foreach (Cell cell in cells)
            {
                if (cell.IsOpened)
                {
                    continue;
                }

                if (cell.IsMine)
                {
                    if (cell.CellImage != CellImage.Flag)
                    {
                        cell.IsOpened = true;
                        cell.CellImage = CellImage.StepOnMine;
                        GameOver(false);

                        return GameStatus.Fail;
                    }
                }
                else
                {
                    return OpenCell(cell, true);
                }
            }

            return GameStatus.Continue;
        }

        /// <summary>
        /// 更新按下的方格的状态
        /// </summary>
        public void UpdatePressedCell(Cell selected)
        {
            foreach (Cell item in pressedCells)
            {
                if (item.IsOpened)
                {
                    continue;
                }

                if (item.CellImage == CellImage.QuestionClicked)
                {
                    item.CellImage = CellImage.Question;
                }
                else if (item.CellImage == CellImage.Blank)
                {
                    item.CellImage = CellImage.NotOpened;
                }
            }
            pressedCells.Clear();

            if (selected == null)
            {
                return;
            }

            if (Mouse.LeftButton == MouseButtonState.Pressed && Mouse.RightButton == MouseButtonState.Pressed)
            {
                IList<Cell> cells = GetAroundCells(selected);
                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i].CellImage == CellImage.Question)
                    {
                        cells[i].CellImage = CellImage.QuestionClicked;
                        pressedCells.Add(cells[i]);
                    }
                    else if (cells[i].CellImage == CellImage.NotOpened)
                    {
                        cells[i].CellImage = CellImage.Blank;
                        pressedCells.Add(cells[i]);
                    }
                }
            }
            else
            {
                if (selected.CellImage == CellImage.Flag)
                {
                    return;
                }

                if (selected.CellImage == CellImage.NotOpened)
                {
                    selected.CellImage = CellImage.Blank;
                }
                else if (selected.CellImage == CellImage.Question)
                {
                    selected.CellImage = CellImage.QuestionClicked;
                }

                pressedCells.Add(selected);
            }
        }

        private void InitCells()
        {
            //以下两个方案效果不同
            //不开线程，数据量小，显示快；数据量大,加载时界面卡顿
            //开线程，不管数据量大小，界面不卡顿，但加载需要花时间

            //方案一
            //while (Cells.Count < _maxcells)//当雷区所需的总方块数量多余绑定的方块列表总数量，则缺少多少方块的就从方块池里拿。如果方块池方块数量不够则会创建
            //{
            //    Cells.Add(CellPool.GetCell(Convert.ToUInt16(Cells.Count)));
            //}
            //for (int i = _maxcells; _maxcells < Cells.Count; i = Cells.Count - _maxcells)//当雷区所需的总方块数量小于当前绑定的方块列表总数量，则将多余的方块返回给方块池。
            //{
            //    CellPool.ReturnCell(Cells[Cells.Count - i]);
            //    Cells.RemoveAt(Cells.Count - i);
            //}

            //foreach (Cell cell in Cells)
            //{
            //    cell.Init();
            //}

            System.Threading.ThreadPool.QueueUserWorkItem(state =>
            {
                while (Cells.Count < _maxcells)//当雷区所需的总方块数量多余绑定的方块列表总数量，则缺少多少方块的就从方块池里拿。如果方块池方块数量不够则会创建
                {
                    MvvmLight.Threading.DispatcherHelper.UIDispatcher.Invoke(() =>
                    {
                        Cells.Add(CellPool.GetCell(Convert.ToUInt16(Cells.Count)));
                    });
                }
                for (int i = _maxcells; _maxcells < Cells.Count; i = Cells.Count - _maxcells)//当雷区所需的总方块数量小于当前绑定的方块列表总数量，则将多余的方块返回给方块池。
                {
                    CellPool.ReturnCell(Cells[Cells.Count - i]);
                    MvvmLight.Threading.DispatcherHelper.UIDispatcher.Invoke(() =>
                    {
                        Cells.RemoveAt(Cells.Count - i);
                    });
                }

                foreach (Cell cell in Cells)
                {
                    cell.Init();
                }
            });

            //方案二
            //ThreadPool.QueueUserWorkItem(state =>
            //{
            //    foreach (var num in Enumerable.Range(1, CustomRows * CustomCols))
            //    {
            //        _service.GetDispatcher().BeginInvoke(() =>
            //        {
            //            Cells.Add(new Cell(num) { CellImage = CellImage.NotOpened });
            //        }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //    }
            //});
            //await Task.Run(() =>
            //{
            //    foreach (var num in Enumerable.Range(1, CustomRows * CustomCols))
            //    {
            //        _service.GetDispatcher().BeginInvoke(() =>
            //        {
            //            Cells.Add(new Cell(num) { CellImage = CellImage.NotOpened });
            //        }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //    }
            //});
        }

        /// <summary>
        /// 当前游戏点击方格后，判断游戏结果是胜利还是失败或者继续
        /// </summary>
        private GameStatus CheckGame()
        {
            if (_maxcells - MinesCount == Cells.Where(p => p.IsOpened).Count())
            {
                GameOver(true);
                return GameStatus.Win;
            }

            return GameStatus.Continue;
        }

        /// <summary>
        /// 游戏胜利或失败
        /// </summary>
        /// <remarks>
        /// 若参数 <paramref name="winOrlose"/> 为 <see langword="true"/> 时，游戏胜利；反之失败
        /// </remarks>
        private void GameOver(bool winOrlose)
        {
            foreach (Cell item in Cells.Where(p => p.IsMine || p.CellImage == CellImage.Flag))
            {
                if (item.IsOpened)
                {
                    continue;
                }

                if (winOrlose)
                {
                    if (item.IsMine)
                    {
                        item.CellImage = CellImage.Flag;
                    }
                }
                else
                {
                    if (item.IsMine)
                    {
                        if(item.CellImage != CellImage.Flag)
                        {
                            item.IsOpened = true;
                            item.CellImage = CellImage.NotMarkedMine;
                        }
                    }
                    else
                    {
                        item.IsOpened = true;
                        if (item.CellImage == CellImage.Flag)
                        {
                            item.CellImage = CellImage.MarkedErrorMine;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前方格的周围方格
        /// </summary>
        /// <remarks>
        /// 若此格在中腹，则周围有8格；若在边上，则有5格；若在角上，则有3格
        /// </remarks>
        /// <param name="cell">当前方格</param>
        private IList<Cell> GetAroundCells(Cell cell)
        {
            IList<Cell> result = new List<Cell>();
            for (int i = 0; i < offsets.Count; i++)
            {
                int offset = cell.Index + offsets[i];
                int offsetRow = i / 3;
                //要查找的方格位置必须在偏移范围内和雷区范围内，同时判断其与相邻方格是否处于同一行
                if (offset >= 0 && offset < _maxcells
                    && offset / _cols == (int)Math.Floor(((float)offsets[offsetRow * 3 + 1] + cell.Index) / _cols))
                {
                    result.Add(Cells[offset]);
                }
            }

            return result;
        }

        /// <summary>
        /// 洗牌算法
        /// </summary>
        private void Shuffle(IList<Cell> list, int exceptIndex)
        {
            IList<int> result = Enumerable.Range(0, list.Count).ToList();
            result.Remove(exceptIndex);
            foreach (int index in result.Skip(result.Count - MinesCount))//设置最后几个方格(与雷数数量相同)埋雷
            {
                list[index].IsBlank = false;
                list[index].IsMine = true;
            }

            for (int i = list.Count - 1; i > 0; i--)
            {
                int _index = RecordParser.GenerateRandom(0, i);
                if (exceptIndex == i || _index == exceptIndex)
                {
                    continue;
                }

                bool ismine = list[i].IsMine;
                bool isblank = list[i].IsBlank;

                list[i].IsMine = list[_index].IsMine;
                list[i].IsBlank = list[_index].IsBlank;
                list[_index].IsMine = ismine;
                list[_index].IsBlank = isblank;
            }
        }

        /// <summary>
        /// 泛洪算法（八邻域，基于栈）
        /// </summary>
        /// <remarks>
        /// 连带出所有连通的空白方块以及相邻的数字方块
        /// </remarks>
        /// <param name="cells">雷区方块集合</param>
        /// <param name="cellIndex">八邻域的中心方块索引</param>
        private void FloodFill(IList<Cell> cells, int cellIndex)
        {
            floodFillStack.Clear();

            void CheckConnectedComponent(IList<Cell> cells, int _cellIndex, int _centerFixedValue)
            {
                if (_cellIndex >= 0 && _cellIndex < _maxcells
                    && (Math.Pow(_centerFixedValue / _cols - _cellIndex / _cols, 2) + Math.Pow(_centerFixedValue % _cols - _cellIndex % _cols, 2)) <= 2 //八邻域中的所有方块与中心方块的距离的平方均为等值(1或2)
                    && !cells[_cellIndex].IsOpened
                    && !cells[_cellIndex].IsMine
                    && cells[_cellIndex].CellImage != CellImage.Question
                    && cells[_cellIndex].CellImage != CellImage.Flag)
                {
                    cells[_cellIndex].IsOpened = true;
                    if (cells[_cellIndex].AroundMineNum > 0)
                    {
                        cells[_cellIndex].IsBlank = false;
                        cells[_cellIndex].CellImage = (CellImage)cells[_cellIndex].AroundMineNum;
                    }
                    else
                    {
                        cells[_cellIndex].CellImage = CellImage.Blank;
                        floodFillStack.Push(_cellIndex);
                    }
                }
            }

            floodFillStack.Push(cellIndex);
            while (floodFillStack.Count > 0)
            {
                int _index = floodFillStack.Pop();

                CheckConnectedComponent(cells, _index - _cols - 1, _index);
                CheckConnectedComponent(cells, _index - _cols, _index);
                CheckConnectedComponent(cells, _index - _cols + 1, _index);
                CheckConnectedComponent(cells, _index - 1, _index);
                //CheckConnectedComponent(cells, _index, _index);
                CheckConnectedComponent(cells, _index + 1, _index);
                CheckConnectedComponent(cells, _index + _cols - 1, _index);
                CheckConnectedComponent(cells, _index + _cols, _index);
                CheckConnectedComponent(cells, _index + _cols + 1, _index);
            }
        }
    }
}
