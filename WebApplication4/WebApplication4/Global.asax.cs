using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication4
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger("Ticknet");

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Log4Net.config")));
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            log.Debug("++++++++++++++++++++++++++++");
            //log.Error("Exception - \n" + ex);
            log.Error("Exception Data - \n" + ex.GetType().Name);
            log.Error("Exception Data - \n" + ex.Data);
            log.Error("Exception Message - \n" + ex.Message);
            log.Debug("++++++++++++++++++++++++++++");
        }
    }
}
