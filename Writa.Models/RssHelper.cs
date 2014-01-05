using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using RssToolkit;
using RssToolkit.Rss;
using Writa.Models.Settings;
namespace Writa.Models
{
    public class RssHelper
    {
        public IDataHelper _dbhelper;
        public IBlogSettingsLoader _blogsettings;

        public RssHelper(IDataHelper db, IBlogSettingsLoader b)
        {
            _dbhelper = db;
            _blogsettings = b;
        }

        public string GenerateRss(string baseurl)
        {
            RssDocument r = new RssDocument();
            RssChannel c = new RssChannel();
            c.Items = new List<RssItem>();

            foreach (WritaPost p in _dbhelper.GetPosts()) {
                c.Items.Add( new RssItem() { Link = baseurl + "/" + p.PostSlug, Title = p.PostTitle, Description = p.PostSummary  } );
            }

            r.Channel = c;
            r.Version = "2.0";

            string doc = r.ToXml(DocumentType.Atom);
            XmlDocument document = new XmlDocument();
            document.LoadXml(doc);
            //context.Response.ContentType = "text/xml";
            document.Save( HttpContext.Current.Server.MapPath("~/atom.rss"));
            return doc;
        }
    }
}
