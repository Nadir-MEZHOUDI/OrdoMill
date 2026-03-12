using System.Windows;
using System.Windows.Controls;

namespace OrdoMill.Views.Users;

/// <summary>
/// Interaction logic for UsersView.xaml
/// </summary>
public partial class UsersView : UserControl
	{
		public UsersView()
		{
			InitializeComponent();
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			TxtFullName.Focus();
		}
	}