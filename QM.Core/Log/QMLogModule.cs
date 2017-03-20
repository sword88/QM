using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace QM.Core.Log
{
    public class QMLogModule : Module
    {
        private readonly ConcurrentDictionary<string, ILogger> _logCache;

        public QMLogModule() {
            _logCache = new ConcurrentDictionary<string, ILogger>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QMLoggerFactory>().As<ILoggerFactory>().InstancePerLifetimeScope();

            builder.Register(CreateLogger).As<ILogger>().InstancePerDependency();
        }

        private static ILogger CreateLogger(IComponentContext context, IEnumerable<Parameter> parameter)
        {
            var logFactory = context.Resolve<ILoggerFactory>();
            var containType = parameter.TypedAs<Type>();

            return logFactory.CreateLogger(containType);
        }
    }
}
