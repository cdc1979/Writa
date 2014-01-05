using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Writa.Models;
namespace Writa.Frontend.Controllers.api
{
    [RoutePrefix("api/plugins")]
    public class PluginController : ApiController
    {
        IDataHelper _db;
        public PluginController(IDataHelper dv)
        {
            _db = dv;
        }

        [HttpGet]
        [Authorize]
        [Route("test")]
        public string UpdatePluginTest()
        {
            return "Done!";
        }

        [HttpGet]
        [Authorize]
        [Route("deletekey")]
        public string DeletePlugin(string sid)
        {
            _db.DeletePluginValue(sid);
            return "Done!";
        }

        [HttpGet]
        [Authorize]
        [Route("updatekey")]
        public string UpdatePlugin(string sid, string newvalue)
        {
            _db.UpdatePluginValue(sid, newvalue);
            return "Done!";
        }

    }
}