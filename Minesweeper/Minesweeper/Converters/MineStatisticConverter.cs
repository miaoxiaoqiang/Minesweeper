using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class MineStatisticConverter : IMultiValueConverter
    {
        private static MineStatisticConverter _instance;
        public static MineStatisticConverter Instance => _instance ??= new MineStatisticConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null
                || values.Length != 3
                || values[0] == null
                || values[1] == null
                || values[2] == null
                || !System.Text.RegularExpressions.Regex.IsMatch(values[0].ToString(), @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled)
                || !System.Text.RegularExpressions.Regex.IsMatch(values[1].ToString(), @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled)
                || !System.Text.RegularExpressions.Regex.IsMatch(values[2].ToString(), @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled))
            {
                return "000";
            }

            int length = System.Convert.ToInt32(values[2]).ToString().Length;
            int temp = System.Convert.ToInt32(values[0]) - System.Convert.ToInt32(values[1]);

            return temp > -1 ? temp.ToString("D" + (length + 1).ToString()) : temp.ToString("D" + length.ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
