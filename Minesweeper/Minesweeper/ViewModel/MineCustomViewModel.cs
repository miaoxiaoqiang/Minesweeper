using System;
using System.ComponentModel;

using MvvmLight;
using MvvmLight.CommandWpf;
using MvvmLight.Messaging;

namespace Minesweeper.ViewModel
{
    public sealed class MineCustomViewModel : ViewModelBase, IDataErrorInfo
    {
        private int maxRow = 0;
        private int maxCol = 0;

        private readonly System.Text.RegularExpressions.Regex _regex;

        public MineCustomViewModel()
        {
            Messenger.Default.Register<ValueTuple<double, double, double, double>>(this, "ResolutionToken", GetResolution);

            _regex = new System.Text.RegularExpressions.Regex(@"^(?!0)([1-9]\d*)$", System.Text.RegularExpressions.RegexOptions.Compiled);
            maxRow = 9;
            maxCol = 9;
            CustomRows = 9;
            CustomCols = 9;
            CustomMinesCount = 10;
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                
                if (columnName == nameof(CustomRows))
                {
                    if ((CustomRows < 9 || CustomRows > maxRow) || !_regex.IsMatch(CustomRows.ToString()))
                    {
                        result = "雷区行数范围：9~" + maxRow.ToString();
                    }
                }
                else if (columnName == nameof(CustomCols))
                {
                    if ((CustomCols < 9 || CustomCols > maxCol) || !_regex.IsMatch(CustomCols.ToString()))
                    {
                        result = "雷区行数范围：9~" + maxCol.ToString();
                    }
                }
                else if (columnName == nameof(CustomMinesCount))
                {
                    int maxccells = CustomRows * CustomCols;
                    if ((CustomMinesCount <= 0 || CustomMinesCount >= maxccells) || !_regex.IsMatch(CustomMinesCount.ToString()))
                    {
                        result = $"雷数数量取值范围：0~(行数*列数)";
                    }
                }

                return result;
            }
        }

        public string Error
        {
            get;
        }

        private int customrows;
        public int CustomRows
        {
            get => customrows;
            set
            {
                customrows = value;
                RaisePropertyChanged();
                ApplyCustomAreaCommand?.RaiseCanExecuteChanged();
            }
        }

        private int customcols;
        public int CustomCols
        {
            get => customcols;
            set
            {
                customcols = value;
                RaisePropertyChanged();
                ApplyCustomAreaCommand?.RaiseCanExecuteChanged();
            }
        }

        public int customminescount;
        public int CustomMinesCount
        {
            get => customminescount;
            set
            {
                customminescount = value;
                RaisePropertyChanged();
                ApplyCustomAreaCommand?.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand applycustomareacommand;
        public RelayCommand ApplyCustomAreaCommand
        {
            get
            {
                applycustomareacommand ??= new RelayCommand(ExecuteApplyCustomArea, CanExecuteApplyCustomArea);
                return applycustomareacommand;
            }
            set
            {
                applycustomareacommand = value;
            }
        }

        private void ExecuteApplyCustomArea()
        {
            Messenger.Default.Send("Custom", "CloseWindowToken");
            Messenger.Default.Send(ValueTuple.Create(CustomRows, CustomCols, CustomMinesCount, Model.GameLevel.Custom), "CustomMineToken");
        }

        private bool CanExecuteApplyCustomArea()
        {
            if (CustomRows < 9 || CustomRows > maxRow
                || CustomCols < 9 || CustomCols > maxCol
                || CustomMinesCount <= 0 || CustomMinesCount >= CustomRows * CustomCols) //雷数：列数X行数X4/5
            {
                return false;
            }

            return true;
        }

        private void GetResolution(ValueTuple<double, double, double, double> data)
        {
            maxRow = Convert.ToInt32((data.Item2 - data.Item4 + 9 * 24) / 24); //为什么要加9 * 24？因为获得窗体实际高度后，雷区容器已布置完毕。行数为9，列数为9，方格大小为24 * 24
            maxCol = Convert.ToInt32((data.Item1 -  26) / 24) - 1;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            Messenger.Default.Unregister(this);
        }
    }
}
