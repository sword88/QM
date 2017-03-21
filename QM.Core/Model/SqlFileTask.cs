using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Diagnostics;
using log4net;
using QM.Core.Exception;

namespace QM.Core.Model
{
    /// <summary>
    /// SQL File任务
    /// </summary>
    public class SqlFileTask
    {
        private static ILog log = LogManager.GetLogger(typeof(SqlFileTask));

        private string taskfile;
        private string dbconstr;
        public SqlFileTask(string file,string dbcon)
        {
            taskfile = file;
            dbconstr = dbcon;
        }

        public void TryRun()
        {
            try
            {                
                ProcessStartInfo startinfo = new ProcessStartInfo();
                startinfo.FileName = "SqlPlus";
                startinfo.Arguments = dbconstr + " @ " + taskfile;
                startinfo.CreateNoWindow = true;
                startinfo.UseShellExecute = false;
                Process pro = Process.Start(startinfo);
                pro.WaitForExit();

                log.Debug(startinfo.FileName + " " + startinfo.Arguments);
            }
            catch (QMException ex)
            {
                throw ex;
            }            
        }
    }
}
