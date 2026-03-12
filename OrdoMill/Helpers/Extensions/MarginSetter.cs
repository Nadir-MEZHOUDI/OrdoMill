using System.Windows;
using System.Windows.Controls;

namespace OrdoMill.Helpers.Extensions;

public static class MarginSetter
{
    public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
        "Margin",
        typeof(Thickness),
        typeof(MarginSetter),
        new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

    public static Thickness GetMargin(DependencyObject obj)
    {
        return (Thickness)obj.GetValue(MarginProperty);
    }

    public static void SetMargin(DependencyObject obj, Thickness value)
    {
        obj.SetValue(MarginProperty, value);
    }

    public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel)
        {
            return;
        }

        panel.Loaded += Panel_Loaded;
    }

    public static void Panel_Loaded(object sender, RoutedEventArgs e)
    {
        Panel panel = sender as Panel;
        if (panel?.Children == null)
        {
            return;
        }

        foreach (FrameworkElement fe in panel.Children.OfType<FrameworkElement>().Where(x => x.Margin == new Thickness()))
        {
            fe.Margin = GetMargin(panel);
        }
    }
}
