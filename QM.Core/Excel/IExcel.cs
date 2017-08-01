using System;
using System.Data;

namespace QM.Core.Excel
{
    interface IExcel
    {      
        bool Export(DataTable dt, string header, string filepath, out string error);
        //bool Import(string filepath, out DataTable dt, out string error);
    }
}
