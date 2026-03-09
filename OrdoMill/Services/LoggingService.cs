using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using OrdoMill.ViewModel;
using OrdoMill.Helpers;

namespace OrdoMill.Services
{
    public static class LoggingService
    {
        public static ViewModelLocator Locator => Application.Current.Resources["Locator"] as ViewModelLocator;

        static LoggingService()
        {
            var logDirectory = Directory.GetCurrentDirectory() + "\\Logs\\";
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
             AppLogger = new ExceptionsLogger($"Logs\\ {DateTime.Now.ToString("yyyy-M-d hh-mm").ToValidFileName()} AppLog.xml") { SpecifiedName = "Application" };
        }

        private static ExceptionsLogger AppLogger { get; set; }

        public static async Task AppLoggingAsync(this Exception ex, bool send = true)
        {
            try
            {
                if (Debugger.IsAttached)
                {
                    MessageBox.Show(ex.ToString());
                }
                else
                {
                    await AppLogger.SerializeExceptionAsync(ex);
                    if (send)
                        await AppLogger.SendExceptionToMailAsync(ex);
                }
            }
            catch (Exception)
            {
                // 
            }
        }
        public static void AppLogging(this Exception ex, bool send = true)
        {
            try
            {
                if (Debugger.IsAttached)
                {
                    MessageBox.Show(ex.ToString());
                }
                else
                {
                    AppLogger.SerializeException(ex);
                    if (send)
                        AppLogger.SendExceptionToMail(ex);
                }
            }
            catch (Exception)
            {
                // 
            }
        }
    }
}
