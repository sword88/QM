using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Model
{
    /// <summary>
    /// 任务日志
    /// </summary>
    public class TaskLog
    {
        /// <summary>
        /// idx
        /// </summary>
        public string idx { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public string taskid { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createtime { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 服务器
        /// </summary>
        public string server { get; set; }
    }
}
