using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Writa.Models.Settings
{
    public class FileSettingsLoader : ISettingsLoader
    {

        public GlobalSettings LoadSettings()
        {
            string file = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/WritaSettings.json"));
            return JsonConvert.DeserializeObject<GlobalSettings>(file);
            //throw new NotImplementedException();
        }

        public GlobalSettings SaveSettings(GlobalSettings s)
        {
            string contents = JsonConvert.SerializeObject(s);
            File.WriteAllText(HttpContext.Current.Server.MapPath("~/App_Data/WritaSettings.json"), contents);
            return s;
        }
    }
}
