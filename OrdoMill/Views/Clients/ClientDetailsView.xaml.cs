using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Helper = OrdoMill.Helpers.Helper;

namespace OrdoMill.Views.Clients;

/// <summary>
///     Interaction logic for ClientDetailsView.xaml
/// </summary>
public partial class ClientDetailsView
{
    public ClientDetailsView()
    {
        InitializeComponent();
    }

    private void cboToUpper_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var cbo = sender as ComboBox;

        var txt = cbo?.Template.FindName("PART_EditableTextBox", cbo) as TextBox;

        if (txt != null)
        {
            txt.CharacterCasing = CharacterCasing.Upper;
        }
    }

    private void ArabTextFocus_OnFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        Helper.ChangeInputLanguage("ar");
        Helper.ChangeInputLanguage("ar-SA");
        Helper.ChangeInputLanguage("ar-DZ");
    }
    private void FrenshTextFocus_OnFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        
        Helper.ChangeInputLanguage("en");
        Helper.ChangeInputLanguage("en-US");
        Helper.ChangeInputLanguage("fr");
        Helper.ChangeInputLanguage("fr-FR");

    }
    void MyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
}