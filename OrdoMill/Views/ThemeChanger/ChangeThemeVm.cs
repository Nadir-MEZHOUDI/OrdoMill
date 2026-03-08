using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using OrdoMill.Properties;
using OrdoMill.Services;

namespace OrdoMill.Views.ThemeChanger
{
    public class ChangeThemeVm
    {
        public ChangeThemeVm()
        {
            AccentColors = ThemeManager.Accents
                .Select(
                    a => new AccentColorMenuData {Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush})
                .ToList();

            // create metro theme color menu items for the demo
            AppThemes = ThemeManager.AppThemes
                .Select(
                    a =>
                        new AppThemeMenuData
                        {
                            Name = a.Name,
                            BorderColorBrush = a.Resources["BlackColorBrush"] as Brush,
                            ColorBrush = a.Resources["WhiteColorBrush"] as Brush
                        })
                .ToList();
        }

        public List<AccentColorMenuData> AccentColors { get; set; }

        public List<AppThemeMenuData> AppThemes { get; set; }

        public static async void ChangeTheme()
        {
            try
            {
                var appTheme = ThemeManager.GetAppTheme(Settings.Default.AppTheme) ??
                               new AppTheme("BaseLight",
                                   new Uri(
                                       "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml"));

                var accent = ThemeManager.GetAccent(Settings.Default.AppAccent) ??
                             new Accent("teal",
                                 new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/teal.xaml"));

                ThemeManager.ChangeAppStyle(Application.Current, accent, appTheme);
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }
    }
}