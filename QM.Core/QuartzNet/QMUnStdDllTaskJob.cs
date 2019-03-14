using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using QM.Core.Common;
using QM.Core.Log;
using QM.Core.Exception;
using QM.Core.Data;
using QM.Core.Model;
using System.Net;

namespace QM.Core.QuartzNet
{
    public class QMUnStdDllTaskJob : IJob
    {
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(QMUnStdDllTaskJob));

        public async Task Execute(IJobExecutionContext context)
        {
            string taskid = context.JobDetail.Key.Name;

            try
            {                
                var taskinfo = QMBaseServer.CreateInstance().GetTask(taskid);
                if (taskinfo == null)
                {
                    log.Error(string.Format("当前任务信息为空,taskid：{0} - {1}", taskid, new QMException()));
                    throw new QMException(string.Format("当前任务信息为空,taskid：{0} - {1}", taskid, new QMException()));
                }
                QMDBLogger.Info(taskid, QMLogLevel.Info.ToString(), "开始运行");
                QMDBLogger.UpdateLastStartTime(taskid, DateTime.Now);

                taskinfo.unStdDllTask.TryRun();
                
                QMDBLogger.UpdateLastEndTime(taskid, DateTime.Now);
                QMDBLogger.Info(taskid, QMLogLevel.Info.ToString(), "运行完成");
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}", ex));
                QMDBLogger.UpdateLastErrorTime(context.JobDetail.Key.Name, DateTime.Now);
                QMDBLogger.Info(context.JobDetail.Key.Name, QMLogLevel.Fatal.ToString(), string.Format("任务回调时发生严重错误，{0}", ex));
            }
        }
    }
}
