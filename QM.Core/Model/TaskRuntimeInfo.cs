using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Model
{
    /// <summary>
    /// 任务运行时的信息
    /// </summary>
    [Serializable]
    public class TaskRuntimeInfo
    {
        /// <summary>
        /// 任务所在的应用程序域
        /// </summary>
        public AppDomain domain;

        /// <summary>
        /// 任务信息
        /// </summary>
        public Tasks task;

        /// <summary>
        /// 任务运行参数
        /// </summary>
        public IList<TasksN2M> parms;

        /// <summary>
        /// 应用程序域中任务dll实例引用
        /// </summary>
        public DllTask dllTask;

        /// <summary>
        /// 任务exe/bat实例引用
        /// </summary>
        public UnStdDll unStdDllTask;

        /// <summary>
        /// SQL文件任务
        /// </summary>
        public SqlFileTask sqlFileTask;

        /// <summary>
        /// SQL导出任务
        /// </summary>
        public SqlExpJob sqlExpTask;
    }
}
