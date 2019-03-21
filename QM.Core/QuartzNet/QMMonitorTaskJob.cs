using QM.Core.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Exception;
using QM.Core.Logisc;
using QM.Core.Mail;
using QM.Core.Model;

namespace QM.Core.QuartzNet
{
    public class QMMonitorTaskJob : IJob
    {
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(QMMonitorTaskJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Monitor monitor = new Monitor();
                monitor.TryRun();
            }
            catch (QMException ex)
            {

            }
        }
    }
}
