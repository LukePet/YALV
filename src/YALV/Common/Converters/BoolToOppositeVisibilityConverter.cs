using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YALV.Common.Converters
{
    /// <summary>
    /// Convert bool to visibility
    /// false -> Visibility.Visible
    /// true or null -> Visibility.Collapsed
    /// </summary>
    public class BoolToOppositeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return Visibility.Collapsed;

            bool oppositeValue = !(bool)value;

            if ((bool)oppositeValue)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
