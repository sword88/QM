using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Exception;
using log4net;
using log4net.Core;

namespace QM.Core.Log
{
    public interface ILogger : ILog
    {
        bool IsEnabled(Level level);
        void Log(Level level, QMException exception, string format, params object[] args);
    }
}
