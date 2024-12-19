using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class StringEmptyToNullConverter : IValueConverter
    {
        private static StringEmptyToNullConverter _instance;
        public static StringEmptyToNullConverter Instance => _instance ??= new StringEmptyToNullConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && (string.IsNullOrWhiteSpace(str) || !System.Text.RegularExpressions.Regex.IsMatch(str, @"^(?!0)([1-9]\d*)$", System.Text.RegularExpressions.RegexOptions.Compiled)))
            {
                return "0";
            }

            return value;
        }
    }
}
