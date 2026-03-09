using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartApp.Helpers.Converters
{
    public class BoolInvercer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool booleanValue)
            {
                return false;
            }

            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool booleanValue)
            {
                return false;
            }

            return !booleanValue;
        }
    }
}
