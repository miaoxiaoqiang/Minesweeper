using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class SecondsToTimeConverter : IValueConverter
    {
        private static SecondsToTimeConverter _instance;
        public static SecondsToTimeConverter Instance => _instance ??= new SecondsToTimeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if(parameter.ToString() == "Front")
                {
                    return "00:00";
                }
                else
                {
                    return "0s";
                }
            }
            //^(0|([1-9][0-9]*))(\.[\d]{1})?$  匹配非负数
            if (parameter.ToString() == "Front")
            {
                long duration = System.Convert.ToInt64(value);
                TimeSpan timespan = TimeSpan.FromSeconds(duration);
                return $"{timespan.Minutes.ToString("D2")}:{timespan.Seconds.ToString("D2")}";
            }
            else
            {
                return $"{System.Convert.ToDouble(value).ToString("0.0")}s";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
