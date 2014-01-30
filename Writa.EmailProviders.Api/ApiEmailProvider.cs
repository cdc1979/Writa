using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Writa.Models.Settings;
using Writa.Models.Email;
using RestSharp;
namespace Writa.EmailProviders
{
    public class ApiEmailProvider : IEmailSend
    {
        GlobalSettings g;

        public ApiEmailProvider(GlobalSettings settings)
        {
            g = settings;
        }

        public void SendEmail(string subject, string body, string to, string from)
        {
            var client = new RestClient(g.EmailServer);
            var request = new RestRequest("api/send", Method.POST);
            request.RequestFormat = DataFormat.Json;
            var myobj = new { fromaddress = from, toaddress = to, subject = subject, body = body, webapikey = g.EmailUsername };
            request.AddBody(myobj);
            var response = client.ExecuteAsync(request, b => { });
        }
    }
}
