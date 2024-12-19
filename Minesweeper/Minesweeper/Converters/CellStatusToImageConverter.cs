using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class CellStatusToImageConverter : IValueConverter
    {
        private static CellStatusToImageConverter _instance;
        public static CellStatusToImageConverter Instance => _instance ??= new CellStatusToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !Enum.IsDefined(typeof(Model.CellImage), value))
            {
                return DependencyProperty.UnsetValue;
            }

            return (System.Windows.Media.Imaging.BitmapImage)Application.Current.Resources[value.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
