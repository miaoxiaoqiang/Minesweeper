using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class LevelToIsCheckedConverter : IValueConverter
    {
        private static LevelToIsCheckedConverter _instance;
        public static LevelToIsCheckedConverter Instance => _instance ??= new LevelToIsCheckedConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.TryParse(value.ToString(), out Model.GameLevel level1) && Enum.TryParse(parameter.ToString(), out Model.GameLevel level2))
            {
                return level1 == level2;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
