using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class FaceStatusToImageConverter : IValueConverter
    {
        private static FaceStatusToImageConverter _instance;
        public static FaceStatusToImageConverter Instance => _instance ??= new FaceStatusToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !Enum.IsDefined(typeof(Model.FaceStatus), value))
            {
                return DependencyProperty.UnsetValue;
            }

            if (parameter == null)
            {
                return (System.Windows.Media.Imaging.BitmapImage)Application.Current.Resources[value.ToString()];
            }

            return (System.Windows.Media.Imaging.BitmapImage)Application.Current.Resources[value.ToString() + parameter.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
