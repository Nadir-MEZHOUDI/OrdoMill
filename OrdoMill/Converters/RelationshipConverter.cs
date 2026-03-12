using System.Globalization;
using System.Windows.Data;
using OrdoMill.Services;

namespace OrdoMill.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class RelationshipConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<string> alliances = "Alliances".GetArray();
        if (alliances.Count > (int)value)
            return alliances[(int)value];
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}