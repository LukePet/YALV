using System;
using System.Windows;
using System.Windows.Data;
using YALV.Properties;

namespace YALV.Common.Converters
{
    public class TimeDeltaDoubleToLocalizedStringConverter : IValueConverter
    {


    
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return "-";
            double? delta = (double?) value;

            return String.Format(delta.Value >= 0 ? Resources.GlobalHelper_getTimeDelta_Positive_Text : Resources.GlobalHelper_getTimeDelta_Negative_Text, Math.Abs(delta.Value).ToString(System.Globalization.CultureInfo.GetCultureInfo(Properties.Resources.CultureName)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    

 
    }
}