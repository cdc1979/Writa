using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Writa.Models;
using Writa.Models.Settings;
namespace Writa.Frontend.Controllers.api
{
    [RoutePrefix("api/settings")]
    public class SettingsController : ApiController
    {
        public IDataHelper _dbhelper;
        public IBlogSettingsLoader _blogsettings;
        public SettingsController(IDataHelper d, IBlogSettingsLoader s)
        {
            _blogsettings = s;
            _dbhelper = d;
        }

        [Route("save")]
        [HttpPost]
        [Authorize]
        public WritaSettings SaveSettings(WritaSettings w) {
            _blogsettings.SaveSettings(w);
            MvcApplication.GenerateVE(w.BlogTheme);
            return w;
        }
    }
}