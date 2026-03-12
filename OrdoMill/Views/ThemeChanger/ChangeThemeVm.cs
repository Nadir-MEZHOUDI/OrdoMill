using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using OrdoMill.Properties;
using OrdoMill.Services;

namespace OrdoMill.Views.ThemeChanger;

public class ChangeThemeVm
{
    public ChangeThemeVm()
    {
        AccentColors = ThemeManager.Current.Themes
            .Where(t => !string.IsNullOrEmpty(t.ColorScheme) && t.BaseColorScheme == ThemeManager.BaseColorLight)
            .GroupBy(t => t.ColorScheme)
            .Select(g => g.First())
            .Select(
                a => new AccentColorMenuData { Name = a.DisplayName, ColorBrush = a.Resources["MahApps.Brushes.Accent"] as Brush })
            .ToList();

        AppThemes = ThemeManager.Current.Themes
            .Where(t => !string.IsNullOrEmpty(t.BaseColorScheme))
            .GroupBy(t => t.BaseColorScheme)
            .Select(g => g.First())
            .Select(
                a => new AppThemeMenuData
                {
                    Name = a.DisplayName,
                    BorderColorBrush = a.Resources["MahApps.Brushes.Black"] as Brush,
                    ColorBrush = a.Resources["MahApps.Brushes.White"] as Brush
                })
            .ToList();
    }

    public List<AccentColorMenuData> AccentColors { get; set; }

    public List<AppThemeMenuData> AppThemes { get; set; }

    public static async void ChangeTheme()
    {
        try
        {
            var theme = ThemeManager.Current.DetectTheme(Application.Current);
            if (theme != null) return;

            var appTheme = ThemeManager.Current.Themes.FirstOrDefault(t => t.DisplayName == Settings.Default.AppTheme && !string.IsNullOrEmpty(t.BaseColorScheme));
            var accent = ThemeManager.Current.Themes.FirstOrDefault(t => t.DisplayName == Settings.Default.AppAccent && !string.IsNullOrEmpty(t.ColorScheme));

            if (appTheme != null && accent != null)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, $"{appTheme.BaseColorScheme}.{accent.ColorScheme}");
            }
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
    }
}
