using QM.Core.Common;
using System.Data;
using QM.Core.Excel;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Data;

namespace QM.Core.Model
{
    /// <summary>
    /// sql导出excel
    /// </summary>
    public class SqlExpJob
    {
        private static ILogger log;

        //数据库连接字符串
        private string dbcon = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        private string error = "";
        /// <summary>
        /// sql语句
        /// </summary>
        private string sql = "";
        private QMDBHelper db;
        /// <summary>
        /// excel内容表头
        /// </summary>
        private string title = "";
        /// <summary>
        /// 文件路径/文件名
        /// </summary>
        private string filepath = "";

        /// <summary>
        /// SQL导出excel初始化
        /// </summary>
        /// <param name="dbstr">数据库连接字符串</param>
        /// <param name="sqlstr">sql语句</param>
        /// <param name="titlestr">excel内容表头</param>
        /// <param name="filestr">文件路径/文件名</param>
        public SqlExpJob(string dbstr, string sqlstr, string titlestr, string filestr)
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
        public void TryRun()
        {
            try
            {
                DataSet ds = QMDBHelper.ExecuteDataset(sql);
                QMExcel ex = new QMExcel(title, filepath);
                if (ex.Export(ds.Tables[0], title, filepath, out error) == false)
                {
                    log.Debug(error);
                }
            }
            catch (QMException ex)
            {
                throw ex;
            }
        }
    }
}
