using System;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using OrdoMill.Properties;
using OrdoMill.Services;

namespace OrdoMill.Views.ThemeChanger
{
    public class AccentColorMenuData
    {
        private RelayCommand changeAccentCommand;

        public Brush BorderColorBrush { get; set; }

        public RelayCommand ChangeAccentCommand
        {
            get
            {
                return changeAccentCommand ??
                    (changeAccentCommand = new RelayCommand(
                    () => DoChangeTheme(null),
                    () => true))
                    ;
            }
        }

        public Brush ColorBrush { get; set; }

        public string Name { get; set; }

        public virtual async void DoChangeTheme(object sender)
        {
            try
            {
                var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);
                var accent = ThemeManager.Current.Themes.FirstOrDefault(t => t.DisplayName == Name && t.ColorScheme != null);
                if (accent != null)
                {
                    Settings.Default.AppAccent = Name;
                    Settings.Default.Save();
                    ThemeManager.Current.ChangeTheme(Application.Current, $"{currentTheme?.BaseColorScheme ?? ThemeManager.BaseColorLight}.{accent.ColorScheme}");
                }
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }
    }
}
