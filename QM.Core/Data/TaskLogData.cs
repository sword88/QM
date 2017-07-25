using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Model;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using QM.Core.Exception;
using System.IO;
using QM.Core.Environments;
using QM.Core.Log;

namespace QM.Core.Data
{
    public class TaskLogData
    {
        private static ILogger log = QMStarter.CreateQMLogger(typeof(TaskLogData));
        private QMDBHelper qmdb = new QMDBHelper(ConfigurationManager.ConnectionStrings["database"].ConnectionString);

        /// <summary>
        /// 新的任务记录
        /// </summary>
        /// <param name="t">任务记录类</param>
        public void Insert(TaskLog t)
        {
            try
            {
                string sql = @"insert into qm_tasklog
                                    (idx,
                                     taskid,
                                     type,
                                     createtime,
                                     message) 
                            values
                                    (?,
                                     ?,
                                     ?,
                                     ?,
                                     ?)";

                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("idx",t.idx),
                    new OleDbParameter("taskid",t.taskid),
                    new OleDbParameter("type",t.type),
                    new OleDbParameter("createtime",t.createtime),
                    new OleDbParameter("message",t.message)    
                };

                QMDBHelper.ExecuteNonQuery(sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
                log.Error(string.Format("TaskLogData=>Insert ERROR:{0}", ex.Message));
            }
        }
    }
}
