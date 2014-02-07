using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Writa;
using Writa.Models;
using Writa.Models.Email;
using Writa.Models.Settings;
using Writa.Data;
using Writa.Frontend.Models;
using Writa.EmailProviders;

namespace Writa.Frontend
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public static GlobalSettings GlobalSettings = new GlobalSettings();
        public static ContainerBuilder builder = new ContainerBuilder();
        public static IContainer container;

        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RegisterViewEngines(ViewEngines.Engines);
            Inject();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configuration.EnsureInitialized();

            
        }

        public static void Inject()
        {
            var configbuilder = new ContainerBuilder();
            configbuilder.RegisterType<FileSettingsLoader>().As<ISettingsLoader>();
            var configcontainer = configbuilder.Build();
            ISettingsLoader i = configcontainer.Resolve<ISettingsLoader>();
            GlobalSettings = i.LoadSettings();

            builder.RegisterInstance(i).As<ISettingsLoader>();

            if (GlobalSettings.BlogDb == DbType.MONGODB)
            {
                builder.RegisterInstance(new MongoDbDataHelper(GlobalSettings)).As<IDataHelper, IBlogSettingsLoader>();
            }
            else if (GlobalSettings.BlogDb == DbType.EF)
            {
                builder.RegisterInstance(new EfDataHelper(GlobalSettings)).As<IDataHelper, IBlogSettingsLoader>();
            }
            else if (GlobalSettings.BlogDb == DbType.RAVENDB)
            {
                builder.RegisterInstance(new RavenDataHelper(GlobalSettings, HttpContext.Current.Server.MapPath(GlobalSettings.LocalDbPath))).As<IDataHelper, IBlogSettingsLoader>().SingleInstance();
            }

            if (GlobalSettings.BlogEmailMethod == EmailType.SMTP)
            {
                builder.RegisterInstance(new SmtpEmailProvider(GlobalSettings)).As<IEmailSend>();
            }
            else if (GlobalSettings.BlogEmailMethod == EmailType.API)
            {
                builder.RegisterInstance(new ApiEmailProvider(GlobalSettings)).As<IEmailSend>();
            }


            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterSource(new ViewRegistrationSource());
            container = builder.Build();

            // get dbhelper.

                IDataHelper h = container.Resolve<IDataHelper>();
                
                var installresult = h.CheckInstall(GlobalSettings);
                if (installresult)
                {
                    RouteConfig.RegisterRoutes(RouteTable.Routes, h);

                    IBlogSettingsLoader b = container.Resolve<IBlogSettingsLoader>();
                    GenerateVE(b.LoadSettings().BlogTheme);


                    // check for enabled plugins.

                    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                    var resolver = new AutofacWebApiDependencyResolver(container);
                    GlobalConfiguration.Configuration.DependencyResolver = resolver;
                }
                else
                {

                }
                //install
        }
        // this allows us to reset the theme
        public static void GenerateVE(string theme)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new WritaViewEngine(theme));
        }

    }
}