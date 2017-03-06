using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using QM.Server;
using QM.Core.Common;
using QM.Core.Model;

namespace QM.Server.Data
{
    /// <summary>
    /// 任务日志数据库操作
    /// </summary>
    public class TaskLogData : QMBaseData
    {
        private ILog log = LogManager.GetLogger(typeof(TaskData));

        /// <summary>
        /// 添加任务日志
        /// </summary>
        /// <param name="tlog">任务日志</param>
        public void Add(TaskLog tlog)
        {
            try
            {
                Mapper.Insert("", tlog);
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("添加任务日志出错,错误信息：{0}", ex.Message));
            }
        }
    }
}
