using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QM.Web.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Message()
        {
            return PartialView();
        }

        public ActionResult AddTask()
        {
            return View();
        }

        public ActionResult EditTask()
        {
            return View();
        }

        public JsonResult NextFireTime()
        {
            string cronExpressionString = Request.Params["CronExpression"].ToString();
            try
            {
                var result = QM.Core.QuartzNet.QMCronHelper.GetNextFireTime(cronExpressionString, 5);

                string msg = JsonConvert.SerializeObject(result);

                return Json(new { result = true, msg = msg });
            }
            catch
            {
                return Json(new { result = false, msg = "" });
            }
        }
    }
}