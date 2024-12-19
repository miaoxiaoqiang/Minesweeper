using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class WinRateConverter : IMultiValueConverter
    {
        private static WinRateConverter _instance;
        public static WinRateConverter Instance => _instance ??= new WinRateConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null
                || values.Length != 2
                || values[0] == null
                || values[1] == null
                || !System.Text.RegularExpressions.Regex.IsMatch(values[0].ToString(), @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled)
                || !System.Text.RegularExpressions.Regex.IsMatch(values[1].ToString(), @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled))
            {
                return "0%";
            }

            int rounds = System.Convert.ToInt32(values[0]);
            if (rounds <= 0)
            {
                return "0%";
            }

            int won = System.Convert.ToInt32(values[1]);
            return (won / (double)rounds).ToString("P1");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
