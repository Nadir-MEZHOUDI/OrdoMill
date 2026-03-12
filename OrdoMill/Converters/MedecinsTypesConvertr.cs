using System.Globalization;
using System.Windows.Data;
using OrdoMill.Services;

namespace OrdoMill.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class MedecinsTypesConvertr : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var a = (int)value;
        var list = "MedecinsTypes".GetArray();
        if (a >= 0)
            return list?[a] ?? value;
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}