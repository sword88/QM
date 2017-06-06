using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QM.Core.Data;

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
            var task = td.GetList();
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


        public ActionResult Ftp()
        {
            return PartialView();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit()
        {
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