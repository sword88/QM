using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Model;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Data;
using Oracle.ManagedDataAccess.Client;
using System.Runtime.ExceptionServices;
using log4net;

namespace QM.Demo.Quituser
{
    public class Program : DllTask
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {           
            quit();
        }

        public static void quit()
        {
            try
            {
                log.Debug("start");
                Common.QuitUserByDevice();
                Common.QuitUser();
                log.Debug("end");
            }
            catch (AccessViolationException aex)
            {
                log.Fatal(aex.Message);
            }
            catch (QMException ex)
            {
                log.Fatal(ex.Message);
            }
        }

        public override void Run()
        {
            quit();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
