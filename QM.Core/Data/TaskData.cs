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

namespace QM.Core.DB
{
    public class TaskData 
    {        
        private QMDBHelper qmdb = new QMDBHelper(ConfigurationManager.ConnectionStrings["database"].ConnectionString);

        /// <summary>
        /// 获得所有任务清单
        /// </summary>
        /// <returns></returns>
        public IList<Tasks> GetList()
        {

            IList<Tasks> task = new List<Tasks>();
            Tasks t = null;
            try
            {                
                string sql = "select * from qm_task";
                OleDbDataReader dr = QMDBHelper.ExecuteReader(sql);
                while (dr.Read())
                {
                    t = new Tasks();
                    t.idx = dr["IDX"].ToString();
                    t.taskCategory = dr["TASKCATEGORY"].ToString();
                    t.taskClsType = dr["TASKCLSTYPE"].ToString();
                    t.taskCount = dr["TASKCOUNT"].ToString();
                    //t.taskCreateTime = dr["taskCreateTime"].ToString();
                    t.taskType = dr["TASKTYPE"].ToString();
                    t.taskDBCon = dr["TASKDBCON"].ToString();
                    t.taskParm = dr["TASKPARM"].ToString();
                    t.taskFile = dr["TASKFILE"].ToString();
                    t.taskExpFile = dr["TASKEXPFILE"].ToString();
                    t.taskName = dr["TASKNAME"].ToString();
                    t.taskState = dr["TASKSTATE"].ToString();
                    t.taskCron = dr["TASKCRON"].ToString();
                    t.taskRemark = dr["TASKREMARK"].ToString();
                    task.Add(t);
                }
            }
            catch (QMException ex)
            {
                throw ex;
            }

            return task;
        }

        public void Insert()
        {

        }

        public void Update()
        {

        }
    }
}
