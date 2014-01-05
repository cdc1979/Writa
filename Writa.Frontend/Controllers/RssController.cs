using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Writa.Models;
using Writa.Models.Settings;
namespace Writa.Frontend.Controllers
{
    public class RssController : Controller
    {
        //
        // GET: /Rss/
        public IDataHelper _db;
        public WritaSettings setts;
        public RssController(IDataHelper d, IBlogSettingsLoader l)
        {
            _db = d;
            setts = l.LoadSettings();
        }

        public string Index()
        {


            List<SyndicationItem> sx = new List<SyndicationItem>();

            var posts = _db.GetAllPosts().Where(w=>w.PostType == WritaPostType.BLOGPOST);
                
            foreach (WritaPost pl in posts) {
                var id = pl.PostSlug;
                var title = String.Format("[{0}] {1}", id, pl.PostTitle);
                var content = pl.PostSummary;

                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/" + id;
                var url = new Uri(String.Format(baseUrl));
                DateTime d1 = pl.PostCreated;
                d1 = DateTime.SpecifyKind(d1,DateTimeKind.Local);
                DateTimeOffset d = d1;

                    sx.Add(new SyndicationItem(title, null, url) { Summary = new TextSyndicationContent(content), PublishDate = d });
    
            }


            

            string s = "";
            SyndicationFeed feed = new SyndicationFeed(sx)
            {
                Title = new TextSyndicationContent(setts.BlogTitle),
                Description = new TextSyndicationContent(setts.BlogSummary)
                //ImageUrl = new Uri("http://www.dovetailsoftware.com/images/header_logo.gif")
            };

            var output = new StringWriter();
            var writer = new XmlTextWriter(output);

            new Rss20FeedFormatter(feed).WriteTo(writer);

            Response.ContentType = "application/rss+xml";
            return output.ToString();
            
        }

    }
}
