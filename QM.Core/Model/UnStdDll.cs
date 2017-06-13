using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Diagnostics;
using QM.Core.Log;
using QM.Core.Exception;
using QM.Core.Environments;
using System.ComponentModel;

namespace QM.Core.Model
{
    /// <summary>
    /// 非标准接口型exe/bat
    /// </summary>
    public class UnStdDll
    {
        private static ILogger log = QMStarter.CreateQMLogger(typeof(SqlExpJob));
        private string filename;
        private string args;
        public UnStdDll(string _filename,string _args)
        {
            filename = _filename;
            args = _args;
        }

        public void TryRun()
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = filename;
            startinfo.Arguments = args;
            startinfo.CreateNoWindow = true;
            startinfo.UseShellExecute = false;
            Process pro = new Process();

            try
            {
                pro.StartInfo = startinfo;
                pro.Start();
                pro.WaitForExit();

                log.Debug(startinfo.FileName + " " + startinfo.Arguments);
            }
            catch (Win32Exception wex)
            {
                throw new QMException(wex.Message);
            }
            catch (QMException ex)
            {
                throw ex;
            }
            finally
            {
                if (pro != null)
                {
                    pro.Close();
                }
            }
        }
    }
}
