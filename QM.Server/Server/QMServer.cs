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

    }
}
