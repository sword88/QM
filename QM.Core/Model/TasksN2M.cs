using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Model
{
    /// <summary>
    /// 任务参数类
    /// </summary>
    [Serializable]
    public class TasksN2M
    {
        /// <summary>
        /// N2M编号
        /// </summary>
        public string idx { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string refidx { get; set; }
        /// <summary>
        /// 引用名称
        /// </summary>
        public string refname { get; set; }
        /// <summary>
        /// 参数名
        /// </summary>
        public string attrname { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string attrval { get; set; }
    }
}
