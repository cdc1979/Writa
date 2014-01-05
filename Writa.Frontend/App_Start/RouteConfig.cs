using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Writa.Models;
using Writa.Frontend.Models;
namespace Writa.Frontend
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, IDataHelper db)
        {
            RebuildRoutes.Rebuild(false, db);
        }
    }
}