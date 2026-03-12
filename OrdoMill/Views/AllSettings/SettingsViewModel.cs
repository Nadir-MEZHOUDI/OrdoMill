using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrdoMill.Properties;
using OrdoMill.Services;
using OrdoMill.Views.DbConnector;
using PropertyChanged;

namespace OrdoMill.Views.AllSettings;

[AddINotifyPropertyChangedInterface]
public class SettingsViewModel : NavigableViewModel
{
    private int pageSize;
    public RelayCommand SetDbPathCommand { get; set; }

    public SettingsViewModel()
    {
        SetDbPathCommand = new RelayCommand(SetDbPathe_Excute);
        pageSize = Settings.Default.PageSize;
    }

    public static void SetDbPathe_Excute()
    {
        new DbConnectorView().Show();
    }

    public int PageSize
    {
        get { return pageSize; }
        set
        {
            pageSize = value;
            Settings.Default.PageSize = value;
            Settings.Default.Save();
            WeakReferenceMessenger.Default.Send(new Tuple<string, int>("PageSize", value));
        }
    }
}




