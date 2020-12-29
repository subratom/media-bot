using Newtonsoft.Json;
using Search.API.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using Search.API.Interfaces;
using Search.API.Implementation;

namespace Search.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.MessageHandlers.Add(new ApiLogHandler(new Logger()));
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new ApiLogHandler());
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Re‌​ferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //GlobalContext.Properties["processId"] = "LoggerSimple";
            AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
