using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Writa.Models;
using Writa.Models.Settings;
using MarkdownSharp;
namespace Writa.Frontend.Models
{
    public abstract class WritaViewPage : WebViewPage
    {
        public IBlogSettingsLoader  blogsettingsloader { get; set; }
        public IDataHelper  datahelper { get; set; }
        public IWritaPage thispage { get; set; }

        public string GetMarkDown()
        {
            var md = new MarkdownSharp.Markdown();
            return md.Transform(thispage.Post.PostContent);
        }
    }
}