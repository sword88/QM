using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Configuration;
using QM.Core.Log;

namespace QM.Core.Environments
{
    public class QMStarter
    {
        public static ILogger CreateQMLogger(Type type)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new QMLogModule());
            
            builder.RegisterType<QMLogger>().As<ILogger>();
            builder.RegisterType<QMLoggerFactory>().As<ILoggerFactory>();

            var container = builder.Build();
            var logFac = container.Resolve<ILoggerFactory>();
            var log = logFac.CreateLogger(type);

            return log;
        }    
    }
}
