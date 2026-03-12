using CommunityToolkit.Mvvm.Input;
using PropertyChanged;
using UserControl = System.Windows.Controls.UserControl;

namespace OrdoMill.Services;

[AddINotifyPropertyChangedInterface]
public class NavigableViewModel : ViewModelWithDialogs
{
    protected NavigableViewModel()
    {
        NavigationDictionary = new Dictionary<string, UserControl>();
        NavigateToCommand = new RelayCommand<Type>(async pageType => await NavigateTo(pageType));
    }
    public RelayCommand<Type> NavigateToCommand { get; set; }
    public async Task NavigateTo(Type pageType)
    {
        try
        {
            var name = pageType.Name;
            CurrentPage = NavigationDictionary.ContainsKey(name)
              ? NavigationDictionary[name]
              : (NavigationDictionary[name] = (UserControl)Activator.CreateInstance(pageType));
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
    }
    public Dictionary<string, UserControl> NavigationDictionary { get; set; }
    public UserControl CurrentPage { get; set; }
}
