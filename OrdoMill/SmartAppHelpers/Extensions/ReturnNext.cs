using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartApp.Helpers.Extensions
{
    public static class ReturnNext
    {
        public static void RegisterReturnEvent()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
            EventManager.RegisterClassHandler(typeof(ComboBox), UIElement.KeyDownEvent, new KeyEventHandler(ComboBox_KeyDown));
            EventManager.RegisterClassHandler(typeof(CheckBox), UIElement.KeyDownEvent, new KeyEventHandler(CheckBox_KeyDown));
        }

        public static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & ((TextBox)sender).AcceptsReturn == false)
            {
                MoveToNextUiElement(e);
            }
        }

        private static void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & ((ComboBox)sender).SelectedIndex >= 0)
            {
                MoveToNextUiElement(e);
            }
        }

        public static void CheckBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MoveToNextUiElement(e);
            }
            else if (e.Handled && e.Key != Key.Enter)
            {
                CheckBox cb = (CheckBox)sender;
                cb.IsChecked = !cb.IsChecked;
            }
        }

        public static void MoveToNextUiElement(KeyEventArgs e)
        {
            var request = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus == null)
            {
                return;
            }

            e.Handled |= elementWithFocus.MoveFocus(request);
        }
    }
}
