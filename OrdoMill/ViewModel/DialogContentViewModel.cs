using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OrdoMill.Interfaces;
using PropertyChanged;

namespace OrdoMill.ViewModel
{
    [ImplementPropertyChanged]
    public abstract class DialogContentViewModel : ViewModelBase, IDialogContentViewModel
    {
        public RelayCommand ShowDialogIfNecessariesCommand { get; set; }
        public RelayCommand HideDialogIfPossibleCommand { get; set; }
        public RelayCommand HideDialogCommand { get; set; }
        public RelayCommand ShowDialogCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }

        protected DialogContentViewModel()
        {
            ShowDialogIfNecessariesCommand = new RelayCommand(async () => await ShowDialogIfNecessaries());
            HideDialogIfPossibleCommand = new RelayCommand(async () => await HideDialogIfPossible());
            ShowDialogCommand = new RelayCommand(async () => await ShowDialoge());
            HideDialogCommand = new RelayCommand(async () => await HideDialoge());
            CloseCommand = new RelayCommand(CloseEx);
        }

        public void CloseEx()
        {
            if (!CheckHideDialogesConditions())
            {
                Application.Current.Shutdown(0);
            }
        }
        public Func<Task> ShowAction { get; set; }
        public Func<Task> HideAction { get; set; }

        public async Task ShowDialoge()
        {
             await ShowAction.Invoke();
        }

        public async Task ShowDialogIfNecessaries()
        {
            if (CheckShowDialogesConditions())
            {
                await ShowAction.Invoke();
            }
        }

        public abstract bool CheckShowDialogesConditions();


        public async Task HideDialoge()
        {
            await HideAction.Invoke();
        }

        public async Task HideDialogIfPossible()
        {
            if (CheckHideDialogesConditions())
            {
                await HideAction.Invoke();
            }
        }

        public abstract bool CheckHideDialogesConditions();
    }


}