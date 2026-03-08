using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Interfaces;
using PropertyChanged;

namespace OrdoMill.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class DialogsFormatter<TView, TViewModel> where TView : UserControl, new() where TViewModel : IDialogContentViewModel, new()
    {
        public RelayCommand ShowDialogIfNecessariesCommand { get; set; }
        public RelayCommand HideDialogIfPossibleCommand { get; set; }
        public RelayCommand HideDialogCommand { get; set; }
        public RelayCommand ShowDialogCommand { get; set; }

        public DialogsFormatter(MetroDialogColorScheme colorScheme = MetroDialogColorScheme.Accented)
        {

            Messenger.Default.Register<MetroWindow>(this, a =>
            {
                ViewModel.ShowAction = async () => await a.ShowMetroDialogAsync(Dialog);
                ViewModel.HideAction = async () => await a.HideMetroDialogAsync(Dialog);
            });

            View = new TView();
            if (Dialog == null)
                Dialog = new CustomDialog
                {
                    DialogTitleFontSize = 1,
                    DialogSettings = { ColorScheme = colorScheme },
                    Content = View
                };
            ViewModel = new TViewModel();

            View.DataContext = ViewModel;
            ShowDialogIfNecessariesCommand = new RelayCommand(async () => await ShowDialogIfNecessaries());
            HideDialogIfPossibleCommand = new RelayCommand(async () => await HideDialogIfPossible());
            ShowDialogCommand = new RelayCommand(async () => await ShowDialoge());
            HideDialogCommand = new RelayCommand(async () => await HideDialoge());
        }

        public TView View { get; set; }

        public TViewModel ViewModel { get; set; }

        public CustomDialog Dialog { get; set; }

        public bool CheckHideDialogesConditions()
        {
            return ViewModel.CheckHideDialogesConditions();
        }

        public bool CheckShowDialogesConditions()
        {
            return ViewModel.CheckShowDialogesConditions();
        }

        public async Task HideDialoge()
        {
            await ViewModel.HideDialoge();
        }

        public async Task HideDialogIfPossible()
        {
            await ViewModel.HideDialogIfPossible();
        }

        public async Task ShowDialoge()
        {
            await ViewModel.ShowDialoge();
        }

        public async Task ShowDialogIfNecessaries()
        {
            await ViewModel.ShowDialogIfNecessaries();
        }
    }
}