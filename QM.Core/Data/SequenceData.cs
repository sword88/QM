using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using QM.Core.Exception;

namespace QM.Core.Data
{
    public class SequenceData
    {
        private QMDBHelper qmdb = new QMDBHelper(ConfigurationManager.ConnectionStrings["database"].ConnectionString);

        /// <summary>
        /// 获得sequence
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public string GetIdx(string prefix = "")
        {
            string val = prefix;
            try
            {
                string sql = "select it_seq.nextval from dual";
                object idx = QMDBHelper.ExecuteScalar(sql);
                val = val + idx.ToString();
            }
            catch (QMException ex)
            {
                throw ex;
            }

            return val;
        }
    }
}
