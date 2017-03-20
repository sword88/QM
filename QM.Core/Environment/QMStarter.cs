using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Configuration;
using QM.Core.Log;

namespace QM.Core.Environment
{
    public class QMStarter
    {
        public static IContainer CreateHostContainer(Action<ContainerBuilder> registrations)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new QMLogModule());

            var container = builder.Build();

            return container;
        }
    }
}
