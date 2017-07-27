using QM.Core.Common;
using System.Data;
using System.Linq;
using System.Collections;
using QM.Core.Files;
using QM.Core.Excel;
using QM.Core.Exception;
using QM.Core.Log;
using QM.Core.Data;
using QM.Core.Environments;
using System.Collections.Generic;
using QM.Core.Mail;

namespace QM.Core.Model
{
    /// <summary>
    /// sql导出excel
    /// </summary>
    public class SqlExpJob
    {
        private static ILogger log = QMStarter.CreateQMLogger(typeof(SqlExpJob));

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

        private IList<TasksN2M> parms = null;


        /// <summary>
        /// SQL导出excel初始化
        /// </summary>
        /// <param name="dbstr">数据库连接字符串</param>
        /// <param name="sqlstr">sql语句</param>
        /// <param name="titlestr">excel内容表头</param>
        /// <param name="filestr">文件路径/文件名</param>
        public SqlExpJob(string dbstr, string sqlstr, string titlestr,IList<TasksN2M> sparms)
        {

            parms = sparms;            
            dbcon = dbstr;
            db = new QMDBHelper(dbstr);
            sql = sqlstr;
            title = sparms.FirstOrDefault(p=>p.attrname=="SUBJECT").attrval;
            filepath = sparms.FirstOrDefault(p=>p.attrname=="EXPPATH").attrval +
                sparms.FirstOrDefault(p => p.attrname == "EXPFILE").attrval;

            QMFile.CreateDir(sparms.FirstOrDefault(p => p.attrname == "EXPPATH").attrval);
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        public void TryRun(string sendby = "MAIL")
        {
            try
            {
                DataSet ds = db.ExecuteDataset(sql);
                QMExcel ex = new QMExcel(title, filepath);
                log.Debug(string.Format("[SqlExpJob] 导出文件{0},{1},{2}", title,filepath,sql));

                if (ex.Export(ds.Tables[0], title, filepath, out error) == false)
                {
                    log.Debug(string.Format("[SqlExpJob] 导出文件异常{0}", error));
                }
                else
                {
                    log.Debug("[SqlExpJob] 导出成功");

                    foreach (var item in sendby.Split('+'))
                    {
                        switch (item)
                        {
                            case "MAIL":
                                IMail mail = new QMMail();
                                mail.AddAttachment(filepath);
                                foreach (var parm in parms.Where(x => x.refname == "MAIL"))
                                {
                                    switch (parm.attrname)
                                    {
                                        case "TO":
                                            mail.AddRecipient(parm.attrval);
                                            break;
                                        case "CC":
                                            mail.AddRecipientCC(parm.attrval);
                                            break;
                                        case "BCC":
                                            mail.AddRecipientBCC(parm.attrval);
                                            break;
                                        case "SUBJECT":
                                            mail.Subject = parm.attrval;
                                            break;
                                        case "BODY":
                                            mail.AddBody(parm.attrval, "REPORTID");
                                            break;
                                    }
                                }
                                mail.Send();

                                break;
                            case "FTP":

                                break;
                            case "SFTP":

                                break;
                        }
                    }
                }
            }
            catch (QMException ex)
            {
                log.Debug(string.Format("[SqlExpJob] 异常{0}", ex.Message));
                throw ex;
            }
        }
    }
}
