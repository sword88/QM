﻿using System;
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
        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <param name="taskinfo"></param>
        /// <returns></returns>
        public static ITrigger CreateTrigger(TaskRuntimeInfo taskinfo)
        {
            TriggerBuilder trigger = null;

            //暂停自定义task category想法
            /*
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
            */
            //默认cron job
            trigger = TriggerBuilder.Create()
                        .WithIdentity(taskinfo.task.taskName, "Cron")
                        .WithCronSchedule(taskinfo.task.taskCron)
                        .ForJob(taskinfo.task.taskName,taskinfo.task.taskCategory);

            return trigger.Build();
        }
    }
}
