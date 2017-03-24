using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using QM.Core.Model;

namespace QM.Core.QuartzNet
{
    public class QMCornFactory
    {
        public static ITrigger CreateTrigger(TaskRuntimeInfo taskinfo)
        {
            TriggerBuilder trigger = null;
            switch (taskinfo.task.taskCategory)
            {
                case "Cron":
                    trigger = TriggerBuilder.Create()
                                .WithIdentity(taskinfo.task.taskName, taskinfo.task.taskCategory)
                                .WithCronSchedule(taskinfo.task.taskCron);
                    break;
                case "Sample":
                default:
                    trigger = TriggerBuilder.Create()
                                .WithIdentity(taskinfo.task.taskName, taskinfo.task.taskCategory)
                                .WithSimpleSchedule();
                    trigger.StartAt(taskinfo.task.taskCreateTime);
                    
                    trigger.WithDailyTimeIntervalSchedule();
                    break;
            }
          
            return trigger.Build();
        }
    }
}
