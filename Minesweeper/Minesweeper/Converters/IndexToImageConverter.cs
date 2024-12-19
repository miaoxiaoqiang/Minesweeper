using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class IndexToImageConverter : IValueConverter
    {
        private static IndexToImageConverter _instance;
        public static IndexToImageConverter Instance => _instance ??= new IndexToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null
                || !System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), @"^[0-9]+$", System.Text.RegularExpressions.RegexOptions.Compiled))
            {
                return DependencyProperty.UnsetValue;
            }

            int index = System.Convert.ToInt32(value);
            if(index > 5)
            {
                return (System.Windows.Media.Imaging.BitmapImage)Application.Current.Resources["Cry"];
            }
            return (System.Windows.Media.Imaging.BitmapImage)Application.Current.Resources["No" + index.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
