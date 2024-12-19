using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    public sealed class TabItemBorderThicknessConverter : IMultiValueConverter
    {
        private static TabItemBorderThicknessConverter _instance;
        public static TabItemBorderThicknessConverter Instance => _instance ??= new TabItemBorderThicknessConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null
                || values.Length != 2
                || !Enum.IsDefined(typeof(Model.GameLevel), values[0])
                || values[1] is not Dictionary<Model.GameLevel, Model.Record>.KeyCollection)
            {
                return new Thickness(0.4, 0.4, 0.4, 0);
            }

            Model.GameLevel level = (Model.GameLevel)Enum.Parse(typeof(Model.GameLevel), values[0].ToString());

            var keys = values[1] as Dictionary<Model.GameLevel, Model.Record>.KeyCollection;
            int index = keys.ToList().IndexOf(level);
            if (index < 0)
            {
                return new Thickness(0.4, 0.4, 0.4, 0);
            }
            else
            {
                return index switch
                {
                    0 => new Thickness(0.4, 0.4, 0.4, 0),
                    1 => new Thickness(0, 0.4, 0.4, 0),
                    2 => new Thickness(0, 0.4, 0.4, 0),
                    3 => new Thickness(0, 0.4, 0.4, 0),
                    _ => (object)new Thickness(0.4, 0.4, 0.4, 0),
                };
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
