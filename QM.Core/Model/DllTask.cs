using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using QM.Core.Exception;
using QM.Core.Log;

namespace QM.Core.Model
{
    /// <summary>
    /// 标准DLL/exe任务
    /// </summary>
    public abstract class DllTask : MarshalByRefObject, IDisposable
    {        
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(DllTask));
        public DllTask()
        { }

        public void TryRun()
        {
            try
            {
                log.Debug(string.Format("[StdDll][Start]标准DLL/exe任务"));
                Run();
                log.Debug(string.Format("[StdDll][End]标准DLL/exe任务"));
            }
            catch (AccessViolationException aex)
            {
                throw new QMException(aex.Message);
            }
            catch (QMException ex)
            {
                throw ex;
            }
            catch(System.Exception sex)
            {
                throw sex; 
            }
            finally
            {
                Dispose();
            }            
        }

        /// <summary>
        /// 与第三方约定的运行接口
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 重载生命周期策略
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            //return base.InitializeLifetimeService();
            return null;
        }

        /// <summary>
        /// 系统资源释放
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
