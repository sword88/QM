﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using QM.Core.Common;
using log4net;
using QM.Core.Exception;

namespace QM.Server
{
    public class QMUnStdDllTaskJob : IJob
    {
        private static ILog log = LogManager.GetLogger(typeof(QMUnStdDllTaskJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string taskid = context.JobDetail.Key.Name;
                var taskinfo = QMServer.CreateInstance().GetTask(taskid);
                if (taskinfo == null)
                {
                    log.Error(string.Format("当前任务信息为空,taskid：{0} - {1}", taskid, new QMException()));
                    return;       
                }
                taskinfo.unStdDllTask.TryRun();
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}",ex));
            }
        }
    }
}
