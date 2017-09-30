using System;
using System.Collections.Generic;
using System.Data;
using QM.Core.Common;
using QM.Core.Model;

namespace QM.Sql2Excel
{
    /// <summary>
    /// sql导出excel
    /// </summary>
    public class SqlJob : DllTask
    {
        //数据库连接字符串
        private static string dbcon = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        private static string error = "";
        /// <summary>
        /// sql语句
        /// </summary>
        private static string sql = "";
        private QMDBHelper db;
        /// <summary>
        /// excel内容表头
        /// </summary>
        private static string title = "";
        /// <summary>
        /// 文件路径/文件名
        /// </summary>
        private static string filepath = "";

        /// <summary>
        /// SQL导出excel初始化
        /// </summary>
        /// <param name="dbstr">数据库连接字符串</param>
        /// <param name="sqlstr">sql语句</param>
        /// <param name="titlestr">excel内容表头</param>
        /// <param name="filestr">文件路径/文件名</param>
        public SqlJob(string dbstr, string sqlstr, string titlestr, string filestr)
        {
            dbcon = dbstr;
            db = new QMDBHelper(dbstr);
            sql = sqlstr;
            title = titlestr;
            filepath = filestr;
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        public override void Run()
        {
            DataSet ds = QMDBHelper.ExecuteDataset(sql);
            QMExcel ex = new QMExcel(title, filepath);
            if (ex.Export(ds.Tables[0], title, filepath, out error) == false)
            {
                TaskLog log = new TaskLog();
                log.message = error;
                QMLog.Debug(log);
            }
        }
    }
}
