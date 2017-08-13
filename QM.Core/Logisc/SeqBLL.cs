using QM.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Logisc
{
    public class SeqBLL
    {
        SequenceData sd = new SequenceData();

        public string GetIdx(string prefix = "")
        {
            return sd.GetIdx(prefix);
        }
    }
}
