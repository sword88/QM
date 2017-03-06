using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.ComponentModel;
using Quartz.Util;

namespace QM.Server
{
    public class QMJobFactory : IJobFactory
    {

        public QMJobFactory()
        {
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IJobDetail jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;
            
            try
            {
                return ObjectUtils.InstantiateType<IJob>(jobType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
