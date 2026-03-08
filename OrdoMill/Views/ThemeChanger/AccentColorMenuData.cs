using System;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro;
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
				var theme = ThemeManager.DetectAppStyle(Application.Current);
				var accent = ThemeManager.GetAccent(Name);
				Settings.Default.AppAccent = accent.Name;
				Settings.Default.Save();
				ThemeManager.ChangeAppStyle(Application.Current, accent, theme?.Item1);
			}
			catch (Exception ex)
			{
				await ex.AppLoggingAsync();
			}
		}
	}
}
