using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using log4net;
using QM.Core.Model;

namespace QM.Server
{
    class QMServer : ServiceControl , IDisposable
    {
        /// <summary>
        /// 任务工厂
        /// </summary>
        private ISchedulerFactory _factory = null;
        /// <summary>
        /// 任务执行计划
        /// </summary>
        private IScheduler _scheduler = null;
        /// <summary>
        /// 任务锁标记
        /// </summary>
        private static object _lock = new object();
        /// <summary>
        /// 任务运行池
        /// </summary>
        private static Dictionary<string, TaskRuntimeInfo> _taskPool = new Dictionary<string, TaskRuntimeInfo>();
        /// <summary>
        /// 单实例任务管理
        /// </summary>
        private static QMServer _server = new QMServer();

        /// <summary>
        /// 初始化
        /// </summary>
        public QMServer()
        {
            _factory = new StdSchedulerFactory();
            _scheduler = _factory.GetScheduler();
            _scheduler.JobFactory = new QMJobFactory();
            _scheduler.Start();
            //InitLoadTaskList();
            TaskRuntimeInfo a = new TaskRuntimeInfo();

            Tasks t = new Tasks();
            t.idx = "123";
            t.taskName = "123";
            t.taskType = "DLL";
            t.taskCategory = "Cron";
            t.taskCreateTime = DateTime.Now;
            t.taskCron = "* * * * * ? *";
            t.taskFile = "E:\\ASECode\\Test\\QM.git\\QM.Test\\bin\\Debug\\QM.Test.exe";
            t.taskClsType = "QM.Test.Program";
            var dll = new QMAppDomainLoader<DllTask>().Load(t.taskFile, t.taskClsType, out a.domain);
            a.task = t;
            a.dllTask = dll;
            AddTask("123", a);


            Tasks t1 = new Tasks();
            t1.idx = "234";
            t1.taskName = "234";
            t1.taskType = "SQL";
            t1.taskCategory = "Cron";
            t1.taskCreateTime = DateTime.Now;
            t1.taskCron = "* * * * * ? *";
            t1.taskFile = "E:\\ASECode\\Test\\QM.git\\QM.Test\\bin\\Debug\\sql\\1.sql";
            a.task = t1;
            a.sqlTask = new SqlFileTask(t1.taskFile,"whfront/wh123@whdb");
            AddTask("234", a);
        }

        /// <summary>
        /// 获得任务池中实例
        /// </summary>
        /// <returns></returns>
        public static QMServer CreateInstance()
        {
            return _server;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            if (_scheduler != null || !_scheduler.IsStarted)
            {
                _scheduler.Start();
            }

            return _scheduler.IsStarted;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            if(_scheduler.IsStarted)
            {
                _scheduler.Shutdown();
            }

            return _scheduler.IsShutdown;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (!_scheduler.IsShutdown) 
            {
                _scheduler.Shutdown();
            }
        }

        /// <summary>
        /// 添加任务到队列中
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="taskinfo"></param>
        /// <returns></returns>
        public bool AddTask(string taskid, TaskRuntimeInfo taskinfo) 
        {
            lock(_lock)
            {
                if (!_taskPool.ContainsKey(taskid))
                {
                    //添加任务
                    JobBuilder jobBuilder = JobBuilder.Create()
                                            .WithIdentity(taskinfo.task.idx, taskinfo.task.taskCategory);

                    switch (taskinfo.task.taskType) {
                        case "SQLFILE":
                            jobBuilder = jobBuilder.OfType(typeof(QMSqlFileTaskJob));
                            break;
                        case "DLL":
                        default:
                            jobBuilder = jobBuilder.OfType(typeof(QMDllTaskJob));
                            break;
                    }

                    IJobDetail jobDetail = jobBuilder.Build();

                   ITrigger trigger = QM.Server.QMCornFactory.CreateTrigger(taskinfo);
                    _scheduler.ScheduleJob(jobDetail, trigger);
                    
                    _taskPool.Add(taskid, taskinfo);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 将任务从队列中移除
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool RemoveTask(string taskid)
        {
            lock(_lock)
            {
                if (_taskPool.ContainsKey(taskid))
                {
                    /*移除任务*/
                    var taskinfo = _taskPool[taskid];
                    //停止触发器  
                    _scheduler.PauseTrigger(new TriggerKey(taskinfo.task.idx, taskinfo.task.taskCategory));
                    //删除触发器
                    _scheduler.UnscheduleJob(new TriggerKey(taskinfo.task.idx, taskinfo.task.taskCategory));
                    //删除任务
                    _scheduler.DeleteJob(new JobKey(taskinfo.task.idx, taskinfo.task.taskCategory));

                    _taskPool.Remove(taskid);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 获得任务池中任务的信息
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public TaskRuntimeInfo GetTask(string taskid)
        {
            lock(_lock)
            {
                if (_taskPool.ContainsKey(taskid))
                {
                    return _taskPool[taskid];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获得任务池中任务列表
        /// </summary>
        /// <returns></returns>
        public List<TaskRuntimeInfo> GetTaskList()
        {
            return _taskPool.Values.ToList();
        }

        /// <summary>
        /// 初始化加载DB中任务
        /// </summary>
        public void InitLoadTaskList()
        {
            QM.Server.Data.TaskData task = new Data.TaskData();
            var tasklist = task.GetTaskList(null, null, null, null);
            foreach(Tasks t in tasklist){
                //AddTask(t.idx, t);
            }                     
        }
    }
}
