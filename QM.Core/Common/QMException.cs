using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Common
{
    /// <summary>
    /// 自定义异常类
    /// </summary>
    public class QMException : ApplicationException
    {
        public QMException() : base() { }
        public QMException(string message) : base (message) { }
        public QMException(string message,Exception innerException) : base(message,innerException) {}
    }
}
