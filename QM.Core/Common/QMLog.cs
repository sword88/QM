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

        public static void Info(TaskLog tlog)
        {
            log.Info(tlog.message);
        }

        public static void Debug(TaskLog tlog)
        {
            log.Debug("");
        }

        public static void Fatal(TaskLog tlog)
        {
            log.Fatal("");
        }

        public static void Error(TaskLog tlog)
        {
            log.Error("");
        }
    }
}
