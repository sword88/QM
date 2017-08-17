using QM.Core.Data;
using QM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Logisc;

namespace QM.Core.Log
{
    public class QMDBLogger
    {
        public static TaskLogBLL log = new TaskLogBLL();
        public static TaskBLL tsk = new TaskBLL();

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="result"></param>
        public static void Info(string taskId, string type, string result)
        {
            Task t = new Task(()=>log.Info(taskId,type,result));
            t.Start();
        }

        /// <summary>
        /// 更新上次开始时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastStartTime"></param>
        public static void UpdateLastStartTime(string idx, DateTime lastStartTime)
        {
            Task t = new Task(() => tsk.UpdateLastStartTime(idx, lastStartTime));
            t.Start();
        }

        /// <summary>
        /// 更新上次结束时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastStartTime"></param>
        public static void UpdateLastEndTime(string idx, DateTime lastStartTime)
        {
            Task t = new Task(() => tsk.UpdateLastEndTime(idx, lastStartTime));
            t.Start();
        }

        /// <summary>
        /// 更新上次错误时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastStartTime"></param>
        public static void UpdateLastErrorTime(string idx, DateTime lastStartTime)
        {
            Task t = new Task(() => tsk.UpdateLastErrorTime(idx, lastStartTime));
            t.Start();
        }
    }
}
