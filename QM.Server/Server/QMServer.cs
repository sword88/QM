using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.Core.Log;
using QM.Core.Model;
using QM.Core.QuartzNet;

namespace QM.Server
{
    class QMServer 
    {
        /// <summary>
        /// 单实例任务管理
        /// </summary>
        private static QMBaseServer _server = QMBaseServer.CreateInstance();

        /// <summary>
        /// 初始化
        /// </summary>
        public QMServer()
        {
            //初始化加载DB
            _server.InitLoadTaskList();
        }


        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {                        
            return _server.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            return _server.Stop();
        }

        /// <summary>
        /// 暂时所有任务
        /// </summary>
        /// <returns></returns>
        public bool Pause()
        {
            return _server.PauseAll();
        }

        /// <summary>
        /// 恢复所有任务
        /// </summary>
        /// <returns></returns>
        public bool Resume()
        {
            return _server.ResumeAll();
        }
    }
}
