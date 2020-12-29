using log4net;
using Search.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Search.API.Implementation
{
    public class Logger : ILogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly log4net.ILog _log;

        public Logger()
        {
            _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public Logger(string name)
        {
            _log = log4net.LogManager.GetLogger(name);
        }

        public Logger(Type type)
        {
            _log = log4net.LogManager.GetLogger(type);
        }

        public void Debug(object message, Exception ex = null)
        {
            if (_log.IsDebugEnabled)
            {
                if (ex == null)
                {
                    _log.Debug(message);
                }
                else
                {
                    _log.Debug(message, ex);
                }
            }
        }

        public void Info(object message, Exception ex = null)
        {
            if (_log.IsInfoEnabled)
            {
                if (ex == null)
                {
                    _log.Info(message);
                }
                else
                {
                    _log.Info(message, ex);
                }
            }
        }

        public void Warn(object message, Exception ex = null)
        {
            if (_log.IsWarnEnabled)
            {
                if (ex == null)
                {
                    _log.Warn(message);
                }
                else
                {
                    _log.Warn(message, ex);
                }
            }
        }

        public void Error(object message, Exception ex = null)
        {
            if (_log.IsErrorEnabled)
            {
                if (ex == null)
                {
                    _log.Error(message);
                }
                else
                {
                    _log.Error(message, ex);
                }
            }
        }

        public void Fatal(object message, Exception ex = null)
        {
            if (_log.IsFatalEnabled)
            {
                if (ex == null)
                {
                    _log.Fatal(message);
                }
                else
                {
                    _log.Fatal(message, ex);
                }
            }
        }

        //public void Debug(object message)
        //{
        //    Log.Debug(message);
        //}

        //public void Debug(object message, Exception exception)
        //{
        //    Log.Debug(message, exception);
        //}

        //public void Info(object message)
        //{
        //    Log.Info(message);
        //}

        //public void Info(object message, Exception exception)
        //{
        //    Log.Info(message, exception);
        //}

        //public void Warn(object message)
        //{
        //    Log.Warn(message);
        //}

        //public void Warn(object message, Exception exception)
        //{
        //    Log.Warn(message, exception);
        //}

        //public void Error(object message)
        //{
        //    Log.Error(message);
        //}

        //public void Error(object message, Exception exception)
        //{
        //    Log.Error(message, exception);
        //}

        //public void Fatal(object message)
        //{
        //    Log.Fatal(message);
        //}

        //public void Fatal(object message, Exception exception)
        //{
        //    Log.Fatal(message, exception);
        //}
    }
}