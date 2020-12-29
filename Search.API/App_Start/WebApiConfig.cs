using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Headers;
using Search.API.App_Start;
using Newtonsoft.Json;

namespace Search.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.Add(new BrowserJsonFormatter());

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // Web API routes
            config.MapHttpAttributeRoutes();

            log4net.Config.XmlConfigurator.Configure();

            config.Filters.Add(new ValidateActionFilter());

            config.Routes.MapHttpRoute(
                name: "SearchApi",
                routeTemplate: "{controller}",
                defaults: new { controller = "SearchResults"}
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }
    }
}
