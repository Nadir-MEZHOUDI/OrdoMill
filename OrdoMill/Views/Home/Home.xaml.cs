using System.Windows.Controls;
using OrdoMill.Services;

namespace OrdoMill.Views.Home
{
    /// <summary>
    ///     Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
	{
		public Home()
		{
			InitializeComponent();
		    DataContext = this;
		}
        public string Version => Helper.GetAssemblyVersion();

    }
}