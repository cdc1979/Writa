using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Writa.Frontend.Models
{
    public class WritaViewEngine : RazorViewEngine
    {
        protected string SelectedTheme { get; set; }

        public WritaViewEngine(string s) : base()
        {
            SelectedTheme = s;
            ViewLocationFormats = new string[] {
            "~/Themes/"+s+"/Templates/{0}.cshtml",
            "~/Themes/Default/Templates/{0}.cshtml",
            "~/Themes/"+s+"/Templates/{0}.vbhtml"
        };
            PartialViewLocationFormats =  new string[] {
            "~/Themes/"+s+"/Partials/{0}.cshtml",
            "~/Themes/Default/Partials/{0}.cshtml",
            "~/Themes/"+s+"/Partials/{0}.vbhtml"
        };
            MasterLocationFormats = new string[] {
            "~/Themes/"+s+"/Masterpage/Masterpage.cshtml","~/Themes/Default/Masterpage/Masterpage.cshtml"

        };

            
        }
                protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
                {
                    return base.CreateView(controllerContext, viewPath, masterPath);
                }

                protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
                {
                    return base.CreatePartialView(controllerContext, partialPath);
                }

                public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
                {
                    return base.FindView(controllerContext, viewName, masterName, false);
                }
        
    }
}