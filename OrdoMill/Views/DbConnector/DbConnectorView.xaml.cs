using System.ComponentModel;
using System.Windows;
using OrdoMill.Data.Model;
using OrdoMill.Properties;

namespace OrdoMill.Views.DbConnector
{
    /// <summary>
    /// Interaction logic for DbConnectorView.xaml
    /// </summary>
    public partial class DbConnectorView
    {
        public DbConnectorView()
        {
            InitializeComponent();
        }
        private void BtnClose(object sender, RoutedEventArgs e)
        {
           Close();
        }

        private void DbConnectorView_OnClosing(object sender, CancelEventArgs e)
        {
            if (!new DbCon(Settings.Default.ConnectionString).Database.Exists())
                Application.Current.Shutdown(0);
        }

    }
}
