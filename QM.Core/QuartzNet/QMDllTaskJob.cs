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
using System.Runtime.ExceptionServices;

namespace QM.Core.QuartzNet
{
    public class QMDllTaskJob : IJob
    {
        private static ILog log = QMLoggerFactory.GetInstance().CreateLogger(typeof(QMDllTaskJob));

        [HandleProcessCorruptedStateExceptions]
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string taskid = context.JobDetail.Key.Name;
                var taskinfo = QMBaseServer.CreateInstance().GetTask(taskid);
                if (taskinfo == null)
                {
                    log.Error(string.Format("当前任务信息为空,taskid：{0} - {1}", taskid, new QMException()));
                    throw new QMException(string.Format("当前任务信息为空,taskid：{0} - {1}", taskid, new QMException()));
                }

                QMDBLogger.Info(taskid, QMLogLevel.Info.ToString(), "开始运行");
                QMDBLogger.UpdateLastStartTime(taskid, DateTime.Now);

                taskinfo.dllTask.TryRun();
                taskinfo.dllTask.Dispose();

                QMDBLogger.UpdateLastEndTime(taskid, DateTime.Now);
                QMDBLogger.Info(taskid, QMLogLevel.Info.ToString(), "运行完成");
            }
            catch (AccessViolationException aex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}", aex.Message));
                QMDBLogger.UpdateLastErrorTime(context.JobDetail.Key.Name, DateTime.Now);
                QMDBLogger.Info(context.JobDetail.Key.Name, QMLogLevel.Fatal.ToString(), string.Format("任务回调时发生严重错误，{0}", aex.Message));
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}", ex.Message));
                QMDBLogger.UpdateLastErrorTime(context.JobDetail.Key.Name, DateTime.Now);
                QMDBLogger.Info(context.JobDetail.Key.Name, QMLogLevel.Fatal.ToString(), string.Format("任务回调时发生严重错误，{0}", ex.Message));
            }
            catch (SystemException sex)
            {
                log.Fatal(string.Format("任务回调时发生严重错误，{0}", sex.Message));
                QMDBLogger.UpdateLastErrorTime(context.JobDetail.Key.Name, DateTime.Now);
                QMDBLogger.Info(context.JobDetail.Key.Name, QMLogLevel.Fatal.ToString(), string.Format("任务回调时发生严重错误，{0}", sex.Message));
            }
            catch
            {
                log.Fatal("任务回调时发生严重错误");
                QMDBLogger.UpdateLastErrorTime(context.JobDetail.Key.Name, DateTime.Now);
                QMDBLogger.Info(context.JobDetail.Key.Name, QMLogLevel.Fatal.ToString(), "任务回调时发生严重错误，{0}");
            }
        }
    }
}
