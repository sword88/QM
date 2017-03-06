using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Quartz;

namespace QM.Server
{
    //
    //http://stackoverflow.com/questions/8392596/how-can-i-run-quartz-net-jobs-in-a-separate-appdomain
    //
    class QMIsolatedJob : IInterruptableJob, IDisposable
    {
        private readonly Type JobType;                

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void Interrupt()
        {
            throw new NotImplementedException();
        }
    }
}
