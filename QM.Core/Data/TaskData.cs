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
    public class TaskData
    {
        private QMDBHelper qmdb = new QMDBHelper(ConfigurationManager.ConnectionStrings["database"].ConnectionString);

        /// <summary>
        /// 获得所有任务清单
        /// </summary>
        /// <returns>IList[Tasks]()</returns>
        public IList<Tasks> GetTaskList()
        {

            IList<Tasks> task = new List<Tasks>();
            Tasks t = null;
            OracleDataReader dr;
            try
            {
                string sql = "select * from qm_task where taskstate = 'Y' order by taskcreatetime desc";
                dr = qmdb.ExecuteReader(CommandType.Text,sql);
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
                    if (dr["TASKERRORCOUNT"].ToString() != "")
                    {
                        t.taskErrorCount = int.Parse(dr["TASKERRORCOUNT"].ToString());
                    }
                    t.taskType = dr["TASKTYPE"].ToString();
                    t.taskDBCon = dr["TASKDBCON"].ToString();
                    t.taskParm = dr["TASKPARM"].ToString();
                    t.taskFile = dr["TASKFILE"].ToString();
                    //t.taskExpFile = dr["TASKEXPFILE"].ToString();
                    t.taskName = dr["TASKNAME"].ToString();
                    t.taskState = dr["TASKSTATE"].ToString();
                    t.taskCron = dr["TASKCRON"].ToString();
                    t.taskRemark = dr["TASKREMARK"].ToString();
                    t.taskSendby = dr["TASKSENDBY"].ToString();
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

        /// <summary>
        /// 任务详细信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Tasks TaskDetail(string idx)
        {
            Tasks t = null;
            OracleDataReader dr;
            try
            {
                string sql = "select * from qm_task where idx = :idx";
                OracleParameter[] param = new OracleParameter[] {
                    new OracleParameter(":idx",idx)
                };
                dr = qmdb.ExecuteReader(CommandType.Text,sql, param);
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
                    t.taskName = dr["TASKNAME"].ToString();
                    t.taskState = dr["TASKSTATE"].ToString();
                    t.taskCron = dr["TASKCRON"].ToString();
                    t.taskRemark = dr["TASKREMARK"].ToString();
                    t.taskSendby = dr["TASKSENDBY"].ToString();
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

            return t;
        }

        /// <summary>
        /// 新的任务
        /// </summary>
        /// <param name="t"></param>
        /// <param name="n2m"></param>
        /// <returns></returns>
        public bool Insert(Tasks t, IList<TasksN2M> n2m)
        {
            bool result = false;
            qmdb.BeginTransaction();
            try
            {
                InsertTask(t);
                foreach (var item in n2m)
                {
                    InsertParms(item);
                }
                qmdb.Commit();
                result = true;
            }
            catch (QMException ex)
            {
                throw ex;
                qmdb.Rollback();
            }

            return result;
        }

        /// <summary>
        /// 新的任务
        /// </summary>
        /// <param name="t">任务类</param>
        public void InsertTask(Tasks t)
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
                                     taskclstype,
                                     tasktype) 
                            values
                                    (:idx,
                                     :taskname,
                                     :tasksendby,
                                     :taskcron,
                                     :taskfile,
                                     :taskparm,
                                     :taskcreatetime,
                                     :taskstate,
                                     :taskremark,
                                     :taskdbcon,
                                     :taskclstype,
                                     :tasktype)";

                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":idx",t.idx),
                    new OracleParameter(":taskname",t.taskName),
                    new OracleParameter(":tasksendby",t.taskSendby),
                    new OracleParameter(":taskcron",t.taskCron),
                    new OracleParameter(":taskfile",t.taskFile),
                    new OracleParameter(":taskparm",t.taskParm),
                    new OracleParameter(":taskcreatetime",t.taskCreateTime),
                    new OracleParameter(":taskstate",t.taskState),
                    new OracleParameter(":taskremark",t.taskRemark),
                    new OracleParameter(":taskdbcon",t.taskDBCon),
                    new OracleParameter(":taskclstype",t.taskClsType),
                    new OracleParameter(":tasktype", t.taskType)
                };

                qmdb.ExecuteNonQuery(qmdb.p_trans, CommandType.Text, sql, param);
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
        public void UpdateTask(Tasks t)
        {
            try
            {
                string sql = @"update qm_task set 
                                     taskname = :taskname,
                                     tasksendby = :tasksendby,
                                     taskcron = :taskcron ,
                                     taskfile = :taskfile,
                                     taskparm = :taskparm,
                                     taskcreatetime = :taskcreatetime,
                                     taskstate = :taskstate,
                                     taskremark = :taskremark,
                                     taskdbcon = :taskdbcon,
                                     taskexpfile = :taskexpfile,
                                     taskclstype = :taskclstype,
                                     tasktype = :tasktype
                                where idx = :idx";

                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":taskname",t.taskName),
                    new OracleParameter(":tasksendby",t.taskSendby),
                    new OracleParameter(":taskcron",t.taskCron),
                    new OracleParameter(":taskfile",t.taskFile),
                    new OracleParameter(":taskparm",t.taskParm),
                    new OracleParameter(":taskcreatetime",t.taskCreateTime),
                    new OracleParameter(":taskstate",t.taskState),
                    new OracleParameter(":taskremark",t.taskRemark),
                    new OracleParameter(":taskdbcon",t.taskDBCon),
                    new OracleParameter(":taskexpfile",t.taskExpFile),
                    new OracleParameter(":taskclstype",t.taskClsType),
                    new OracleParameter(":tasktype", t.taskType),
                    new OracleParameter(":idx",t.idx)                                                                                                                                                                                                        
                };

                qmdb.ExecuteNonQuery(sql,CommandType.Text, param);
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
                string sql = "update qm_task set taskstate = :taskstate where idx = :idx";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":taskstate",state),
                    new OracleParameter(":idx",idx)
                    
                };

                qmdb.ExecuteNonQuery(sql,CommandType.Text, param);

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
                string sql = "update qm_task set tasklaststarttime = :tasklaststarttime where idx = :idx";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":tasklaststarttime",lastStartTime),
                    new OracleParameter(":idx",idx)                    
                };

                qmdb.ExecuteNonQuery(sql, CommandType.Text,param);
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
                string sql = "update qm_task set tasklastendtime = :tasklastendtime,taskcount = taskcount + 1  where idx = :idx";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":tasklastendtime",lastEndTime),
                    new OracleParameter(":idx",idx)                  
                };

                qmdb.ExecuteNonQuery(sql,CommandType.Text, param);               
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
                string sql = "update qm_task set tasklasterrortime = :tasklasterrortime,taskerrorcount = taskerrorcount + 1  where idx = :idx";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":tasklasterrortime",lastErrorTime),
                    new OracleParameter(":idx",idx)
                };

                qmdb.ExecuteNonQuery(sql,CommandType.Text, param);                
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获得参数
        /// </summary>
        /// <param name="taskid">任务Id</param>
        /// <param name="refname">参数组</param>
        /// <returns></returns>
        public IList<TasksN2M> GetParms(string taskid, string refname = "")
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

        /// <summary>
        /// 新的任务参数
        /// </summary>
        /// <param name="n2m">任务参数</param>
        public void InsertParms(TasksN2M n2m)
        {
            try
            {
                string sql = @"insert into qm_task_n2m 
                                    (idx,
                                     refidx,
                                     refname,
                                     attrname,
                                     attrval) 
                            values
                                    (:idx,
                                     :refidx,
                                     :refname,
                                     :attrname,
                                     :attrval)";

                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":idx",n2m.idx),
                    new OracleParameter(":refidx",n2m.refidx),
                    new OracleParameter(":refname",n2m.refname),
                    new OracleParameter(":attrname",n2m.attrname),
                    new OracleParameter(":attrval",n2m.attrval)
                };

                qmdb.ExecuteNonQuery(qmdb.p_trans, CommandType.Text, sql, param);
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }
    }
}
