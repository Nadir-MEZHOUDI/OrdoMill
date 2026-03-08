using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using OrdoMill.Services;
using SmartApp.Helpers.Extensions;
using WpfBindingErrors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace OrdoMill
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App:Application
    {
        static App()
        {
            try
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                DispatcherHelper.Initialize();
                ReturnNext.RegisterReturnEvent();
               EventManager.RegisterClassHandler(typeof(ToggleSwitch), UIElement.KeyDownEvent, new KeyEventHandler(ToggleSwitch_KeyDown));
               var listener = new BindingErrorListener();
               listener.ErrorCatched += async s => await new BindingException(s).AppLoggingAsync();
             }
            catch (Exception ex)
            {
                  //ex.AppLogging();
            }
        }

        private static void ToggleSwitch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ReturnNext.MoveToNextUiElement(e);
            //Successfully moved on and marked key as handled.
            //Toggle check box since the key was handled and
            //the check box will never receive it.
            else if (e.Handled && e.Key != Key.Enter)
            {
                var cb = (ToggleSwitch)sender;
                cb.IsChecked = !cb.IsChecked;
            }
        }

        private static async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await (e.ExceptionObject as Exception).AppLoggingAsync();
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private async void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                await e.Exception.AppLoggingAsync();
                Exception ex = e.Exception;
                do
                {
                    MessageBox.Show(ex?.ToString());
                } while ((ex = ex?.InnerException) != null);
            }
            catch
            {
                // ignored
            }
        }
    }
}
