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
using Oracle.ManagedDataAccess.Client;

namespace QM.Core.Data
{
    public class TaskN2MData
    {
        private QMDBHelper qmdb = new QMDBHelper(ConfigurationManager.ConnectionStrings["database"].ConnectionString);

        /// <summary>
        /// 获得参数
        /// </summary>
        /// <param name="taskid">任务Id</param>
        /// <param name="refname">参数组</param>
        /// <returns></returns>
        public IList<TasksN2M> GetParms(string taskid,string refname = "")
        {

            IList<TasksN2M> task = new List<TasksN2M>();
            TasksN2M t = null;
            try
            {
                string sql = "select * from qm_task_n2m where refidx = '" + taskid + "'";
                if (refname != "")
                {
                    sql += "and refname = '" + refname + "'";
                }

                OracleDataReader dr = qmdb.ExecuteReader(CommandType.Text, sql);
                while (dr.Read())
                {
                    t = new TasksN2M();
                    t.idx = dr["IDX"].ToString();
                    t.refidx = dr["REFIDX"].ToString();
                    t.refname = dr["REFNAME"].ToString();
                    t.attrname = dr["ATTRNAME"].ToString();
                    t.attrval = dr["ATTRVAL"].ToString();
                    task.Add(t);
                }
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
