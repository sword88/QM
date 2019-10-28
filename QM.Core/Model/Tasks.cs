using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Model
{
    /// <summary>
    /// 任务类
    /// </summary>
    [Serializable]
    public partial class Tasks
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string idx { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string taskName { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string taskType { get; set; }
        /// <summary>
        /// 发送类型
        /// </summary>
        public string taskSendby { get; set; }
        /// <summary>
        /// 任务分类
        /// </summary>
        public string taskCategory { get; set; }
        /// <summary>
        /// 任务计划
        /// </summary>
        public string taskCron { get; set; }
        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime taskCreateTime { get; set; }
        /// <summary>
        /// 任务上次开始时间
        /// </summary>
        public DateTime taskLastStartTime { get; set; }
        /// <summary>
        /// 任务最后结束时间
        /// </summary>
        public DateTime taskLastEndTime { get; set; }
        /// <summary>
        /// 任务最后出错时间
        /// </summary>
        public DateTime taskLastErrorTime { get; set; }
        /// <summary>
        /// 任务出错数统计
        /// </summary>
        public int taskErrorCount { get; set; }
        /// <summary>
        /// 任务执行次数
        /// </summary>
        public string taskCount { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string taskState { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string taskRemark { get; set; }
        /// <summary>
        /// 任务类名
        /// </summary>
        public string taskClsType { get; set; }
        /// <summary>
        /// 任务路径
        /// </summary>
        public string taskFile { get; set; }
        /// <summary>
        /// 任务参数
        /// </summary>
        public string taskParm { get; set; }
        /// <summary>
        /// 数据库连接信息
        /// </summary>
        public string taskDBCon { get; set; }
        /// <summary>
        /// 导出文件名
        /// </summary>
        public string taskExpFile { get; set; }
    }
}
