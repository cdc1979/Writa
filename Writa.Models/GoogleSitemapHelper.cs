using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Writa.Models
{
    public static class GoogleSitemapHelper
    {
        public static void WritaMap(IDataHelper d)
        {
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
            StringBuilder sb = new StringBuilder();
            StringWriter w = new StringWriter();
            SiteMapFeedGenerator gen = new SiteMapFeedGenerator(w);
            gen.WriteStartDocument();

            foreach (WritaPost p in d.GetPosts())
            {
                gen.WriteItem(baseUrl + "" + p.PostSlug, String.Format("{0:yyyy-MM-dd}", p.PostLastEdited), 1.0);
            }

            gen.WriteEndDocument();
            gen.Close();


            TextWriter tw = new StreamWriter(HttpContext.Current.Server.MapPath("~/sitemap.xml"));
            tw.WriteLine(w.ToString().Replace("utf-16", "utf-8"));
            tw.Close();
        }
    }

    public class SiteMapFeedGenerator
    {
        XmlTextWriter writer;

        public SiteMapFeedGenerator(System.IO.TextWriter w)
        {
            writer = new XmlTextWriter(w);
            //writer.Settings.Encoding = new UTF8Encoding();
            writer.Formatting = Formatting.Indented;
        }
        /// <summary>
        /// Writes the beginning of the SiteMap document
        /// </summary>
        public void WriteStartDocument()
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("urlset");

            writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
        }

        public void WriteEndDocument()
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public void Close()
        {
            writer.Flush();
            writer.Close();
        }

        public void WriteItem(string link, string publishedDate, double priority)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", link);

            writer.WriteElementString("lastmod", publishedDate);

            double newpri = Math.Round(priority, 1);
            if (newpri > 0.9)
            {
                writer.WriteElementString("changefreq", "daily");
            }
            else
            {
                writer.WriteElementString("changefreq", "weekly");
            }


            writer.WriteElementString("priority", newpri.ToString());
            writer.WriteEndElement();
        }

        public string formatDate(DateTime d)
        {
            string date = "";
            date = d.Year + "-";
            if (d.Month.ToString().Length == 1)
            {
                date += "0" + d.Month + "-";
            }
            else
            {
                date += d.Month + "-";
            }

            if (d.Day.ToString().Length == 1)
            {
                date += "0" + d.Month;
            }
            else
            {
                date += d.Day;
            }

            return date;
        }
    } 
}
