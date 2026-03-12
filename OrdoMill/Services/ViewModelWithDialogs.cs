using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Annotations;
using PropertyChanged;

namespace OrdoMill.Services;

[AddINotifyPropertyChangedInterface]
public class ViewModelWithDialogs : ObservableObject, INotifyPropertyChanged
{       
    #region Messages Implementation
    protected async Task<MessageDialogResult> ShowMessage(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
    {
        return await DialogCoordinator.ShowMessageAsync(context, title.GetString(), message.GetString(), style, settings ?? Statics.MessageSettings);
    }

    protected async Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
    {
        return await DialogCoordinator.ShowMessageAsync(this, title.GetString(), message.GetString(), style, settings ?? Statics.MessageSettings);
    }

    protected async Task<MessageDialogResult> ShowErrorMessage(string message)
    {
        return await DialogCoordinator.ShowMessageAsync(this, "Error".GetString(), message.GetString(), MessageDialogStyle.Affirmative, Statics.ErrorMessageSettings);
    }
    protected async Task<ProgressDialogController> ShowProgressMessage(string title, string message, bool cancellable = true)
    {
        return await DialogCoordinator.ShowProgressAsync(this, title.GetString(), message.GetString(), cancellable, Statics.MessageSettings);
    }

    protected async Task ShowFinishMessage(string message, double delay = 1.5)
    {
        await ShowFinishMessage("Finish", message, delay);
    }

    protected async Task ShowFinishMessage(string title, string message, double delay = 1.5)
    {
        var dialog = new CustomDialog
        {
            Background = Application.Current.Resources["MahApps.Brushes.Accent"] as Brush,
            FontSize = 20,
            Title = title.GetString(),
            Content = message.GetString()
        };
        await DialogCoordinator.ShowMetroDialogAsync(this, dialog, Statics.ErrorMessageSettings);
        await Task.Delay(TimeSpan.FromSeconds(delay));
        await DialogCoordinator.HideMetroDialogAsync(this, dialog, Statics.MessageSettings);
    }

    protected IDialogCoordinator DialogCoordinator => MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance;

    #endregion

    public new event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
