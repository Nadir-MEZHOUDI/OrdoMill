using System.Globalization;
using System.Windows.Data;

namespace OrdoMill.Helpers.Converters;

public class StringToDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return DateTime.Now;
        }

        string originalValue = (string)value;
        return DateTime.TryParse(originalValue, out DateTime a) ? a : DateTime.Now;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
