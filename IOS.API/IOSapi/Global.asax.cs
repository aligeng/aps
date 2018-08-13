using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IOSapi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string strDecPWd = ConfigurationManager.AppSettings["Pwd"].ToString();
            string dataType = ConfigurationManager.AppSettings["DataType"].ToString();
            string dbServer = ConfigurationManager.AppSettings["Server"].ToString();
            string dbPort = ConfigurationManager.AppSettings["Port"].ToString();
            string dbDatabase = ConfigurationManager.AppSettings["Database"].ToString();
            string dbUID = ConfigurationManager.AppSettings["UID"].ToString();
            string dbPwd = strDecPWd ?? ConfigurationManager.AppSettings["Pwd"].ToString();
            AMOData.AMODataHelper.InitDB(dataType, dbServer, dbPort, dbDatabase, dbUID, dbPwd);
            AMOData.Settings.SysSetting.Load();
        }
    }
}
