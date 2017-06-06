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
using QM.Core.Environments;

namespace QM.Core.QuartzNet
{
    public class QMUnStdDllTaskJob : IJob
    {
        private static ILogger log = QMStarter.CreateQMLogger(typeof(QMUnStdDllTaskJob));

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
                Data.TaskData t = new TaskData();
                t.UpdateLastStartTime(taskid, DateTime.Now);

                taskinfo.unStdDllTask.TryRun();

                t.UpdateLastEndTime(taskid, DateTime.Now);
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}",ex));
            }
        }
    }
}
