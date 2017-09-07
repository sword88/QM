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
using QM.Core.Log;
using Oracle.ManagedDataAccess.Client;

namespace QM.Core.Data
{
    public class TaskLogData
    {
        private static ILogger log = QMLoggerFactory.GetInstance().CreateLogger(typeof(TaskLogData));
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
                                     server,
                                     createtime,
                                     message) 
                            values
                                    (:idx,
                                     :taskid,
                                     :type,
                                     :server,
                                     :createtime,
                                     :message)";

                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":idx",t.idx),
                    new OracleParameter(":taskid",t.taskid),
                    new OracleParameter(":type",t.type),
                    new OracleParameter(":server",t.server),
                    new OracleParameter(":createtime",t.createtime),
                    new OracleParameter(":message",t.message)
                };

                qmdb.ExecuteNonQuery(sql, CommandType.Text, param);
            }
            catch (QMException ex)
            {
                throw ex;
                log.Error(string.Format("TaskLogData=>Insert ERROR:{0}", ex.Message));
            }
            finally
            {
                qmdb.Disponse();
            }
        }


        public IList<TaskLog> GetLogList(string idx)
        {
            IList<TaskLog> task = new List<TaskLog>();
            TaskLog t = null;
            OracleDataReader dr;
            try
            {
                string sql = "select * from qm_tasklog";
                if (idx != "")
                {
                    sql += " where taskid='" + idx + "'";
                }
                sql += " order by createtime";
                dr = qmdb.ExecuteReader(CommandType.Text, sql);
                while (dr.Read())
                {
                    t = new TaskLog();
                    t.idx = dr["IDX"].ToString();
                    t.taskid = dr["TASKID"].ToString();
                    t.type = dr["TYPE"].ToString();
                    t.server = dr["SERVER"].ToString();
                    t.createtime = dr["CREATETIME"].ToString();
                    t.message = dr["MESSAGE"].ToString();
                    task.Add(t);
                }
                dr.Close();
            }
            catch (QMException ex)
            {
                throw ex;
            }
            finally
            {
                qmdb.Disponse();
            }

            return task;
        }
    }
}
