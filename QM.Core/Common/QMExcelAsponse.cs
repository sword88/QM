using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Aspose.Cells;
using Aspose.Cells.Drawing;
using System.IO;

namespace QM.Core.Common
{
    /// <summary>
    /// EXCEL Asponse操作类
    /// </summary>
    public class QMExcelAsponse
    {
        /// <summary>
        /// DataTable导出Excel
        /// </summary>
        /// <param name="dt">DataTable数据源</param>
        /// <param name="filepath">导出路径</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public bool DataTableToExcel(DataTable dt, string filepath, out string error)
        {            
            bool result = false;
            int iRow = 0;
            error = "";

            try
            {
                if (dt == null)
                {
                    error = "QMExcel->DataTableToExcel:dt为空";
                    return result;
                }

                new License().SetLicense("Aspose.Cells.lic");
                //创建工作薄
                Workbook workbook = new Workbook();
                //为单元格添加样式
                Style style = workbook.CreateStyle();
                //设置单元格式居中
                style.HorizontalAlignment = TextAlignmentType.Center;
                //设置背景颜色
                style.ForegroundColor = Color.FromArgb(205, 205, 222);
                style.Pattern = BackgroundType.Solid;
                style.Font.IsBold = true;

                //创建工作表
                Worksheet worksheet = workbook.Worksheets[0];
                Cells cells = worksheet.Cells;

                //列名
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn col = dt.Columns[i];
                    string columnName = col.Caption ?? col.ColumnName;
                    workbook.Worksheets[0].Cells[iRow, i].PutValue(columnName);
                    workbook.Worksheets[0].Cells[iRow, i].SetStyle(style);
                }

                iRow++;

                //行值
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        workbook.Worksheets[0].Cells[iRow, i].PutValue(dr[i].ToString());
                    }

                    iRow++;
                }

                //自适应列宽
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    workbook.Worksheets[0].AutoFitColumn(i, 0, 150);
                }

                workbook.Worksheets[0].FreezePanes(1, 0, 1, dt.Columns.Count);
                workbook.Save(filepath);
                result = true;
            }
            catch (QMException ex)
            {
                error = ex.Message;
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Excel导入DataTable
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="dt">DataTable</param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ExcelToDataTable(string filepath, out DataTable dt, out string error)
        {
            bool result = false;
            dt = null;
            error = "";

            try
            {
                if (QMFile.FileExists(filepath) == false)
                {
                    error = "QMExcel->ExcelToDataTable:filepath文件不存在";
                    return result;
                }

                Workbook workbook = new Workbook(filepath);                
                Worksheet worksheet = workbook.Worksheets[0];
                dt = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxRow + 1, worksheet.Cells.MaxColumn + 1);
                
                result = true;
            }
            catch (QMException ex)
            {
                error = ex.Message;
                throw ex;
            }

            return result;
        }
    }
}
