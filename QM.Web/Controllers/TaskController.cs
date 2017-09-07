using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QM.Core.Data;
using QM.Core.Model;
using QM.Core.Files;
using QM.Core.QuartzNet;
using QM.Core.Logisc;
using QM.Core.Log;
using QM.Core.Exception;

namespace QM.Web.Controllers
{
    public class TaskController : Controller
    {
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(TaskLogData));
        // GET: Task
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            TaskBLL tb = new TaskBLL();
            var task = tb.GetList();
            return View(task);
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Menu()
        {
            return PartialView();
        }

        /// <summary>
        /// 顶部消息
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Message()
        {
            return PartialView();
        }

        /// <summary>
        /// 时间设置
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Time()
        {
            return PartialView();
        }

        public ActionResult TimePop()
        {
            return PartialView();
        }

        /// <summary>
        /// 邮件设置
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Mail()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Ftp()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult SQL()
        {
            return PartialView();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(FormCollection fc)
        {
            try
            {
                #region 基本设置

                Tasks t = new Tasks();
                SeqBLL sd = new SeqBLL();
                t.idx = sd.GetIdx();
                t.taskName = fc.GetValue("qm_name").AttemptedValue;
                t.taskParm = fc.GetValue("qm_parms").AttemptedValue;
                t.taskRemark = fc.GetValue("qm_remark").AttemptedValue;
                t.taskCron = fc.GetValue("qm_cron").AttemptedValue;
                t.taskState = fc.GetValue("qm_status").AttemptedValue;
                t.taskCreateTime = DateTime.Now;
                t.taskFile = QMFile.UploadFile();
                if (t.taskFile == "")
                {
                    t.taskFile = fc.GetValue("qm_sql").AttemptedValue;
                }
                t.taskType = fc.GetValue("qm_type").AttemptedValue;
                t.taskDBCon = fc.GetValue("qm_dbcon").AttemptedValue;
                t.taskSendby = fc.GetValue("qm_sendby").AttemptedValue;
                t.taskClsType = "";

                QMDBLogger.Info(t.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(t));

                IList<TasksN2M> n2m = new List<TasksN2M>();
                TasksN2M n;

                #endregion

                #region SQL设置
                if (t.taskType == "SQL-EXP")
                {
                    //SQL Header
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "SQL";
                    n.attrname = "HEADER";
                    n.attrval = fc.GetValue("sql_subject").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //SQL File
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "SQL";
                    n.attrname = "EXPFILE";
                    n.attrval = fc.GetValue("sql_filename").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //SQL EXP Date
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "SQL";
                    n.attrname = "EXPDATE";
                    n.attrval = fc.GetValue("sql_expdate").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //SQL EXP Path
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "SQL";
                    n.attrname = "EXPPATH";
                    n.attrval = fc.GetValue("sql_exppath").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //SQL File Type
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "SQL";
                    n.attrname = "EXPTYPE";
                    n.attrval = fc.GetValue("sql_filetype").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);
                }
                #endregion

                #region  MAIL设置
                if (t.taskSendby.Contains("MAIL"))
                {
                    //Mail to
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "MAIL";
                    n.attrname = "TO";
                    n.attrval = fc.GetValue("qm_to").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //Mail cc
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "MAIL";
                    n.attrname = "CC";
                    n.attrval = fc.GetValue("qm_cc").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //Mail bcc
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "MAIL";
                    n.attrname = "BCC";
                    n.attrval = fc.GetValue("qm_bcc").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //Mail subject
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "MAIL";
                    n.attrname = "SUBJECT";
                    n.attrval = fc.GetValue("qm_subject").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //Mail subdate
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "MAIL";
                    n.attrname = "SUBDATE";
                    n.attrval = fc.GetValue("qm_subdate").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //Mail body
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "MAIL";
                    n.attrname = "BODY";
                    n.attrval = fc.GetValue("qm_body").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);
                }
                #endregion

                #region FTP设置
                if (t.taskSendby.Contains("FTP"))
                {
                    //ftp server
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "FTP";
                    n.attrname = "SERVER";
                    n.attrval = fc.GetValue("ftp_server").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //ftp account
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "FTP";
                    n.attrname = "ACCOUNT";
                    n.attrval = fc.GetValue("ftp_account").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //ftp password
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "FTP";
                    n.attrname = "PASSWORD";
                    n.attrval = fc.GetValue("ftp_password").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);

                    //ftp remote folder
                    n = new TasksN2M();
                    n.idx = sd.GetIdx();
                    n.refidx = t.idx;
                    n.refname = "FTP";
                    n.attrname = "RMOTEPATH";
                    n.attrval = fc.GetValue("ftp_remotepath").AttemptedValue;
                    QMDBLogger.Info(n.idx, QMLogLevel.Debug.ToString(), JsonConvert.SerializeObject(n));
                    n2m.Add(n);
                }
                #endregion

                TaskBLL tbl = new TaskBLL();
                if (tbl.Insert(t, n2m))
                {
                    QMBaseServer.AddWebTask(t.idx);
                    return RedirectToAction("list");
                }
            }
            catch (QMException ex)
            {
                log.Log(QMLogLevel.Fatal, ex, "新增任务出错", null);
                QMDBLogger.Info("NA", QMLogLevel.Fatal.ToString(), ex.Message);
            }

            return View();
        }


        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Log(string idx = "")
        {
            TaskLogBLL tb = new TaskLogBLL();
            var log = tb.LogList(idx);

            return View(log);
        }

        /// <summary>
        /// 性能
        /// </summary>
        /// <returns></returns>
        public JsonResult Performance()
        {
            try
            {
                QM.Core.Common.QMSysinfo sys = new Core.Common.QMSysinfo();
                var mresult = sys.GetMemoryFreeUsage() * 100;
                var cresult = QM.Core.Common.QMSysinfo.GetCpuUsage();

                string cpu = JsonConvert.SerializeObject(cresult);
                string memory = JsonConvert.SerializeObject(mresult);

                return Json(new { result = true, cpu = cpu, memory = memory });
            }
            catch
            {
                return Json(new { result = false, cpu = "", memory = "" });
            }
        }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        /// <returns></returns>
        public JsonResult NextFireTime()
        {
            string cronExpressionString = Request.Params["CronExpression"].ToString();
            try
            {
                string msg;     
                if (QM.Core.QuartzNet.QMCronHelper.ValidExpression(cronExpressionString))
                {
                    var result = QM.Core.QuartzNet.QMCronHelper.GetNextFireTime(cronExpressionString, 5);
                    msg = JsonConvert.SerializeObject(result);
                }
                else
                {
                    var result = new List<string> { "CRON表达式不正确" };
                    msg = JsonConvert.SerializeObject(result);
                }                

                return Json(new { result = true, msg = msg });
            }
            catch(Exception ex)
            {
                return Json(new { result = false, msg = ex.Message });
            }
        }
    }
}