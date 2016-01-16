using System;
using System.Windows;
using System.Windows.Data;

namespace YALV.Common.Converters
{
    /// <summary>
    /// Convert bool to visibility
    /// true -> Visibility.Visible
    /// false or null -> Visibility.Collapsed
    /// </summary>
    public class BoolToVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return Visibility.Collapsed;

            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
