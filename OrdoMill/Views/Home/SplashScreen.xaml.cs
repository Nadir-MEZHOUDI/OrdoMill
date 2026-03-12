using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.ViewModel;
using OrdoMill.Views.DbConnector;
using OrdoMill.Views.ThemeChanger;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.Home;

[AddINotifyPropertyChangedInterface]
public partial class SplashScreen
{
    public SplashScreen()
    {
        InitializeComponent();
        ChangeThemeVm.ChangeTheme();
    }

    private async Task LoadForms()
    {
        try
        {
            var shouldShowDbConnector = await ShouldShowDbConnectorAsync();
            if (shouldShowDbConnector)
                new DbConnectorView().ShowDialog();

            var mainView = (MetroWindow)(object)ViewModelLocator.Instance.MainView;
            mainView.Show();
            WeakReferenceMessenger.Default.Send(mainView);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        Close();
    }

    private static async Task<bool> ShouldShowDbConnectorAsync()
    {
        try
        {
            if (Settings.Default.ConnectionString.IsNullOrEmpty())
                return true;

            using var context = new DbCon(Settings.Default.ConnectionString);
            return !await context.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            return true;
        }
    }

    private async void SplashScreen_OnContentRendered(object sender, EventArgs e)
    {
        await LoadForms();
    }
}
