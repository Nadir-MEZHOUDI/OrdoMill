using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace OrdoMill.Helpers.Converters
{
    public class RowToIndexConverter : MarkupExtension, IValueConverter
    {
        private static RowToIndexConverter _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataGridRow row)
            {
                return row.GetIndex() + 1;
            }

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ??= new RowToIndexConverter();
        }
    }
}
