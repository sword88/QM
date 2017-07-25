using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Diagnostics;
using QM.Core.Exception;
using QM.Core.Environments;
using QM.Core.Log;

namespace QM.Core.Model
{
    /// <summary>
    /// SQL File任务
    /// </summary>
    public class SqlFileTask
    {
        private static ILogger log = QMStarter.CreateQMLogger(typeof(SqlFileTask));

        private string taskfile;
        private string dbconstr;
        public SqlFileTask(string file,string dbcon)
        {
            taskfile = file;
            dbconstr = dbcon;
        }

        public void TryRun()
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = "SqlPlus";
            startinfo.Arguments = dbconstr + " @ " + taskfile;
            startinfo.CreateNoWindow = true;
            startinfo.UseShellExecute = false;
            Process pro = new Process();

            try
            {
                pro.StartInfo = startinfo;
                log.Debug(string.Format("[SqlFileTask][Start] 程序名:{0},参数：{1}", startinfo.FileName, startinfo.Arguments));
                pro.Start();
                pro.WaitForExit();                
            }
            catch (QMException ex)
            {
                log.Error(string.Format("[SqlFileTask][Error] 程序名:{0},参数：{1},错误：{2}", startinfo.FileName, startinfo.Arguments, ex.Message));
                throw ex;
            }
            finally
            {
                if (pro != null)
                {
                    pro.Close();
                    log.Debug(string.Format("[SqlFileTask][End] 程序名:{0}", startinfo.FileName));
                }
            }            
        }
    }
}
