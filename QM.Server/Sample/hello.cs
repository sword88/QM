using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Server;
using Quartz;
using Quartz.Impl;
using Quartz.Job;
using log4net;

namespace QM.Server.Sample
{
    class hello : IJob
    {
        private ILog log = LogManager.GetLogger("123");

        public void Execute(IJobExecutionContext context)
        {
            log.Debug("123run");
        }
    }
}