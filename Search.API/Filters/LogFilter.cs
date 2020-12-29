using Search.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
//using Search.Logging;

namespace Search.API.Filters
{
    public class LogFilter : ActionFilterAttribute
    {
        //public ILogger _logger;
        private string _elapsed;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var timer = Stopwatch.StartNew();
            actionContext.Request.Properties["logtimer"] = timer;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                var timer = ((Stopwatch)actionExecutedContext.Request.Properties["logtimer"]);
                timer.Stop();
                _elapsed = timer.ElapsedMilliseconds.ToString();
                var message = GetMessage(actionExecutedContext);
                //_logger.Info(message);
            }
        }

        private object GetMessage(HttpActionExecutedContext actionExecutedContext)
        {
            return new LogMessage
            {
                StatusCode = (int)actionExecutedContext.Response.StatusCode,
                RequestMethod = actionExecutedContext.Request.Method.Method,
                RequestUri = actionExecutedContext.Request.RequestUri.LocalPath,
                Message = actionExecutedContext.Response.StatusCode.ToString(),
                ElapsedMls = _elapsed
            }.ToString();
        }
    }
}