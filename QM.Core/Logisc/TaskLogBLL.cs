using QM.Core.Data;
using QM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Logisc
{
    public class TaskLogBLL
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="result">日志内容</param>
        public void Info(string taskId, string result)
        {
            TaskLog tlog = new TaskLog();
            SequenceData seq = new SequenceData();
            tlog.idx = seq.GetIdx();
            tlog.taskid = taskId;
            tlog.type = Dns.GetHostName();
            tlog.createtime = DateTime.Now.ToString();
            tlog.message = string.Format("任务ID：{0},时间:{1},服务器：{2},信息:{3}", taskId, DateTime.Now, Dns.GetHostName(), result);
            Data.TaskLogData tl = new TaskLogData();
            tl.Insert(tlog);
        }
    }
}
