using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SmartApp.Helpers.Helpers
{
    public class EmailSender : IDisposable
    {
        private static readonly string SenderName = Environment.MachineName;

        private MailMessage Mail { get; }

        private SmtpClient Client { get; }

        private string Sender { get; set; }

        public EmailSender(string fromMail, string pass, string toMail, string sender = "Nan")
        {
            try
            {
                Sender = sender;
                Mail = new MailMessage(fromMail, toMail);
                Client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Credentials = new NetworkCredential(fromMail, pass)
                };
            }
            catch (Exception)
            {
            }
        }

        public async Task<bool> SendAsync(string title, string body) => await SendAsync(title, body, Sender);

        public bool Send(string title, string body, string sender = null)
        {
            try
            {
                Mail.Subject = title;
                Mail.Body = $"Sender:{sender ?? SenderName}\n {body}";
                Client.Send(Mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendAsync(string title, string body, string sender)
        {
            try
            {
                Mail.Subject = title;
                Mail.Body = $"Sender:{sender}\n {body}";
                Client.SendAsync(Mail, null);
                await Task.Delay(1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            Mail.Dispose();
            Client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
