using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using QM.Core.Common;
using log4net;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Model;
using QM.Core.Data;
using System.Net;

namespace QM.Core.QuartzNet
{
    public class QMDllTaskJob : IJob
    {
        private static ILog log = LogManager.GetLogger(typeof(QMDllTaskJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string taskid = context.JobDetail.Key.Name;
                var taskinfo = QMBaseServer.CreateInstance().GetTask(taskid);
                if (taskinfo == null)
                {
                    log.Error(string.Format("当前任务信息为空,taskid：{0} - {1}", taskid, new QMException()));
                    return;       
                }

                QMDBLogger.Info(taskid, "开始运行");
                QMDBLogger.UpdateLastStartTime(taskid, DateTime.Now);

                taskinfo.dllTask.TryRun();

                QMDBLogger.UpdateLastEndTime(taskid, DateTime.Now);
                QMDBLogger.Info(taskid, "运行完成");
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}",ex));
                QMDBLogger.UpdateLastErrorTime(context.JobDetail.Key.Name, DateTime.Now);
                QMDBLogger.Info(context.JobDetail.Key.Name, string.Format("任务回调时发生严重错误，{0}", ex));
            }
        }
    }
}
