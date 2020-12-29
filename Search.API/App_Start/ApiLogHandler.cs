using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using Search.API.Classes;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Diagnostics;
using Search.API.Interfaces;
using Search.API.Models;

namespace Search.API.App_Start
{
    public class ApiLogHandler : DelegatingHandler
    {
        ILogger _logger;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiLogEntry = CreateApiLogEntryWithRequestData(request);
            if (request.Content != null)
            {
                await request.Content.ReadAsStringAsync()
                    .ContinueWith(task =>
                    {
                        apiLogEntry.RequestContentBody = task.Result;
                    }, cancellationToken);
            }

            return await base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var response = task.Result;

                    // Update the API log entry with response info
                    apiLogEntry.ResponseStatusCode = (int)response.StatusCode;
                    apiLogEntry.ResponseTimestamp = DateTime.Now;

                    if (response.Content != null)
                    {
                        //  apiLogEntry.ResponseContentBody = response.Content.ReadAsStringAsync().Result;
                        apiLogEntry.ResponseContentType = response.Content.Headers.ContentType.MediaType;
                        apiLogEntry.ResponseHeaders = SerializeHeaders(response.Content.Headers);
                    }

                    // TODO: Save the API log entry to the database

                    if (apiLogEntry.ResponseStatusCode == 200)
                    {
                        apiLogEntry.ApiLogLevel = "Info";
                    }
                    else
                    {
                        apiLogEntry.ApiLogLevel = "Error";
                    }


                    //_logger.Info(JsonConvert.SerializeObject(apiLogEntry).Replace("\r\n",""));
                    
                    //if (apiLogEntry.ResponseStatusCode != 200)
                    //    _logger.Error(string.Format("{0}, {1}, {2}, {3}, {4}", apiLogEntry.ResponseTimestamp, apiLogEntry.RequestIpAddress, apiLogEntry.ResponseStatusCode, apiLogEntry.RequestMethod, apiLogEntry.RequestUri));
                    //else

                    //    _logger.Info(string.Format("{0}, {1}, {2}, {3}, {4}", apiLogEntry.ResponseTimestamp, apiLogEntry.RequestIpAddress, apiLogEntry.ResponseStatusCode, apiLogEntry.RequestMethod, apiLogEntry.RequestUri));

                    return response;
                }, cancellationToken);


        }

        private ApiLogEntry CreateApiLogEntryWithRequestData(HttpRequestMessage request)
        {
            var context = ((HttpContextBase)request.Properties["MS_HttpContext"]);
            var routeData = request.GetRouteData();

            return new ApiLogEntry
            {
                Application = "Search.API",
                User = context.User.Identity.Name,
                Machine = Environment.MachineName,
                RequestContentType = context.Request.ContentType,
                RequestRouteTemplate = routeData.Route.RouteTemplate,
                RequestRouteData = SerializeRouteData(routeData),
                RequestIpAddress = context.Request.UserHostAddress,
                RequestMethod = request.Method.Method,
                RequestHeaders = SerializeHeaders(request.Headers),
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString()
            };
        }

        private string SerializeRouteData(IHttpRouteData routeData)
        {
            return JsonConvert.SerializeObject(routeData, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            //return JsonConvert.SerializeObject(routeData, Formatting.Indented, new JsonSerializerSettings
            //{
            //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
            //});
        }

        private string SerializeHeaders(HttpHeaders headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var item in headers.ToList())
            {
                if (item.Value != null)
                {
                    var header = String.Empty;
                    foreach (var value in item.Value)
                    {
                        header += value + " ";
                    }

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(" ".ToCharArray());
                    dict.Add(item.Key, header);
                }
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }

        public ApiLogHandler(ILogger logger)
        {
            _logger = logger;
        }
    }
}