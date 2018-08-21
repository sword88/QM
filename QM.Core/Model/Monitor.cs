using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Model
{
    public class Monitor : DllTask
    {
        public int Interval
        {
            get
            {
                return 1000 * 60;   //每1分扫描 
            }
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
