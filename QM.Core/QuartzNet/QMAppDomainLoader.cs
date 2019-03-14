using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using QM.Core.Model;
using QM.Core.Log;
using QM.Core.Exception;

namespace QM.Core.QuartzNet
{
    /// <summary>
    /// 应用程序域加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QMAppDomainLoader<T> where T : class
    {            
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(QMAppDomainLoader<T>));
        /// <summary>
        /// 加载应用程序域，获取相应实例
        /// </summary>
        /// <param name="dllpath"></param>
        /// <param name="classpath"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public T Load(string dllpath, string classpath, out AppDomain domain)
        {
            try
            {
                if (File.Exists(dllpath))
                {
                    AppDomainSetup setup = new AppDomainSetup();
                    if (File.Exists(dllpath + ".config"))
                    {
                        setup.ConfigurationFile = dllpath + ".config";
                        log.Debug(string.Format("[QMAppDomainLoader] 加载config file:{0}", setup.ConfigurationFile));
                    }

                    setup.ApplicationBase = Path.GetDirectoryName(dllpath);
                    log.Debug(string.Format("[QMAppDomainLoader] 加载目录:{0}", setup.ApplicationBase));



                    setup.CachePath = setup.ApplicationBase;
                    setup.ShadowCopyFiles = "true";
                    setup.ShadowCopyDirectories = setup.ApplicationBase;
                    //setup.ApplicationName = "Dynamic";
                    domain = AppDomain.CreateDomain(Path.GetFileName(dllpath), null, setup);
                    AppDomain.MonitoringIsEnabled = true;
                    T obj = (T)domain.CreateInstanceFromAndUnwrap(dllpath, classpath);

                    return obj;
                }
                else
                {
                    log.Error(string.Format("[QMAppDomainLoader] 加载 file 不存在!:{0}", dllpath));
                }

            }
            catch (QMException ex)
            {
                log.Error(ex.Message);
            }

            domain = null;
            T obj1 = null;
            return obj1;
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        /// <param name="domain"></param>
        public void UnLoad(AppDomain domain)
        {
            AppDomain.Unload(domain);
            log.Debug("[QMAppDomainLoader] 卸载应用程序域成功");
            domain = null;
        }

    }
}
