using System;
using System.ComponentModel;

namespace QM.Core.QuartzNet
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
    /// MisFire处理规则
    /// </summary>
    public enum QMMisFire
    {
        [Description("不触发立即执行，等下次Cron触发频率时执行")]
        DoNothing = 0,
        [Description("以错过的第一个频率时间开始执行,重做所有MisFire")]
        IgnoreMisfires = 1,
        [Description("以当前时间为触发频率立即执行一次,然后按频率执行")]
        FireAndProceed = 2
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
