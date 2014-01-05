using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;

namespace Writa.Models.Email
{
    public interface IEmailSend
    {
        void SendEmail(string subject, string body, string to, string from);
    }
}
