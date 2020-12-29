using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.API.Interfaces
{
    public interface ILogger
    {
        void Debug(object message, Exception exception);
        //bool IsDebugEnabled { get; }

        void Info(object message, Exception ex = null);
        void Warn(object message, Exception ex = null);

        void Error(object message, Exception ex = null);

        void Fatal(object message, Exception ex = null);

        // continue for all methods like Error, Fatal ...
    }
}
