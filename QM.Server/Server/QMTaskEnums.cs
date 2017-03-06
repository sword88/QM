using System;
using System.ComponentModel;

namespace QM.Server.Server
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum QMTaskStateEnum
    {
        [Description("运行中")]
        Running = 0,
        [Description("停止")]
        Stop = 1
    }

    /// <summary>
    /// 任务类型
    /// </summary>
    public enum QMTaskType
    {
        [Description("DLL类型")]
        Dll = 0,
        [Description("SQL类型")]
        SQL = 1
    }

    /// <summary>
    /// 任务命令
    /// </summary>
    public enum QMTaskCmdEnum
    {
        [Description("开启任务")]
        Start = 0,
        [Description("关闭任务")]
        Stop = 1,
        [Description("重启任务")]
        ReStart = 2
    }
}
