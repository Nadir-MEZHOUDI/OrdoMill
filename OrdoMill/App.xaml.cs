using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using OrdoMill.Services;
using SmartApp.Helpers.Extensions;
using WpfBindingErrors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace OrdoMill
{
    public partial class App : Application
    {
        static App()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                ReturnNext.RegisterReturnEvent();
               EventManager.RegisterClassHandler(typeof(ToggleSwitch), UIElement.KeyDownEvent, new KeyEventHandler(ToggleSwitch_KeyDown));
               var listener = new BindingErrorListener();
               listener.ErrorCatched += async s => await new BindingException(s).AppLoggingAsync();
             }
            catch (Exception)
            {
                  //ex.AppLogging();
            }
        }

        private static void ToggleSwitch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ReturnNext.MoveToNextUiElement(e);
            else if (e.Handled && e.Key != Key.Enter)
            {
                var cb = (ToggleSwitch)sender;
                cb.IsOn = !cb.IsOn;
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
