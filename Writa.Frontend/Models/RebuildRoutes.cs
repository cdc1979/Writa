using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Writa.Models;
namespace Writa.Frontend.Models
{
    public static class RebuildRoutes
    {
        public static void Rebuild(bool clearroutes, IDataHelper d)
        {
            //RouteTable.Routes.Clear();
            //RouteTable.Routes.MapHubs();

            if (!clearroutes)
            {
                RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                //route for all admin actions
                RouteTable.Routes.MapRoute(
                    name: "Writa",
                    url: "Writa/{action}/{id}",
                    defaults: new { controller = "Writa", action = "Index", id = UrlParameter.Optional }
                );

                RouteTable.Routes.MapRoute(
                    name: "Rss",
                    url: "Rss",
                    defaults: new { controller = "Rss", action = "Index", id = UrlParameter.Optional }
                );

                RouteTable.Routes.MapRoute(
                    name: "Home",
                    url: "",
                    defaults: new { controller = "Home", action = "Homepage", id = UrlParameter.Optional }
                );

                // hardcoded route for tags
                RouteTable.Routes.MapRoute(
                    name: "Tags",
                    url: "tag/{id}",
                    defaults: new { controller = "Home", action = "Tag", id = UrlParameter.Optional }
                );

                // hardcoded route to load index of posts by month and year
                RouteTable.Routes.MapRoute(
                    name: "Archives",
                    url: "archive/{year}/{month}",
                    defaults: new { controller = "Home", action = "Archives", id = UrlParameter.Optional }
                );
            }

            //load all remaining routes dynamically based on post "slug"

            //if (clearroutes)
            //{
                foreach (WritaPost w in d.GetAllPosts())
                {
                    RouteTable.Routes.Remove(RouteTable.Routes[w.PostId.ToString()]);
                }
            //}

            foreach (WritaPost w in d.GetAllPosts())
            {
                RouteTable.Routes.MapRoute(
                w.PostId.ToString(), // Route name
                w.PostSlug, // URL with parameters
                new
                {
                    controller = "Home",
                    action = "Read",
                    id = w.PostSlug
                });
            }
        }
    }
}