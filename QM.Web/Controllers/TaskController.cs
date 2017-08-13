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

namespace QM.Web.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            TaskData td = new TaskData();
            var task = td.GetTaskList();
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
            if(t.taskFile == ""){
                t.taskFile = fc.GetValue("qm_sql").AttemptedValue;
            }
            t.taskType = fc.GetValue("qm_type").AttemptedValue;
            t.taskDBCon = fc.GetValue("qm_dbcon").AttemptedValue;
            t.taskSendby = fc.GetValue("qm_sendby").AttemptedValue;
            t.taskClsType = "";            
            
            QMDBLogger.Info(t.idx, JsonConvert.SerializeObject(t));

            IList<TasksN2M> n2m = null;
            TasksN2M n = new TasksN2M();

            #region SQL设置
            if (t.taskType == "SQL-EXP")
            {
                //SQL Header
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "SQL";
                n.attrname = "HEADER";
                n.attrval = fc.GetValue("sql_subject").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //SQL File
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "SQL";
                n.attrname = "EXPFILE";
                n.attrval = fc.GetValue("sql_filename").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //SQL File Type
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "SQL";
                n.attrname = "EXPTYPE";
                n.attrval = fc.GetValue("sql_filetype").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);
            }
            #endregion

            #region  MAIL设置
            if (t.taskSendby.Contains("MAIL"))
            {
                //Mail to
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "MAIL";
                n.attrname = "TO";
                n.attrval = fc.GetValue("qm_to").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //Mail cc
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "MAIL";
                n.attrname = "CC";
                n.attrval = fc.GetValue("qm_cc").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //Mail bcc
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "MAIL";
                n.attrname = "BCC";
                n.attrval = fc.GetValue("qm_bcc").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //Mail subject
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "MAIL";
                n.attrname = "SUBJECT";
                n.attrval = fc.GetValue("mail_subject").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //Mail body
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "MAIL";
                n.attrname = "BODY";
                n.attrval = fc.GetValue("qm_body").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);
            }
            #endregion

            #region FTP设置
            if (t.taskSendby.Contains("FTP"))
            {
                //ftp server
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "FTP";
                n.attrname = "SERVER";
                n.attrval = fc.GetValue("ftp_server").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //ftp account
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "FTP";
                n.attrname = "ACCOUNT";
                n.attrval = fc.GetValue("ftp_account").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //ftp password
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "FTP";
                n.attrname = "PASSWORD";
                n.attrval = fc.GetValue("ftp_password").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);

                //ftp remote folder
                n.idx = sd.GetIdx();
                n.refidx = t.idx;
                n.refname = "FTP";
                n.attrname = "RMOTEPATH";
                n.attrval = fc.GetValue("ftp_remotepath").AttemptedValue;
                QMDBLogger.Info(n.idx, JsonConvert.SerializeObject(n));
                n2m.Add(n);
            }
            #endregion

            TaskBLL tbl = new TaskBLL();
            tbl.Insert(t, n2m);

            return View();
        }


        public ActionResult Edit()
        {
            QMBaseServer.AddWebTask("9121");
            return View();
        }

        /// <summary>
        /// cpu利用率
        /// </summary>
        /// <returns></returns>
        public JsonResult Cpu()
        {
            try
            {
                var result = QM.Core.Common.QMSysinfo.GetCpuUsage();

                string msg = JsonConvert.SerializeObject(result);

                return Json(new { result = true, msg = msg });
            }
            catch
            {
                return Json(new { result = false, msg = "" });
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