namespace OrdoMill.Interfaces;

public interface IDialogContentViewModel
{
    Func<Task> HideAction { get; set; }
    Func<Task> ShowAction { get; set; }
    bool CheckHideDialogesConditions();
    bool CheckShowDialogesConditions();
    Task HideDialoge();
    Task HideDialogIfPossible();
    Task ShowDialoge();
    Task ShowDialogIfNecessaries();
}