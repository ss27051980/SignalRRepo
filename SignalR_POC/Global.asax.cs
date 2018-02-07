using SignalR_POC.Component;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SignalR_POC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            SqlDependency.Start(connStr);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var notifier = new NotificationComp();
            var now = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = now;
            notifier.Register(now);
        }

        protected void Application_End()
        {
            SqlDependency.Stop(connStr);
        }
    }
}
