using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Services;
using OrdoMill.ViewModel;
using PropertyChanged;

namespace OrdoMill.Views.Home;

[AddINotifyPropertyChangedInterface]
public sealed class MainViewModel : ViewModelWithDialogs
{
    public MainViewModel()
    {
        LicenseDialog = new DialogsFormatter<LicenseView, LicenseViewModel>(MetroDialogColorScheme.Theme);
        LoginDialog = new DialogsFormatter<LoginView, LoginViewModel>(MetroDialogColorScheme.Theme);
        ChangeLngCommand = new RelayCommand(ChangeLang);
        OpenInfoFlyoutCommand = new RelayCommand(() => IsInfoFlyoutOpen = true);
        OpenChangeThemeFlyout = new RelayCommand(() => IsChngThemeFlyoutOpen = true);
        ExiteCommand = new RelayCommand(() => Environment.Exit(0));
        var a = InitializeAsync();
   }
    private async Task InitializeAsync()
    {
        await Locator.StaticServices.NavigationEx(typeof(Home));
        if (!Debugger.IsAttached)
        {
           // await LoginDialog.ShowDialogIfNecessaries();
          //  await LicenseDialog.ShowDialogIfNecessaries();
        }
    }
    public DialogsFormatter<LicenseView, LicenseViewModel> LicenseDialog { get; set; }
    public DialogsFormatter<LoginView, LoginViewModel> LoginDialog { get; set; }
    public UserControl CurrentView { get; set; }
    public static ViewModelLocator Locator => Application.Current.Resources["Locator"] as ViewModelLocator;

    public bool IsInfoFlyoutOpen { get; set; }

    public bool IsChngThemeFlyoutOpen { get; set; }

    private static void ChangeLang()
    {
        try
        {
            var resourceDictionary =
                 Application.Current.Resources.MergedDictionaries.FirstOrDefault(r => r.Source.OriginalString.Contains("Global")) ?? new ResourceDictionary { Source = new Uri("/Resources/GlobalFr.xaml", UriKind.Relative) };
            resourceDictionary.Source = resourceDictionary.Source.OriginalString.Contains("Fr") ?
                new Uri("/Resources/GlobalAr.xaml", UriKind.Relative) :
                new Uri("/Resources/GlobalFr.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
        catch (Exception)
        {
            // ex.ToString().ToMsg();
        }
    }




    public RelayCommand ChangeLngCommand { get; set; }

    public RelayCommand OpenInfoFlyoutCommand { get; set; }

    public RelayCommand OpenChangeThemeFlyout { get; set; }

    public RelayCommand ExiteCommand { get; set; }

    //public string Version => Helper.GetAssemblyVersion();

}
