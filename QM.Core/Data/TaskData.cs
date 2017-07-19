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

namespace QM.Core.Data
{
    public class TaskData
    {
        private QMDBHelper qmdb = new QMDBHelper(ConfigurationManager.ConnectionStrings["database"].ConnectionString);

        /// <summary>
        /// 获得所有任务清单
        /// </summary>
        /// <returns>IList[Tasks]()</returns>
        public IList<Tasks> GetList()
        {

            IList<Tasks> task = new List<Tasks>();
            Tasks t = null;
            try
            {
                string sql = "select * from qm_task where taskstate = 'Y'";
                OleDbDataReader dr = QMDBHelper.ExecuteReader(sql);
                while (dr.Read())
                {
                    t = new Tasks();
                    t.idx = dr["IDX"].ToString();                    
                    t.taskClsType = dr["TASKCLSTYPE"].ToString();
                    t.taskCount = dr["TASKCOUNT"].ToString();
                    t.taskCreateTime = DateTime.Parse(dr["TASKCREATETIME"].ToString());
                    if (dr["TASKLASTSTARTTIME"].ToString() != "")
                    {
                        t.taskLastStartTime = DateTime.Parse(dr["TASKLASTSTARTTIME"].ToString());
                    }
                    if (dr["TASKLASTENDTIME"].ToString() != "")
                    {
                        t.taskLastEndTime = DateTime.Parse(dr["TASKLASTENDTIME"].ToString());
                    }
                    if (dr["TASKLASTERRORTIME"].ToString() != "")
                    {
                        t.taskLastErrorTime = DateTime.Parse(dr["TASKLASTERRORTIME"].ToString());
                    }
                    if(dr["TASKERRORCOUNT"].ToString() != "")
                    {
                        t.taskErrorCount = int.Parse(dr["TASKERRORCOUNT"].ToString());
                    }
                    t.taskType = dr["TASKTYPE"].ToString();
                    t.taskDBCon = dr["TASKDBCON"].ToString();
                    t.taskParm = dr["TASKPARM"].ToString();
                    t.taskFile = dr["TASKFILE"].ToString();
                    t.taskExpFile = dr["TASKEXPFILE"].ToString();
                    t.taskName = dr["TASKNAME"].ToString();
                    t.taskState = dr["TASKSTATE"].ToString();
                    t.taskCron = dr["TASKCRON"].ToString();
                    t.taskRemark = dr["TASKREMARK"].ToString();
                    t.taskSendby = dr["TASKSENDBY"].ToString();
                    task.Add(t);
                }
            }
            catch (QMException ex)
            {
                throw ex;
            }

            return task;
        }

        /// <summary>
        /// 任务详细信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Tasks Detail(string idx)
        {
            Tasks t = null;
            try
            {
                string sql = "select * from qm_task where idx = ?";
                OleDbParameter[] param = new OleDbParameter[] {
                    new OleDbParameter("idx",idx)
                };
                OleDbDataReader dr = QMDBHelper.ExecuteReader(sql,param);
                while (dr.Read())
                {
                    t = new Tasks();
                    t.idx = dr["IDX"].ToString();                    
                    t.taskClsType = dr["TASKCLSTYPE"].ToString();
                    t.taskCount = dr["TASKCOUNT"].ToString();
                    if (dr["TASKLASTSTARTTIME"].ToString() != "")
                    {
                        t.taskLastStartTime = DateTime.Parse(dr["TASKLASTSTARTTIME"].ToString());
                    }
                    if (dr["TASKLASTENDTIME"].ToString() != "")
                    {
                        t.taskLastEndTime = DateTime.Parse(dr["TASKLASTENDTIME"].ToString());
                    }
                    if (dr["TASKLASTERRORTIME"].ToString() != "")
                    {
                        t.taskLastErrorTime = DateTime.Parse(dr["TASKLASTERRORTIME"].ToString());
                    }
                    if (dr["TASKERRORCOUNT"].ToString() != "")
                    {
                        t.taskErrorCount = int.Parse(dr["TASKERRORCOUNT"].ToString());
                    }
                    t.taskType = dr["TASKTYPE"].ToString();
                    t.taskDBCon = dr["TASKDBCON"].ToString();
                    t.taskParm = dr["TASKPARM"].ToString();
                    t.taskFile = dr["TASKFILE"].ToString();
                    t.taskExpFile = dr["TASKEXPFILE"].ToString();
                    t.taskName = dr["TASKNAME"].ToString();
                    t.taskState = dr["TASKSTATE"].ToString();
                    t.taskCron = dr["TASKCRON"].ToString();
                    t.taskRemark = dr["TASKREMARK"].ToString();
                    t.taskSendby = dr["TASKSENDBY"].ToString();
                }
            }
            catch (QMException ex)
            {
                throw ex;
            }

            return t;
        }

        /// <summary>
        /// 新的任务
        /// </summary>
        /// <param name="t">任务类</param>
        public void Insert(Tasks t)
        {
            try
            {
                string sql = @"insert into qm_task 
                                    (idx,
                                     taskname,
                                     tasksendby,
                                     taskcron,
                                     taskfile,
                                     taskparm,
                                     taskcreatetime,
                                     taskstate,
                                     taskremark,
                                     taskdbcon,
                                     taskexpfile,
                                     taskclstype,
                                     tasktype) 
                            values
                                    (?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?,
                                     ?)";

                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("idx",t.idx),
                    new OleDbParameter("taskname",t.taskName),
                    new OleDbParameter("tasksendby",t.taskSendby),
                    new OleDbParameter("taskcron",t.taskCron),
                    new OleDbParameter("taskfile",t.taskFile),
                    new OleDbParameter("taskparm",t.taskParm),
                    new OleDbParameter("taskcreatetime",t.taskCreateTime),
                    new OleDbParameter("taskstate",t.taskState),
                    new OleDbParameter("taskremark",t.taskRemark),
                    new OleDbParameter("taskdbcon",t.taskDBCon),
                    new OleDbParameter("taskexpfile",t.taskExpFile),
                    new OleDbParameter("taskclstype",t.taskClsType),
                    new OleDbParameter("tasktype", t.taskType)
                };

                QMDBHelper.ExecuteNonQuery(sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="t">任务类</param>
        public void Update(Tasks t)
        {
            try
            {
                string sql = @"update qm_task set 
                                     taskname = ?,
                                     tasksendby = ?,
                                     taskcron = ? ,
                                     taskfile = ?,
                                     taskparm = ?,
                                     taskcreatetime = ?,
                                     taskstate = ?,
                                     taskremark = ?,
                                     taskdbcon = ?,
                                     taskexpfile = ?,
                                     taskclstype = ?,
                                     tasktype = ?
                                where idx = ?";

                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("taskname",t.taskName),
                    new OleDbParameter("tasksendby",t.taskSendby),
                    new OleDbParameter("taskcron",t.taskCron),
                    new OleDbParameter("taskfile",t.taskFile),
                    new OleDbParameter("taskparm",t.taskParm),
                    new OleDbParameter("taskcreatetime",t.taskCreateTime),
                    new OleDbParameter("taskstate",t.taskState),
                    new OleDbParameter("taskremark",t.taskRemark),
                    new OleDbParameter("taskdbcon",t.taskDBCon),
                    new OleDbParameter("taskexpfile",t.taskExpFile),
                    new OleDbParameter("taskclstype",t.taskClsType),
                    new OleDbParameter("tasktype", t.taskType),
                    new OleDbParameter("idx",t.idx)                                                                                                                                                                                                        
                };

                QMDBHelper.ExecuteNonQuery(sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="idx">任务id</param>
        /// <param name="state">状态</param>
        public bool UpdateStatus(string idx, string state)
        {
            bool result = false;
            try
            {
                string sql = "update qm_task set taskstate = ? where idx = ?";
                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("taskstate",state),
                    new OleDbParameter("idx",idx)
                    
                };                                

                QMDBHelper.ExecuteNonQuery(sql, param);

                result = true;
            }
            catch (QMException ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 更新上次开始时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="laststarttime"></param>
        public void UpdateLastStartTime(string idx, DateTime lastStartTime)
        {
            try
            {
                string sql = "update qm_task set tasklaststarttime = ? where idx = ?";
                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("tasklaststarttime",lastStartTime),
                    new OleDbParameter("idx",idx)                    
                };                                

                QMDBHelper.ExecuteNonQuery(sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新上次结束时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastEndTime"></param>
        public void UpdateLastEndTime(string idx, DateTime lastEndTime)
        {
            try
            {
                string sql = "update qm_task set tasklastendtime = ?,taskcount = taskcount + 1  where idx = ?";
                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("tasklastendtime",lastEndTime),
                    new OleDbParameter("idx",idx)                  
                };                                

                QMDBHelper.ExecuteNonQuery(sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新上次错误时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastErrorTime"></param>
        public void UpdateLastErrorTime(string idx, DateTime lastErrorTime)
        {
            try
            {
                string sql = "update qm_task set tasklasterrortime = ?,taskerrorcount = taskerrorcount + 1  where idx = ?";
                OleDbParameter[] param = new OleDbParameter[]
                {
                    new OleDbParameter("tasklasterrortime",lastErrorTime),
                    new OleDbParameter("idx",idx)
                };                              

                QMDBHelper.ExecuteNonQuery(sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }
    }
}
