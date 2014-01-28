using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Writa.Models;
using Writa.Models.Settings;
using Writa.Frontend.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Autofac;
using Autofac.Integration.Mvc;
using MarkdownSharp;
namespace Writa.Frontend.Controllers
{
    public class HomeController : Controller
    {
        /*
         * This is the only "public" controller.  It handles the processing of all the pages of the blog engine.
         */

        private IDataHelper _dtahelper;
        private IBlogSettingsLoader _blogsettings;

        public HomeController(IDataHelper d, IBlogSettingsLoader b)
        {
            _dtahelper = d;
            _blogsettings = b;
        }

        public ActionResult Homepage()
        {
            WritaPluginSetting plh = _dtahelper.GetPluginSettings("Homepage", "Homepage Number Of Posts", "10");
            int numbr = int.Parse(plh.Value);

            // get the post for the homepage
            var f = _dtahelper.GetAllPosts().Where(w => w.PostType == WritaPostType.HOMEPAGE).FirstOrDefault();
            

            WritaPage p = new WritaPage();
            p.Settings = Writa.Frontend.MvcApplication.GlobalSettings;
            p.BlogSettings = _blogsettings.LoadSettings();
            p.PageTitle = f.PostTitle;
            p.PageDescription = f.PostSummary;
            p.RelatedPosts = _dtahelper.GetPosts().Where(w => w.PostType == WritaPostType.BLOGPOST).OrderByDescending(w => w.PostCreated).Take(numbr).ToList();
            
            var updater = new ContainerBuilder();
            updater.RegisterInstance(p).As<IWritaPage>();
            updater.Update(MvcApplication.container);

            return View("Index", "Masterpage", p);
        }

        public ActionResult Archives(string year, string month)
        {
            WritaPage p = new WritaPage();

            int xyear = int.Parse(year);
            int xmonth = int.Parse(month);

            p.RelatedPosts = new List<WritaPost>();
            var f = _dtahelper.GetPosts().Where(w => w.PostType == WritaPostType.BLOGPOST).OrderByDescending(w => w.PostCreated).ToList();
            foreach (WritaPost px in f) {
                if (px.PostCreated.Year == xyear && px.PostCreated.Month == xmonth)
                {
                    p.RelatedPosts.Add(px);
                }
            }
             
            p.BlogSettings = _blogsettings.LoadSettings();
            p.PageTitle = "Posts in " + year + " and " + month;
            p.PageDescription = "This page list all posts posted in " + year;
            
            var updater = new ContainerBuilder();
            updater.RegisterInstance(p).As<IWritaPage>();
            updater.Update(MvcApplication.container);

            if (p.RelatedPosts.Count == 0)
            {
                return HttpNotFound();
            }
            else
            {
                return View("Index", "Masterpage", p);
            }
        }

        public ActionResult Tag(string id)
        {
            WritaPage p = new WritaPage();
            p.Settings = Writa.Frontend.MvcApplication.GlobalSettings;
            p.Tag = id;
            p.RelatedPosts = _dtahelper.GetAllPosts(id).ToList();
            p.BlogSettings = _blogsettings.LoadSettings();
            p.PageTitle = "Pages tagged " + id;
            p.PageDescription = "This page list all posts and pages that have been tagged as " + id;
            var updater = new ContainerBuilder();
            updater.RegisterInstance(p).As<IWritaPage>();
            updater.Update(MvcApplication.container);

            if (p.RelatedPosts.Count == 0)
            {
                return HttpNotFound();
            }
            else
            {
                return View("TagView", "Masterpage", p);
            }
        }

        public ActionResult Read(string id)
        {
            bool pagefound = false;
            
            // for testing what resources are available
            //string[] resourceNames = this.GetType().Assembly.GetManifestResourceNames();
            //foreach (string resourceName in resourceNames)
            //{
               //Response.Write(resourceName);
            //}

            WritaPage p = new WritaPage();
            p.Settings = Writa.Frontend.MvcApplication.GlobalSettings;

            WritaPost pagepost = _dtahelper.GetPostFromSlug(id);

            
            if (pagepost != null)
            {
                if (pagepost.PostRedirect != null)
                {
                    if (pagepost.PostRedirect.Length > 2)
                    {
                        Response.Redirect("~/" + pagepost.PostRedirect);
                    }
                }

                pagefound = true;
                p.Post = pagepost;
                p.PageTitle = p.Post.PostTitle;
                p.PageDescription = p.Post.PostSummary;

                p.NextPost = _dtahelper.GetNextPost(pagepost);
                p.PreviousPost = _dtahelper.GetPreviousPost(pagepost);
            }

            if (!pagefound)
            {
                return HttpNotFound();
            }
            else
            {

                p.BlogSettings = _blogsettings.LoadSettings();

                var updater = new ContainerBuilder();
                updater.RegisterInstance(p).As<IWritaPage>();
                updater.Update(MvcApplication.container);

                    if (p.Post.PostType == WritaPostType.BLOGPOST)
                    {
                        return View("Read","Masterpage", p);
                    }
                    else
                    {
                        return View("StaticPage","Masterpage", p);
                    }
            }
        }

    }
}
