using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QM.Core.Model;

namespace QM.Core.QuartzNet
{
    public class QMBaseServer : IDisposable
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
        private static QMBaseServer _server = new QMBaseServer();

        /// <summary>
        /// 初始化
        /// </summary>
        public QMBaseServer()
        {
            _factory = new StdSchedulerFactory();
            _scheduler = _factory.GetScheduler();
            _scheduler.JobFactory = new QMJobFactory();
            _scheduler.Start();
            InitLoadTaskList();
            //TaskRuntimeInfo a = new TaskRuntimeInfo();

            //Tasks t = new Tasks();
            //t.idx = "123";
            //t.taskName = "123";
            //t.taskType = "DLL-STD";
            //t.taskCategory = "Cron";
            //t.taskCreateTime = DateTime.Now;
            //t.taskCron = "0 0 0/1 * * ? ";
            //t.taskFile = @"E:\ASECode\Test\QM.git\QM.Demo.Quituser\bin\Debug\QM.Demo.Quituser.dll";
            //t.taskClsType = "QM.Demo.Quituser.Class1";
            //var dll = new QMAppDomainLoader<DllTask>().Load(t.taskFile, t.taskClsType, out a.domain);
            //a.task = t;
            //a.dllTask = dll;
            //AddTask("123", a);


            //Tasks t1 = new Tasks();
            //t1.idx = "234";
            //t1.taskName = "234";
            //t1.taskType = "SQL";
            //t1.taskCategory = "Cron";
            //t1.taskCreateTime = DateTime.Now;
            //t1.taskCron = "* * * * * ? *";
            //t1.taskFile = "E:\\ASECode\\Test\\QM.git\\QM.Test\\bin\\Debug\\sql\\1.sql";
            //a.task = t1;
            //a.sqlTask = new SqlFileTask(t1.taskFile, "whfront/wh123@whdb");
            //AddTask("234", a);


            //Tasks t2 = new Tasks();
            //t2.idx = "345";
            //t2.taskName = "345";
            //t2.taskType = "SQL";
            //t2.taskCategory = "Sample";
            //t2.taskCreateTime = DateTime.Now;
            //t2.taskCron = "* * * * * ? *";
            //t2.taskFile = "E:\\ASECode\\Test\\QM.git\\QM.Excel\\bin\\Debug\\QM.Excel.dll";
            //t2.taskClsType = "QM.Excel.Class1";
            //var dll1 = new QMAppDomainLoader<DllTask>().Load(t2.taskFile, t2.taskClsType, out a.domain);
            //a.task = t2;
            //a.dllTask = dll1;
            //AddTask("345", a);

            //Tasks t3 = new Tasks();
            //t3.idx = "345";
            //t3.taskName = "345";
            //t3.taskType = "SQL";
            //t3.taskCategory = "Sample";
            //t3.taskCreateTime = DateTime.Now;
            //t3.taskCron = "* * * * * ? *";
            //SqlJob s = new SqlJob("Provider=MSDAORA.1;Data Source=WHDB;Password=wh123;User ID=whfront", "select * from AF_LOGIN_HIS", "测试标题", "e:\\1.xls");
            //a.task = t3;
            //a.sqlTask = s;
            //AddTask("345", a);

            //Tasks t4 = new Tasks();
            //t4.idx = "567";
            //t4.taskName = "567";
            //t4.taskType = "DLL-UNSTD";
            //t4.taskCategory = "Cron";
            //t4.taskCreateTime = DateTime.Now;
            //t4.taskCron = "0/2 * * * * ? *";
            //UnStdDll u = new UnStdDll(@"E:\ASECode\Test\QM.git\QM.BAT\BAT\123.BAT", "");
            //a.task = t4;
            //a.unStdDllTask = u;
            //AddTask("567", a);

            //TaskRuntimeInfo a1 = new TaskRuntimeInfo();
            //Tasks t5 = new Tasks();
            //t5.idx = "678";
            //t5.taskName = "678";
            //t5.taskType = "DLL-UNSTD";
            //t5.taskCategory = "Cron";
            //t5.taskCreateTime = DateTime.Now;
            //t5.taskCron = "0 0/1 * * * ? *";
            //UnStdDll u1 = new UnStdDll(@"E:\ASECode\Test\QM.git\QM.BAT\bin\Debug\QM.BAT.exe", "asy");
            //a1.task = t5;
            //a1.unStdDllTask = u1;
            //AddTask("678", a1);

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
                        case "SQL-FILE":
                            jobBuilder = jobBuilder.OfType(typeof(QMSqlFileTaskJob));
                            break;
                        case "SQL-EXP":
                        case "SQL":
                            jobBuilder = jobBuilder.OfType(typeof(QMSqlTaskJob));
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

                   ITrigger trigger = QMCornFactory.CreateTrigger(taskinfo);
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

            DB.TaskData td = new DB.TaskData();
            var tasklist = td.GetList();
            TaskRuntimeInfo trun = null;
            foreach (Tasks t in tasklist)
            {
                trun = new TaskRuntimeInfo();
                switch (t.taskType)
                {
                    case "SQL-FILE":                        
                        trun.sqlFileTask = new SqlFileTask(t.taskFile, t.taskDBCon);
                        break;
                    case "SQL-EXP":
                        trun.sqlTask = new SqlExpJob(t.taskDBCon, t.taskFile, t.taskParm, t.taskExpFile);
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
    }
}
