using QM.Core.Common;
using QM.Core.Excel;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Logisc;
using QM.Core.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Model
{
    public class Monitor : DllTask
    {
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(Monitor));

        public int MaxSeconds
        {
            get
            {
                return 5 * 60;   //每1分扫描 
            }
        }

        public override void Run()
        {
            TaskBLL td = new TaskBLL();
            IList<Tasks> t = td.GetTimeOutList(MaxSeconds);

            if (t.Count > 0)
            {
                log.Debug(string.Format("发现超时任务{0}",t.Count));

                DataTable dt = QMExtend.ToDataTable<Tasks>(t);
                QMText txt = new QMText();
                string body = txt.Export(dt, "任务超时清单");

                IMail mail = new QMMail();
                mail.Subject = "任务超时清单";
                mail.AddBody(body, "MONITOR");
                mail.AddRecipient("junxiao_liang@aseglobal.com");


                if (mail.Send())
                {
                    log.Debug("[MAIL] 发送成功");
                }
                else
                {
                    log.Fatal("[MAIL] 发送失败");
                    throw new QMException("[MAIL] 发送失败");
                }
            }
            else
            {
                log.Debug("未发现超时任务");
            }

        }

        public override void Dispose()
        {            
            base.Dispose();
        }
    }
}
