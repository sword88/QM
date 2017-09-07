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
using System.ComponentModel;

namespace QM.Core.Model
{
    /// <summary>
    /// 非标准接口型exe/bat
    /// </summary>
    public class UnStdDll
    {
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(SqlExpJob));
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
                log.Debug(string.Format("[UnStdDll][Start] 程序名:{0},参数：{1}", startinfo.FileName, startinfo.Arguments));
                pro.Start();
                pro.WaitForExit();                
            }
            catch (Win32Exception wex)
            {
                log.Error(string.Format("[UnStdDll][Start] 程序名:{0},参数：{1},错误：{2}", startinfo.FileName, startinfo.Arguments, wex.Message));
                throw new QMException(wex.Message);                
            }
            catch (QMException ex)
            {
                log.Error(string.Format("[UnStdDll][Error] 程序名:{0},参数：{1},错误：{2}", startinfo.FileName, startinfo.Arguments, ex.Message));
                throw ex;
            }
            finally
            {
                if (pro != null)
                {
                    pro.Close();
                    log.Debug(string.Format("[UnStdDll][End] 程序名:{0}", startinfo.FileName));
                }
            }
        }
    }
}
