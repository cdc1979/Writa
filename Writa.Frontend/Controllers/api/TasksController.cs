using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Writa.Frontend;
using Writa.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
namespace Writa.Frontend.Controllers.api
{
    [RoutePrefix("api/tasks")]
    public class TasksController : ApiController
    {
        IDataHelper _db;
        public TasksController(IDataHelper d)
        {
            _db = d;
        }

        [Authorize]
        [HttpPost]
        [Route("showcase")]
        public string Showcase()
        {

            var x = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);
            var thisurl = x;
            var client = new RestClient("http://writa.org");
            var request = new RestRequest("api/notify/ping", Method.GET);
            request.AddParameter("url", thisurl);
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string
            return content + "["+thisurl+"]";
        }

        [Authorize]
        [HttpPost]
        [Route("sitemap")]        
        public string RebuildSitemap()
        {

            GoogleSitemapHelper.WritaMap(_db);

            return "Done";
        }

        [Authorize]
        [HttpPost]
        [Route("restore")]
        public string BackupTask(dynamic obj)
        {
            string h = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/"+obj.filename));
            List<WritaPost> posts = JsonConvert.DeserializeObject<List<WritaPost>>(h);

            _db.DeleteAllPosts();
            System.Threading.Thread.Sleep(1000);
            foreach (WritaPost p in posts)
            {
                _db.CreatePost(p);
            }

            return obj.filename + " restored";
        }

        [Authorize]
        [HttpPost]
        [Route("backup")]
        public string BackupTask()
        {
            var f = _db.GetAllPosts();

            string h = JsonConvert.SerializeObject(f, Formatting.Indented);
            var filename = "backup_"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".json";
            System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/App_Data") + "/" + filename, h);

            return "Done";
        }

    }
}