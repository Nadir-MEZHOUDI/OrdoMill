using System.Windows;
using System.Windows.Controls;

namespace SmartApp.Helpers.Extensions
{
    public class SelecteAll : DependencyObject
    {
        public static readonly DependencyProperty SelecteProperty = DependencyProperty.RegisterAttached(
            "Selecte",
            typeof(string),
            typeof(SelecteAll),
            new PropertyMetadata(string.Empty, SelecteChanged));

        public static string GetSelecte(DependencyObject obj)
        {
            return (string)obj.GetValue(SelecteProperty);
        }

        public static void SetSelecte(DependencyObject obj, string value)
        {
            obj.SetValue(SelecteProperty, value);
        }

        private static void SelecteChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is not TextBox textBox)
            {
                return;
            }

            bool b = bool.Parse(args.NewValue.ToString());
            if (b)
            {
                textBox.SelectAll();
            }
            else
            {
                textBox.Select(0, 0);
            }
        }
    }
}
