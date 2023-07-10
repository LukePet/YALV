using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace YALV.Common.Converters
{
    using YALV.Core.Domain;

    public class LevelToSolidColorConverter
        : IValueConverter
    {
        private SolidColorBrush traceColor = Application.Current.FindResource("TraceLevelColor") as SolidColorBrush;
        private SolidColorBrush debugColor = Application.Current.FindResource("DebugLevelColor") as SolidColorBrush;
        private SolidColorBrush infoColor = Application.Current.FindResource("InfoLevelColor") as SolidColorBrush;
        private SolidColorBrush warnColor = Application.Current.FindResource("WarnLevelColor") as SolidColorBrush;
        private SolidColorBrush errorColor = Application.Current.FindResource("ErrorLevelColor") as SolidColorBrush;
        private SolidColorBrush fatalColor = Application.Current.FindResource("FatalLevelColor") as SolidColorBrush;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return Brushes.Transparent;

            int levelIndex = (int)value;
            switch (levelIndex)
            {
                case (int)LevelIndex.TRACE:
                    return traceColor ?? Brushes.Transparent;
                case (int)LevelIndex.DEBUG:
                    return debugColor ?? Brushes.Transparent;
                case (int)LevelIndex.INFO:
                    return infoColor ?? Brushes.Transparent;
                case (int)LevelIndex.WARN:
                    return warnColor ?? Brushes.Transparent;
                case (int)LevelIndex.ERROR:
                    return errorColor ?? Brushes.Transparent;
                case (int)LevelIndex.FATAL:
                    return fatalColor ?? Brushes.Transparent;
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
