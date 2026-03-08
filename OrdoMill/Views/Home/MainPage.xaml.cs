using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Services;
using PropertyChanged;

namespace OrdoMill.Views.Home
{
    /// <summary>
    ///     Interaction logic for MainPage.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
 

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;     
            var a = await this.ShowMessageAsync("Exit", "Voulez-vous fermer l'application", MessageDialogStyle.AffirmativeAndNegative,Statics.MessageSettings);
            if (a == MessageDialogResult.Affirmative)
                Application.Current.Shutdown(); 
              //e.Cancel = a != MessageDialogResult.Affirmative;         
        }
    }
}