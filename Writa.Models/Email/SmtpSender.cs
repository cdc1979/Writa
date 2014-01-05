using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;
namespace Writa.Models.Email 
{
    public class SmtpSender : IEmailSend
    {
        GlobalSettings g;

        public SmtpSender(ISettingsLoader settings)
        {
            g = settings.LoadSettings();
        }

        public void SendEmail(string subject, string body, string to, string from)
        {
            
        }
    }
}
