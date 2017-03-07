using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Model;
using Quartz;
using Quartz.Impl;
using Quartz.Job;
using log4net;

namespace QM.Server
{
    public class QMSqlFileTask : SqlFileTask
    {
        private ILog log = LogManager.GetLogger(typeof(QMSqlFileTask));

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}