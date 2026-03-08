using System.Windows;
using ControlzEx.Theming;
using OrdoMill.Properties;

namespace OrdoMill.Views.ThemeChanger
{
    public class AppThemeMenuData : AccentColorMenuData
    {
        public override void DoChangeTheme(object sender)
        {
            var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);
            var appTheme = ThemeManager.Current.Themes.FirstOrDefault(t => t.DisplayName == Name && t.IsTheme);
            if (appTheme != null)
            {
                Settings.Default.AppTheme = Name;
                Settings.Default.Save();
                ThemeManager.Current.ChangeTheme(Application.Current, $"{appTheme.BaseColorScheme}.{currentTheme?.ColorScheme ?? "Teal"}");
            }
        }
    }
}
