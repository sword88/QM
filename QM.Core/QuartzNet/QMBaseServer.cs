using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QM.Core.Model;
using QM.Core.Exception;
using QM.Core.Log;
using System.Configuration;
using QM.Core.Logisc;

namespace QM.Core.QuartzNet
{
    public class QMBaseServer : IDisposable
    {
        /// <summary>
        /// 任务工厂
        /// </summary>
        private ISchedulerFactory _factory = null;
        /// <summary>
        /// 任务持久化参数
        /// </summary>
        private NameValueCollection _properties = null;
        /// <summary>
        /// 任务执行计划
        /// </summary>
        private static IScheduler _scheduler = null;
        private static IScheduler _remoteScheduler = null;
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
        private static QMBaseServer _server = new QMBaseServer();
        /// <summary>
        /// log
        /// </summary>
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(QMBaseServer));

        /// <summary>
        /// 初始化
        /// </summary>
        public QMBaseServer()
        {
            _factory = new StdSchedulerFactory();
            _scheduler = _factory.GetScheduler();
            _scheduler.JobFactory = new QMJobFactory();
        }

        /// <summary>
        /// 获得任务池中实例
        /// </summary>
        /// <returns></returns>
        public static QMBaseServer CreateInstance()
        {
            return _server;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (_scheduler != null || !_scheduler.IsStarted)
            {
                _scheduler.Start();
                log.Info("QM Server 开启服务");
            }

            return _scheduler.IsStarted;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if(_scheduler.IsStarted)
            {
                _scheduler.Shutdown();
                log.Info("QM Server 停止服务");
            }

            return _scheduler.IsShutdown;
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public static bool PauseJob(string taskid)
        {
            bool result = false;
            try
            {
                JobKey jk = new JobKey(taskid);

                if (!_scheduler.CheckExists(jk))
                {                    
                    _scheduler.PauseJob(jk);
                    log.Info(string.Format("暂停任务{0}",taskid));
                }

                result = true;
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("暂停任务失败,错误信息{0},{1}", ex.Message,ex.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 暂停所有任务
        /// </summary>
        /// <returns></returns>
        public bool PauseAll()
        {
            bool result = false;

            try
            {
                if (_scheduler.IsStarted)
                {
                    _scheduler.PauseAll();
                    log.Info("QM Server 暂停所有服务");
                }
                result = true;
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("暂停所有任务失败,错误信息{0},{1}", ex.Message,ex.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 恢复暂时运行的任务
        /// </summary>
        /// <param name="taskid">任务id</param>
        /// <returns></returns>
        public static bool ResumeJob(string taskid)
        {
            bool result = false;
            try
            {
                JobKey jk = new JobKey(taskid);
                if (_scheduler.CheckExists(jk))
                {
                    _scheduler.ResumeJob(jk);
                    log.Info(string.Format("恢复暂时运行的任务{0}", taskid));
                }
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("恢复暂时运行的任务,错误信息{0}", ex.Message));
            }
            return result;
        }

        /// <summary>
        /// 恢复所有暂停服务
        /// </summary>
        /// <returns></returns>
        public bool ResumeAll()
        {
            bool result = false;

            try
            {
                if (_scheduler.IsStarted)
                {
                    _scheduler.ResumeAll();
                    log.Info("QM Server 恢复暂停所有服务");
                }
                result = true;
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("恢复暂停所有任务失败,错误信息{0},{1}", ex.Message, ex.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (!_scheduler.IsShutdown) 
            {
                _scheduler.Shutdown();
                log.Info("QM Server 停止服务&资源释放");
            }
        }

        /// <summary>
        /// 添加任务到队列中
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="taskinfo"></param>
        /// <param name="misfire"></param>
        /// <returns></returns>
        public static bool AddTask(string taskid, TaskRuntimeInfo taskinfo, QMMisFire misfire = QMMisFire.DoNothing) 
        {
            lock(_lock)
            {
                if (!_taskPool.ContainsKey(taskid))
                {
                    //添加任务
                    JobBuilder jobBuilder = JobBuilder.Create()
                                            .WithIdentity(taskinfo.task.idx);
                                            //.WithIdentity(taskinfo.task.idx, taskinfo.task.taskCategory);

                    switch (taskinfo.task.taskType) {
                        case "SQL-FILE":
                            jobBuilder = jobBuilder.OfType(typeof(QMSqlFileTaskJob));
                            break;
                        case "SQL-EXP":
                        case "SQL":
                            jobBuilder = jobBuilder.OfType(typeof(QMSqlExpTaskJob));
                            break;
                        case "DLL-STD":
                            jobBuilder = jobBuilder.OfType(typeof(QMDllTaskJob));
                            break;
                        case "DLL-UNSTD":
                        default:
                            jobBuilder = jobBuilder.OfType(typeof(QMUnStdDllTaskJob));
                            break;
                    }

                    IJobDetail jobDetail = jobBuilder.Build();

                    //任务Misfire处理规则
                    ITrigger trigger;
                    
                    if (misfire == QMMisFire.FireAndProceed)        
                    {
                        //以当前时间为触发频率立刻触发一次执行
                        //然后按照Cron频率依次执行        
                        trigger = TriggerBuilder.Create()
                                        .WithIdentity(taskinfo.task.taskName, "Cron")
                                        .WithCronSchedule(taskinfo.task.taskCron,
                                         x => x.WithMisfireHandlingInstructionFireAndProceed())
                                        .ForJob(jobDetail.Key)
                                        .Build();
                    }
                    else if (misfire == QMMisFire.IgnoreMisfires)       
                    {
                        //以错过的第一个频率时间立刻开始执行
                        //重做错过的所有频率周期后
                        //当下一次触发频率发生时间大于当前时间后，再按照正常的Cron频率依次执行
                        trigger = TriggerBuilder.Create()
                                                .WithIdentity(taskinfo.task.taskName, "Cron")
                                                .WithCronSchedule(taskinfo.task.taskCron,
                                                 x => x.WithMisfireHandlingInstructionIgnoreMisfires())
                                                .ForJob(jobDetail.Key)
                                                .Build();
                    }
                    else
                    {
                        //不触发立即执行
                        //等待下次Cron触发频率到达时刻开始按照Cron频率依次执行
                        trigger = TriggerBuilder.Create()
                        .WithIdentity(taskinfo.task.taskName, "Cron")
                        .WithCronSchedule(taskinfo.task.taskCron,
                         x => x.WithMisfireHandlingInstructionDoNothing())
                        .ForJob(jobDetail.Key)
                        .Build();
                    }                   
                                                       

                    if (_scheduler.CheckExists(jobDetail.Key))
                    {
                        _scheduler.DeleteJob(jobDetail.Key);
                    }
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
        /// <param name="taskid">任务id</param>
        /// <returns></returns>
        public static bool RemoveTask(string taskid)
        {
            lock(_lock)
            {
                if (_taskPool.ContainsKey(taskid))
                {
                    /*移除任务*/
                    var taskinfo = _taskPool[taskid];
                    //停止触发器  
                    //_scheduler.PauseTrigger(new TriggerKey(taskinfo.task.idx, taskinfo.task.taskCategory));
                    _scheduler.PauseTrigger(new TriggerKey(taskinfo.task.idx));
                    //删除触发器
                    //_scheduler.UnscheduleJob(new TriggerKey(taskinfo.task.idx, taskinfo.task.taskCategory));
                    _scheduler.UnscheduleJob(new TriggerKey(taskinfo.task.idx));
                    //删除任务
                    //_scheduler.DeleteJob(new JobKey(taskinfo.task.idx, taskinfo.task.taskCategory));
                    _scheduler.DeleteJob(new JobKey(taskinfo.task.idx));

                    _taskPool.Remove(taskid);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 获得任务池中任务的信息
        /// </summary>
        /// <param name="taskid">任务id</param>
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
                    var taskinfo = GetDbTask(taskid);
                    _taskPool.Add(taskid, taskinfo);
                    return _taskPool[taskid];
                }
            }
        }

        /// <summary>
        /// 从DB中查询任务结果
        /// </summary>
        /// <param name="taskid">任务id</param>
        /// <returns></returns>
        public TaskRuntimeInfo GetDbTask(string taskid)
        {
            TaskBLL td = new TaskBLL();
            var t = td.Detail(taskid);
            TaskRuntimeInfo trun = new TaskRuntimeInfo();            

            switch (t.taskType)
            {
                case "SQL-FILE":
                    trun.sqlFileTask = new SqlFileTask(t.taskFile, t.taskDBCon);
                    break;
                case "SQL-EXP":
                    trun.parms = td.GetParms(taskid);
                    trun.sqlExpTask = new SqlExpJob(t.taskDBCon, t.taskFile, taskid, t.taskParm, trun.parms);
                    break;
                case "DLL-STD":
                    trun.dllTask = new QMAppDomainLoader<DllTask>().Load(t.taskFile, t.taskClsType, out trun.domain);
                    break;
                case "DLL-UNSTD":
                default:
                    trun.unStdDllTask = new UnStdDll(t.taskFile, t.taskParm);
                    break;
            }
            trun.task = t;            

            return trun;
        }

        /// <summary>
        /// 获得任务池中任务列表
        /// </summary>
        /// <returns></returns>
        public static List<TaskRuntimeInfo> GetTaskList()
        {
            return _taskPool.Values.ToList();
        }

        /// <summary>
        /// 初始化加载DB中任务
        /// 【开启数据库持久化后，不可以重复加载】
        /// </summary>
        public void InitLoadTaskList()
        {
            try
            {
                TaskBLL td = new TaskBLL();
                var tasklist = td.GetList();
                TaskRuntimeInfo trun = null;

                foreach (Tasks t in tasklist)
                {
                    trun = new TaskRuntimeInfo();
                    trun.parms = td.GetParms(t.idx);

                    switch (t.taskType)
                    {
                        case "SQL-FILE":
                            trun.sqlFileTask = new SqlFileTask(t.taskFile, t.taskDBCon);
                            break;
                        case "SQL-EXP":
                            trun.sqlExpTask = new SqlExpJob(t.taskDBCon, t.taskFile, t.idx, t.taskParm, trun.parms);
                            break;
                        case "DLL-STD":
                            trun.dllTask = new QMAppDomainLoader<DllTask>().Load(t.taskFile, t.taskClsType, out trun.domain);
                            break;
                        case "DLL-UNSTD":
                        default:
                            trun.unStdDllTask = new UnStdDll(t.taskFile, t.taskParm);
                            break;
                    }
                    trun.task = t;

                    AddTask(t.idx, trun);
                }
            }
            catch (QMException ex)
            {
                log.Fatal(ex.Message);
            }
        }

        /// <summary>
        /// 添加远程任务
        /// </summary>
        /// <param name="taskid">任务id</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否成功</returns>
        public static bool AddRemoteTask(string taskid,out string message)
        {
            bool result = false;
            try
            {
                TaskBLL td = new TaskBLL();
                var t = td.Detail(taskid);

                TaskRuntimeInfo trun = new TaskRuntimeInfo();
                trun.parms = td.GetParms(taskid);
                switch (t.taskType)
                {
                    case "SQL-FILE":
                        trun.sqlFileTask = new SqlFileTask(t.taskFile, t.taskDBCon);
                        break;
                    case "SQL-EXP":
                        trun.sqlExpTask = new SqlExpJob(t.taskDBCon, t.taskFile, taskid, t.taskParm, trun.parms);
                        break;
                    case "DLL-STD":
                        trun.dllTask = new QMAppDomainLoader<DllTask>().Load(t.taskFile, t.taskClsType, out trun.domain);
                        break;
                    case "DLL-UNSTD":
                    default:
                        trun.unStdDllTask = new UnStdDll(t.taskFile, t.taskParm);
                        break;
                }
                trun.task = t;

                lock (_lock)
                {
                    if (!_taskPool.ContainsKey(taskid))
                    {
                        //添加任务
                        JobBuilder jobBuilder = JobBuilder.Create()
                                                .WithIdentity(trun.task.idx);
                        //.WithIdentity(taskinfo.task.idx, taskinfo.task.taskCategory);

                        switch (trun.task.taskType)
                        {
                            case "SQL-FILE":
                                jobBuilder = jobBuilder.OfType(typeof(QMSqlFileTaskJob));
                                break;
                            case "SQL-EXP":
                            case "SQL":
                                jobBuilder = jobBuilder.OfType(typeof(QMSqlExpTaskJob));
                                break;
                            case "DLL-STD":
                                jobBuilder = jobBuilder.OfType(typeof(QMDllTaskJob));
                                break;
                            case "DLL-UNSTD":
                            default:
                                jobBuilder = jobBuilder.OfType(typeof(QMUnStdDllTaskJob));
                                break;
                        }

                        IJobDetail jobDetail = jobBuilder.Build();

                        //任务Misfire处理规则
                        //以当前时间为触发频率立刻触发一次执行
                        //然后按照Cron频率依次执行        
                        ITrigger trigger = TriggerBuilder.Create()
                                        .WithIdentity(trun.task.taskName, "Cron")
                                        .WithCronSchedule(trun.task.taskCron,
                                            x => x.WithMisfireHandlingInstructionFireAndProceed())
                                        .ForJob(jobDetail.Key)
                                        .Build();                       


                        if (_remoteScheduler.CheckExists(jobDetail.Key))
                        {
                            _remoteScheduler.DeleteJob(jobDetail.Key);
                        }
                        _remoteScheduler.ScheduleJob(jobDetail, trigger);

                        _taskPool.Add(taskid, trun);
                    }
                }

                message = string.Format("添加远程任务成功:{0}", taskid);
                result = true;
            }
            catch (QMException ex)
            {
                message = string.Format("添加远程任务失败:{0},{1}", ex.Message, ex.StackTrace);
                log.Fatal(message);                
            }

            return result;
        }

        /// <summary>
        /// 删除远程任务
        /// </summary>
        /// <param name="taskid">任务id</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否成功</returns>
        public static bool DelRemoteTask(string taskid, out string message)
        {
            bool result = false;
            try
            {
                lock (_lock)
                {
                    if (!_taskPool.ContainsKey(taskid))
                    {
                        //任务
                        JobBuilder jobBuilder = JobBuilder.Create()
                                                .WithIdentity(taskid);

                        IJobDetail jobDetail = jobBuilder.Build();

                        if (_remoteScheduler.CheckExists(jobDetail.Key))
                        {
                            _remoteScheduler.DeleteJob(jobDetail.Key);
                        }

                        _taskPool.Remove(taskid);
                    }
                }

                message = string.Format("删除远程任务成功:{0}", taskid);
                result = true;
            }
            catch (QMException ex)
            {
                message = string.Format("删除远程任务失败:{0},{1},{2},{3},{4}", taskid, ex.Message, ex.StackTrace);
                log.Fatal(message);
            }

            return result;
        }

        /// <summary>
        /// 初始化远程服务
        /// </summary>
        /// <param name="protocol">协议</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="message">返回信息</param>
        /// <returns>是否成功</returns>
        public static bool InitRemoteScheduler(string protocol = "tcp",string ip = "127.0.0.1",string port = "555",out string message)
        {
            bool result = false;

            try
            {
                NameValueCollection properties = new NameValueCollection();
                properties["quartz.scheduler.instanceName"] = "RemoteServer";

                properties["quartz.scheduler.proxy"] = "false";

                properties["quartz.scheduler.proxy.address"] = string.Format("{0}://{1}:{2}/QuartzScheduler", protocol, ip, port);

                ISchedulerFactory sf = new StdSchedulerFactory(properties);

                _remoteScheduler = sf.GetScheduler();
                message = string.Format("初始化远程服务成功:{0},{1},{2}", protocol, ip, port);
                result = true;
            }
            catch (SchedulerException sex)
            {
                message = string.Format("初始化远程服务失败:{0},{1},{2},{3},{4}", protocol, ip, port, sex.Message, sex.StackTrace);
                log.Fatal(message);
            }
            catch (RemotingException rex)
            {
                message = string.Format("初始化远程服务失败:{0},{1},{2},{3},{4}", protocol, ip, port, rex.Message, rex.StackTrace);
                log.Fatal(message);
            }
            catch (QMException ex)
            {
                message = string.Format("初始化远程服务失败:{0},{1},{2},{3},{4}", protocol, ip, port, ex.Message, ex.StackTrace);
                log.Fatal(message);
            }
            catch (SystemException sex)
            {
                message = string.Format("初始化远程服务失败:{0},{1},{2},{3},{4}", protocol, ip, port, sex.Message, sex.StackTrace);
                log.Fatal(message);
            }

            return result;
        }

        /// <summary>
        /// 返回IScheduler
        /// </summary>
        /// <returns></returns>
        public static IScheduler GetScheduler()
        {
            return _scheduler;
        }
    }
}
