using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using log4net;
using log4net.Config;

namespace QM.Core.Log
{
    public class QMLoggerFactory : ILoggerFactory
    {
        public QMLoggerFactory()
        {
            FileInfo configFile = new FileInfo(ConfigurationManager.AppSettings["log4net.config"]);
            XmlConfigurator.Configure(configFile);        
        }

        public ILogger CreateLogger(Type type)
        {
            return LogManager.GetLogger(type) as ILogger;
        }
    }
}
