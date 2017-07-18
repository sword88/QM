using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using QM.Core;
using QM.Server;
using QM.Core.Log;
using QM.Core.Environments;
using QM.Core.QuartzNet;

namespace QM.Server
{
    class Program
    {

        static void Main(string[] args)
        {
            var host = QMStarter.CreateQMLogger(typeof(Program));

            host.Debug("QM.Server服务开启");
            //异常捕获
            AppDomain.CurrentDomain.UnhandledException += UnKownError;

            try
            {
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
            catch (Exception ex)
            {
                host.Fatal(ex.Message);
            }
        }

        /// <summary>
        /// 未知异常捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnKownError(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = ((Exception)e.ExceptionObject).GetBaseException();            
        }
    }
}
