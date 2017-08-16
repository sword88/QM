using QM.Core.Exception;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Excel
{
    public class QMText : IExcel
    {
        public string Export(DataTable dt, string header)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<table border=\"0\" width=\"100%\" bgcolor=\"#e2e2e2\">");

            #region 表头
            strHTMLBuilder.Append("<tr >");
            if (header == "")
            {
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myColumn);
                    strHTMLBuilder.Append("</td>");

                }
            }
            else
            {
                foreach (string myColumn in header.Split(';'))
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myColumn);
                    strHTMLBuilder.Append("</td>");

                }
            }
            strHTMLBuilder.Append("</tr>");
            #endregion

            #region 正文
            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }
            #endregion

            //Close tags. 
            strHTMLBuilder.Append("</table>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;
        }

        public bool Export(DataTable dt, string header, string filepath, out string error)
        {
            bool result = false;
            error = "";

            try
            {
                if (dt == null)
                {
                    error = "QMText->DataTableToExcel:dt为空";
                    return result;
                }

                using (FileStream fs = new FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default))
                    {
                        string data = "";

                        #region header
                        if (header == "")
                        {
                            //写出列名称
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                data += dt.Columns[i].ColumnName.ToString();
                                if (i < dt.Columns.Count - 1)
                                {
                                    data += "\t";
                                }
                            }
                        }
                        else
                        {
                            string[] headeritem = header.Split(';');
                            for (int i = 0; i < headeritem.Length; i++)
                            {
                                data += headeritem[i].ToString();
                                if (i < headeritem.Length - 1)
                                {
                                    data += "\t";
                                }
                            }
                        }
                        sw.WriteLine(data);
                        #endregion

                        //写出各行数据
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            data = "";
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                data += dt.Rows[i][j].ToString();
                                if (j < dt.Columns.Count - 1)
                                {
                                    data += "\t";
                                }
                            }
                            sw.WriteLine(data);
                        }

                        fs.Flush();
                    }
                }

                result = true;
            }
            catch (IOException iex)
            {
                error = iex.Message;
                //throw iex;
            }
            catch (QMException ex)
            {
                error = ex.Message;
                //throw ex;
            }

            return result;
        }

        public bool ExportCSV(DataTable dt, string header, string filepath, out string error)
        {
            bool result = false;
            error = "";

            try
            {
                if (dt == null)
                {
                    error = "QMText->DataTableToExcel:dt为空";
                    return result;
                }

                FileStream fs = new FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                string data = "";
                if (header == "")
                {
                    //写出列名称
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        data += dt.Columns[i].ColumnName.ToString();
                        if (i < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                }
                else
                {
                    string[] headeritem = header.Split(';');
                    for (int i = 0; i < headeritem.Length; i++ )
                    {
                        data += headeritem[i].ToString();
                        if (i < headeritem.Length - 1)
                        {
                            data += ",";
                        }
                    }
                }
                sw.WriteLine(data);

                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        data += dt.Rows[i][j].ToString();
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }

                sw.Close();
                fs.Close();

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
