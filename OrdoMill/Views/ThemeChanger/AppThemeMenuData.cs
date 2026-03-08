using System.Windows;
using MahApps.Metro;
using OrdoMill.Properties;

namespace OrdoMill.Views.ThemeChanger
{
    public class AppThemeMenuData : AccentColorMenuData
	{
		public override void DoChangeTheme(object sender)
		{
			var theme = ThemeManager.DetectAppStyle(Application.Current);
			var appTheme = ThemeManager.GetAppTheme(Name);
			Settings.Default.AppTheme = Name;
			Settings.Default.Save();
			ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
		}
	}
}
