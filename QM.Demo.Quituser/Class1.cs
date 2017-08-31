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

namespace QM.Demo.Quituser
{
    public class Class1 : DllTask
    {
        public override void Run()
        {
            try
            {
                Common.QuitUserByDevice();
                Common.QuitUser();
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }
    }
}
