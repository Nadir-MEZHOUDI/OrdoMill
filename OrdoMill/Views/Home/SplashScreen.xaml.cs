using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.ViewModel;
using OrdoMill.Views.DbConnector;
using OrdoMill.Views.ThemeChanger;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.Home
{
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
              await  Dispatcher.InvokeAsync(CheckDb);
                ViewModelLocator.Instance.MainView.Show();
                WeakReferenceMessenger.Default.Send<MetroWindow>(ViewModelLocator.Instance.MainView);
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
                // Check if the connection string is empty or if the database cannot connect, show the DbConnectorView dialog
                if (Settings.Default.ConnectionString.IsNullOrEmpty() || !new DbCon(Settings.Default.ConnectionString).Database.CanConnect())
                    new DbConnectorView().ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        } 

        private async void SplashScreen_OnContentRendered(object sender, EventArgs e)
        {
            await LoadForms();
        }
    }
}
