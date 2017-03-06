using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Server.Data
{
    public class QMBaseData : MyBatis
    {
        public QMBaseData() : base("QMSqlMap.config")
        {

        }
    }
}
