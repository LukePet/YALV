using System;
using System.Windows.Data;

namespace YALV.Common.Converters
{
    public class AdjustValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return value;

            double adjust = 0;
            if (parameter != null)
                double.TryParse(parameter as string, out adjust);

            double coord = 0;
            double.TryParse(value.ToString(), out coord);

            double res = coord + adjust;
            return res >= 0 ? res : coord;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
