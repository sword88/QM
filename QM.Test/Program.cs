using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASEWH.Plugin.Mail;
using QM.Core;
using QM.Core.Model;
using log4net;
using ASEWH.Plugin.Mail;

namespace QM.Test
{   
    [Serializable]
    class Program : DllTask
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            try
            {
                Console.Write("1234");
                log.Debug("1234");
                SysMailMessage m = new SysMailMessage();
                m.FromName = "Junxiao Liang";
                m.Subject = "123";
                m.AddRecipient("junxiao_liang@aseglobal.com");
                m.Body = "test";
                m.Send();
            }
            catch (Exception ex)
            {
                throw ex;
            }                            
        }

        public override void Run()
        {
            Console.WriteLine(DateTime.Now.ToString());
            log.Debug(DateTime.Now.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
