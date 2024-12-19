using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class TitlePlayerNameConverter : IValueConverter
    {
        private static TitlePlayerNameConverter _instance;
        public static TitlePlayerNameConverter Instance => _instance ??= new TitlePlayerNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "统计信息";
            }

            return "统计信息 -- " + value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "统计信息";
        }
    }
}
