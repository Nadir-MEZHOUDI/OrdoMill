using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.ViewModel;
using OrdoMill.Views.DbConnector;
using OrdoMill.Views.ThemeChanger;
using PropertyChanged;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Views.Home
{
    [ImplementPropertyChanged]
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
                await DispatcherHelper.RunAsync(CheckDb);
                ViewModelLocator.Instance.MainView.Show();
                Messenger.Default.Send<MetroWindow>(ViewModelLocator.Instance.MainView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            Close();
        }

        private static void CheckDb()   
        {
            try
            {
                while (Settings.Default.ConnectionString.IsNullOrEmpty() || !new DbCon(Settings.Default.ConnectionString).Database.Exists())
                    new DbConnectorView().ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        } 

        private async void SplashScreen_OnContentRendered(object sender, EventArgs e)
        {
            await Task.Delay(1000);
            await LoadForms();
        }
    }
}