using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using OrdoMill.ViewModel;
using PropertyChanged;

namespace OrdoMill.Services
{
    [AddINotifyPropertyChangedInterface]
    public sealed class StaticServices
    {
        public StaticServices()
        {
            PrintCommand = new RelayCommand<Grid>(visual => PrintHelper.Print(visual));
            QPrintCommand = new RelayCommand<Grid>(visual => PrintHelper.QPrint(visual));
            NavigationDictionary = new Dictionary<string, UserControl>();
            NavigateToCommand = new RelayCommand<Type>(async a => await NavigationEx(a), NavigationCanEx);
        }

        public Dictionary<string, UserControl> NavigationDictionary { get; set; }

        public UserControl CurrentView { get; set; }

        public RelayCommand<Type> NavigateToCommand { get; set; }

        //TODO Check Navigation Conditions
        public bool NavigationCanEx(Type obj) => obj != null && obj != CurrentView?.GetType();

        public async Task NavigationEx(Type pageType) 
        {
            try
            {
                var name = pageType.Name;
                if (NavigationDictionary.ContainsKey(name))
                {
                    CurrentView = NavigationDictionary[name];
                }
                else
                {
                    CurrentView = NavigationDictionary[name] = (UserControl)Activator.CreateInstance(pageType);
                }
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }
        public RelayCommand<Grid> PrintCommand { get; set; }
        public RelayCommand<Grid> QPrintCommand { get; set; }
    }
}