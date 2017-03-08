using System;
using System.Collections.Generic;
using System.Data;
using QM.Core.Common;
using QM.Core.Model;

namespace QM.Excel
{
    public class Class1 : DllTask
    {
        private static string dbcon = "Provider=MSDAORA.1;Data Source=WHDB;Password=wh123;User ID=whfront";
        private static string error;
        public override void Run()
        {
            QMDBHelper db = new QMDBHelper(dbcon);
            DataSet ds = QMDBHelper.ExecuteDataset("select * from AF_LOGIN_HIS");
            QMExcel ex = new QMExcel();
            if (ex.DataTableToExcel(ds.Tables[0], @"E:\ASECode\Test\QM.git\QM.Excel\bin\Debug\1.xls", out error) == false)
            {
                TaskLog log = new TaskLog();
                log.message = error;
                QMLog.Debug(log);
            }
        }
    }
}
