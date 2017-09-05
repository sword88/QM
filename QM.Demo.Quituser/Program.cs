using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Environments;
using QM.Core.Model;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Data;
using Oracle.ManagedDataAccess.Client;
using System.Runtime.ExceptionServices;

namespace QM.Demo.Quituser
{
    public class Program : DllTask
    {
        static void Main(string[] args)
        {           
            quit();
        }

        public static void quit()
        {
            try
            {
                Common.QuitUserByDevice();
                Common.QuitUser();
            }
            catch (AccessViolationException aex)
            {
                throw new Exception(aex.Message);
            }
            catch (QMException ex)
            {
                throw ex;
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
