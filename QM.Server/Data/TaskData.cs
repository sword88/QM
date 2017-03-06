using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using QM.Server;
using QM.Core.Common;
using QM.Core.Model;

namespace QM.Server.Data
{
    /// <summary>
    /// 任务数据库操作
    /// </summary>
    public class TaskData : QMBaseData
    {
        private ILog log = LogManager.GetLogger(typeof(TaskData));

        /// <summary>
        /// 添加新任务
        /// </summary>
        /// <param name="task">任务信息</param>
        public void AddTask(Tasks task)
        {
            try
            {
                var obj = Mapper.Insert("INSERT_TASK", task);
            }
            catch (QMException ex)
            {
                log.Fatal(ex);
            }
        }

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="idx">任务ID</param>
        /// <param name="taskcategory">任务分类</param>
        /// <param name="taskcategory">任务类型</param>
        /// <param name="taskstate">任务状态</param>
        /// <returns></returns>
        public IList<Tasks> GetTaskList(string idx,string taskcategory,string tasktype,string taskstate)
        {
            Hashtable hs = new Hashtable();
            hs.Add("idx", idx);
            hs.Add("taskcategory", taskcategory);
            hs.Add("tasktype", tasktype);
            hs.Add("taskstate", taskstate);
            var list = Mapper.QueryForList<Tasks>("SELECT_TASK", hs);

            return list;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="idx">任务编号</param>
        public void DeleteTask(string idx)
        {
            try
            {
                Hashtable hs = new Hashtable();
                hs.Add("idx", idx);
                Mapper.Delete("DELETE_TASK", hs);
            }
            catch (QMException ex)
            {
                log.Fatal(ex);
            }
        }

        /// <summary>
        /// 更新上次开始时间
        /// </summary>
        /// <param name="idx">任务ID</param>
        /// <param name="time">任务时间</param>
        /// <returns></returns>
        public int UpdateLastStartTime(string idx,DateTime time)
        {
            int result = 0;

            try
            {
                Hashtable hs = new Hashtable();
                hs.Add("idx", idx);
                hs.Add("tasklaststarttime", time);
                Mapper.Update("", hs);
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("更新上次开始时间出错,错误信息：{0}",ex.Message));
            }

            return result;
        }

        /// <summary>
        /// 更新上次结束时间
        /// </summary>
        /// <param name="idx">任务ID</param>
        /// <param name="time">任务时间</param>
        /// <returns></returns>
        public int UpdateLastEndTime(string idx, DateTime time)
        {
            int result = 0;

            try
            {
                Hashtable hs = new Hashtable();
                hs.Add("idx", idx);
                hs.Add("tasklastendtime", time);
                Mapper.Update("", hs);
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("更新上次结束时间出错,错误信息：{0}", ex.Message));
            }

            return result;
        }

        /// <summary>
        /// 更新错误统计数
        /// </summary>
        /// <param name="idx">任务ID</param>
        /// <param name="time">任务时间</param>
        /// <returns></returns>
        public int UpdateTaskError(string idx, DateTime time)
        {
            int result = 0;

            try
            {
                Hashtable hs = new Hashtable();
                hs.Add("idx", idx);
                hs.Add("tasklasterrortime", time);
                hs.Add("taskerrorcountplus", true);
                Mapper.Update("", hs);
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("更新错误统计总数出错,错误信息：{0}", ex.Message));
            }

            return result;
        }

        /// <summary>
        /// 更新成功统计数
        /// </summary>
        /// <param name="idx">任务ID</param>
        /// <param name="time">任务时间</param>
        /// <returns></returns>
        public int UpdateTaskSuccess(string idx, DateTime time)
        {
            int result = 0;

            try
            {
                Hashtable hs = new Hashtable();
                hs.Add("idx", idx);
                hs.Add("taskruncountplus", true);
                hs.Add("taskerrorcountclearzero", true);
                Mapper.Update("", hs);
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("更新成功统计总数出错,错误信息：{0}", ex.Message));
            }

            return result;
        }

    }
}
