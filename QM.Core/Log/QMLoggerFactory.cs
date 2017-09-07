using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
        }

        public static QMLoggerFactory GetInstance()
        {
            string filename = ConfigurationManager.AppSettings["log4net.config"];
            if (filename != null)
            {
                FileInfo configFile = new FileInfo(filename);
                XmlConfigurator.Configure(configFile);
            }

            return new QMLoggerFactory();
        }

        public ILogger CreateLogger(Type type)
        {
            var log = new QMLogger(LogManager.GetLogger(type).Logger);
            return log as ILogger;
        }
    }
}
