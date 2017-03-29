using System;
using System.Data;

namespace QM.Core.Excel
{
    interface IExcel
    {
        bool DataTableToExcel(DataTable dt, string filepath, out string error);
        bool ExcelToDataTable(string filepath, out DataTable dt, out string error);
    }
}
