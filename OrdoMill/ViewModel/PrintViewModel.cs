using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using OrdoMill.Services;
using SmartApp.Helpers.Bases;
using static OrdoMill.ViewModel.PrintHelper;

namespace OrdoMill.ViewModel
{
    public class PrintViewModel : IView
	{
		public PrintViewModel()
		{
			PrintCommand = new RelayCommand<Grid>(visual => Print(visual));
			QPrintCommand = new RelayCommand<Grid>(visual => QPrint(visual));
		}

		public RelayCommand<Grid> PrintCommand { get; set; }
		public RelayCommand<Grid> QPrintCommand { get; set; }

		public string Title => "Aperçu".GetString();
	}
}