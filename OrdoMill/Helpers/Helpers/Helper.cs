using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OrdoMill.Helpers
{
    public static class Helper
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T typedChild)
                    {
                        yield return typedChild;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static void ChangeInputLanguage()
        {
            string currentLanguage = InputLanguageManager.Current.CurrentInputLanguage?.TwoLetterISOLanguageName;
            ChangeInputLanguage(currentLanguage == "ar" ? "fr-FR" : "ar-DZ");
        }

        public static void ChangeInputLanguage(string isoLang)
        {
            try
            {
                InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo(isoLang);
            }
            catch (Exception)
            {
            }
        }

        public static void ChangeInputLanguage(CultureInfo inputLanguage)
        {
            try
            {
                if (inputLanguage == null)
                {
                    return;
                }

                InputLanguageManager.Current.CurrentInputLanguage = inputLanguage;
            }
            catch (Exception)
            {
            }
        }
    }
}
