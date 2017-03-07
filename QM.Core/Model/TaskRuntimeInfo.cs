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
        /// 应用程序域中任务dll实例引用
        /// </summary>
        public DllTask dllTask;

        /// <summary>
        /// SQL文件任务
        /// </summary>
        public SqlFileTask sqlTask;
    }
}
