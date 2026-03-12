using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Services;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.About;

[AddINotifyPropertyChangedInterface]
	public sealed class  ContactUsViewModel : ObservableObject
	{
		public EmailSender Sender { get; set; }

		public IDialogCoordinator DialogCoordinator;

		public ContactUsViewModel(IDialogCoordinator dialogCoordinator)
		{
			DialogCoordinator = dialogCoordinator;
			Sender = new EmailSender("Nadirelghazali@gmail.com", "$^$o278293o4", "Nadirelghazali@hotmail.com");

			SendCommand = new RelayCommand(async () => await SendEx(), CanSend);
		}

		public async Task SendEx()
		{
			var result = await Sender.SendAsync(SubjectText, $"Sender Name: {SenderName};\n Sender Mail: {SenderEmail}; \n Message: {MessageBody}");
			await DialogCoordinator.ShowMessageAsync(this, (result ? "Termine" : "Error").GetString(), (result ? "MsgSent" : "NonSend").GetString(), MessageDialogStyle.Affirmative, Statics.MessageSettings);
		}

		public bool CanSend() => true;//!string.IsNullOrEmpty(SubjectText) && !string.IsNullOrEmpty(MessageBody);

		public RelayCommand SendCommand { get; set; }

		public string SenderEmail { get; set; }

		public string SubjectText { get; set; }

		public string SenderName { get; set; }

		public string MessageBody { get; set; }
	}