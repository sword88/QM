using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using QM.Core.Model;

namespace QM.Core.Common
{
    public class QMLog
    {
        private static ILog log = LogManager.GetLogger(typeof(QMLog));

        private static void Info(TaskLog tlog)
        {
            log.Info("");
        }

        private static void Debug(TaskLog tlog)
        {
            log.Debug("");
        }

        private static void Fatal(TaskLog tlog)
        {
            log.Fatal("");
        }

        private static void Error(TaskLog tlog)
        {
            log.Error("");
        }
    }
}
