using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using QM.Core.Log;
using QM.Core.Model;
using QM.Core.QuartzNet;
using QM.Core.Environments;

namespace QM.Server
{
    class QMServer : ServiceControl 
    {
        /// <summary>
        /// 单实例任务管理
        /// </summary>
        private static QMBaseServer _server = new QMBaseServer();        

        /// <summary>
        /// 初始化
        /// </summary>
        public QMServer()
        {
        }


        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {                        
            return _server.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            return _server.Stop();
        }

    }
}
