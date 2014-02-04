using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;
using Writa.Models.Email;
namespace Writa.EmailProviders
{
    public class SmtpEmailProvider : IEmailSend
    {
        GlobalSettings g;

        public SmtpEmailProvider(GlobalSettings settings)
        {
            g = settings;
        }

        public void SendEmail(string subject, string body, string to, string from)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(from);

            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = g.EmailServer;
            client.Send(message);
        }
    }
}
