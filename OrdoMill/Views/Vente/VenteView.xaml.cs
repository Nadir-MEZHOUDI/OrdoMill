using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrdoMill.Views.Vente;

/// <summary>
///     Interaction logic for VentView.xaml
/// </summary>
public partial class VenteView
{
    public VenteView()
    {
        InitializeComponent();
    }
    void MyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Keyboard.Focus(SearchMedText);
        SearchMedText.Focus();
        SearchMedText.CaretIndex = SearchMedText?.Text?.Length ?? 0;
     }

    private void MedicamentsGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Down || e.Key == Key.Up)
        {
            Keyboard.Focus(SearchMedText);
            SearchMedText.Focus();
            SearchMedText.CaretIndex = SearchMedText?.Text?.Length ?? 0;
            e.Handled = true;
        }
    }

}