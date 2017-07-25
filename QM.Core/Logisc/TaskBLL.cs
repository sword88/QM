using QM.Core.Data;
using QM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Logisc
{
    public class TaskBLL
    {
        private TaskData tdal = new TaskData();
        private TaskN2MData tmdal = new TaskN2MData();

        /// <summary>
        /// 获得所有任务清单
        /// </summary>
        /// <returns></returns>
        public IList<Tasks> GetList()
        {
            return tdal.GetList();
        }

        /// <summary>
        /// 获得任务参数
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public IList<TasksN2M> GetParms(string taskid)
        {
            return tmdal.GetParms(taskid);
        }

        public TasksN2M GetParm(IList<TasksN2M> parms, string name)
        {
            foreach (var item in parms)
            {
                if (item.attrname == name)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 新的任务
        /// </summary>
        /// <param name="t"></param>
        public void Insert(Tasks t)
        {
            tdal.Insert(t);
        }

        /// <summary>
        /// 任务详细信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Tasks Detail(string idx)
        {
            return tdal.Detail(idx);
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="t"></param>
        public void Update(Tasks t)
        {
            tdal.Update(t);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool DeleteById(string taskId)
        {
            return tdal.UpdateStatus(taskId, "D");
        }

        /// <summary>
        /// 更新上次开始时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastStartTime"></param>
        public void UpdateLastStartTime(string idx, DateTime lastStartTime)
        {
            tdal.UpdateLastStartTime(idx, lastStartTime);
        }

        /// <summary>
        /// 更新上次结束时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastEndTime"></param>
        public void UpdateLastEndTime(string idx, DateTime lastEndTime)
        {
            tdal.UpdateLastEndTime(idx, lastEndTime);
        }

        /// <summary>
        /// 更新上次错误时间
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="lastErrorTime"></param>
        public void UpdateLastErrorTime(string idx, DateTime lastErrorTime)
        {
            tdal.UpdateLastErrorTime(idx, lastErrorTime);
        }
    }
}
