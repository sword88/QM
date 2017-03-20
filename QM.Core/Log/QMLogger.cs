using System;
using System.Configuration;
using System.IO;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Core;
using QM.Core.Exception;
using QM.Core.Model;

namespace QM.Core.Log
{
    public class QMLogger : ILogger
    {
        private static readonly Type declaringType = typeof(QMLogger);
        protected internal log4net.Core.ILogger _logger { get; set; }

        public QMLogger(log4net.Core.ILogger log)
        {
            _logger = log;
        }

        public bool IsDebugEnabled
        {
            get
            {
                return _logger.IsEnabledFor(Level.Debug);
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return _logger.IsEnabledFor(Level.Error);
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return _logger.IsEnabledFor(Level.Fatal);
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return _logger.IsEnabledFor(Level.Info);
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return _logger.IsEnabledFor(Level.Warn);
            }
        }

        public log4net.Core.ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public void Debug(object message)
        {
            _logger.Log(declaringType, Level.Debug, message, null);
        }

        public void Debug(object message, System.Exception exception)
        {
            _logger.Log(declaringType, Level.Debug, message, exception);
        }

        public void DebugFormat(string format, object arg0)
        {
            _logger.Log(declaringType, Level.Debug, string.Format(format, arg0), null);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Debug, string.Format(format,args), null);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Debug, string.Format(provider, format, args), null);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            _logger.Log(declaringType, Level.Debug, string.Format(format, arg0, arg1), null);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Log(declaringType, Level.Debug, string.Format(format, arg0, arg1, arg2), null);
        }

        public void Error(object message)
        {
            _logger.Log(declaringType, Level.Error, message, null);
        }

        public void Error(object message, System.Exception exception)
        {
            _logger.Log(declaringType, Level.Error, message, exception);
        }

        public void ErrorFormat(string format, object arg0)
        {
            _logger.Log(declaringType, Level.Error, string.Format(format, arg0), null);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Error, string.Format(format, args), null);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Error, string.Format(provider, format, args), null);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            _logger.Log(declaringType, Level.Error, string.Format(format, arg0, arg1), null);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Log(declaringType, Level.Error, string.Format(format, arg0, arg1, arg2), null);
        }

        public void Fatal(object message)
        {
            _logger.Log(declaringType, Level.Fatal, message, null);
        }

        public void Fatal(object message, System.Exception exception)
        {
            _logger.Log(declaringType, Level.Fatal, message, exception);
        }

        public void FatalFormat(string format, object arg0)
        {
            _logger.Log(declaringType, Level.Fatal, string.Format(format, arg0), null);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Fatal, string.Format(format, args), null);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Fatal, string.Format(provider, format, args), null);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            _logger.Log(declaringType, Level.Fatal, string.Format(format, arg0, arg1), null);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Log(declaringType, Level.Fatal, string.Format(format, arg0, arg1, arg2), null);
        }

        public void Info(object message)
        {
            _logger.Log(declaringType, Level.Info, message, null);
        }

        public void Info(object message, System.Exception exception)
        {
            _logger.Log(declaringType, Level.Info, message, exception);
        }

        public void InfoFormat(string format, object arg0)
        {
            _logger.Log(declaringType, Level.Info, string.Format(format, arg0), null);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Info, string.Format(format, args), null);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Info, string.Format(provider, format, args), null);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            _logger.Log(declaringType, Level.Info, string.Format(format, arg0, arg1), null);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Log(declaringType, Level.Info, string.Format(format, arg0, arg1, arg2), null);
        }

        public void Warn(object message)
        {
            _logger.Log(declaringType, Level.Warn, message, null);
        }

        public void Warn(object message, System.Exception exception)
        {
            _logger.Log(declaringType, Level.Warn, message, exception);
        }

        public void WarnFormat(string format, object arg0)
        {
            _logger.Log(declaringType, Level.Warn, string.Format(format, arg0), null);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Warn, string.Format(format, args), null);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.Log(declaringType, Level.Warn, string.Format(provider, format, args), null);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            _logger.Log(declaringType, Level.Warn, string.Format(format, arg0, arg1), null);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _logger.Log(declaringType, Level.Debug, string.Format(format, arg0, arg1, arg2), null);
        }

        public void Log(Level level, QMException exception, string format, params object[] args)
        {
            if (args == null)
            {
                if (level == Level.Debug)
                {
                    Debug(format, exception);
                }
                else if (level == Level.Info)
                {
                    Info(format, exception);
                }
                else if (level == Level.Warn)
                {
                    Warn(format, exception);
                }
                else if (level == Level.Error)
                {
                    Error(format, exception);
                }
                else if (level == Level.Fatal)
                {
                    Fatal(format, exception);
                }                
            }
            else
            {
                if (level == Level.Debug)
                {
                    DebugFormat(format, args);
                }
                else if (level == Level.Info)
                {
                    InfoFormat(format, args);
                }
                else if (level == Level.Warn)
                {
                    WarnFormat(format, args);
                }
                else if (level == Level.Error)
                {
                    ErrorFormat(format, args);
                }
                else if (level == Level.Fatal)
                {
                    FatalFormat(format, args);
                }
            }
        }

        public bool IsEnabled(Level level)
        {
            if (level == Level.Debug)
            {
                return IsDebugEnabled;
            }
            else if (level == Level.Info)
            {
                return IsInfoEnabled;
            }
            else if (level == Level.Warn)
            {
                return IsWarnEnabled;
            }
            else if (level == Level.Error)
            {
                return IsErrorEnabled;
            }
            else if (level == Level.Fatal)
            {
                return IsFatalEnabled;
            }
            else
            {
                return false;
            }
            
        }
    }
}
