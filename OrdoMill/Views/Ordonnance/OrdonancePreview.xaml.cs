using OrdoMill.Data.Model;

namespace OrdoMill.Views.Ordonnance
{
    /// <summary>
    /// Interaction logic for OrdonancePreview.xaml
    /// </summary>
    public partial class OrdonancePreview 
    {
 
        public OrdonancePreview()
        {
            InitializeComponent();
        }

        public OrdonancePreview(Data.Model.Ordonnance selectedItem) :this()
        {
            DataContext = selectedItem;
        }
    }
}
