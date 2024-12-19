using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class DatetimeToStringConverter : IValueConverter
    {
        private static DatetimeToStringConverter _instance;
        public static DatetimeToStringConverter Instance => _instance ??= new DatetimeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value is not DateTime || parameter == null)
            {
                return "0s";
            }

            DateTime dateTime = (DateTime)value;
            string @para = parameter.ToString();
            if (@para == "Tip")
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return dateTime.ToString("yyyy/MM/dd");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
