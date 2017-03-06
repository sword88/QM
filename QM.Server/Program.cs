using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Topshelf;
using QM.Core;
using QM.Server.Server;

namespace QM.Server
{
    class Program
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            //异常捕获
            AppDomain.CurrentDomain.UnhandledException += UnKownError;

            //设置Server信息
            HostFactory.Run(
                x =>
                {
                    x.SetDescription("ASEWH CIM Schedule Job Server");
                    x.SetDisplayName("CIM QM Server");
                    x.SetInstanceName("QM.Server");
                    x.SetServiceName("QM.Server");

                    x.Service(
                        s =>
                        {
                            var server = new QMServer();
                            return server;
                        });
                });
        }

        /// <summary>
        /// 未知异常捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnKownError(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = ((Exception)e.ExceptionObject).GetBaseException();
            log.Fatal(string.Format("QMServer未知异常:{0}", ex.Message));
        }
    }
}
