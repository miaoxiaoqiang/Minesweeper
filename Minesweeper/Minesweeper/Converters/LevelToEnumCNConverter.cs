using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class LevelToEnumCNConverter : IValueConverter
    {
        private static LevelToEnumCNConverter _instance;
        public static LevelToEnumCNConverter Instance => _instance ??= new LevelToEnumCNConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !Enum.IsDefined(typeof(Model.GameLevel), value))
            {
                return DependencyProperty.UnsetValue;
            }

            Model.GameLevel level = (Model.GameLevel)Enum.Parse(typeof(Model.GameLevel), value.ToString());

            return level switch
            {
                Model.GameLevel.Primary => "初级",
                Model.GameLevel.Intermediate => "中级",
                Model.GameLevel.Advanced => "高级",
                Model.GameLevel.Custom => "自定义",
                _ => DependencyProperty.UnsetValue,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
