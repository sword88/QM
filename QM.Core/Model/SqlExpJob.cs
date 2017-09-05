using QM.Core.Common;
using System;
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
        private string header = "";
        private string filename = "";
        private string filetype = "XLS";
        private string filedate = "";
        private string body = "";
        private string inmail = "";
        private string reportid = "REPORTID";
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
        public SqlExpJob(string dbstr, string sqlstr, string repid, string titlestr, IList<TasksN2M> sparms)
        {

            parms = sparms;
            dbcon = dbstr;
            db = new QMDBHelper(dbstr);
            sql = sqlstr;
            title = sparms.FirstOrDefault(p => p.attrname == "SUBJECT").attrval;
            filename = sparms.Where(x => x.attrname == "EXPFILE").FirstOrDefault().attrval;
            filetype = sparms.Where(x => x.attrname == "EXPTYPE").FirstOrDefault().attrval;
            filedate = DateTime.Now.ToString(sparms.Where(x => x.attrname == "EXPDATE").FirstOrDefault().attrval);
            filepath = sparms.FirstOrDefault(p => p.attrname == "EXPPATH").attrval +
                filename + filedate + "." + filetype;

            QMFile.CreateDir(sparms.FirstOrDefault(p => p.attrname == "EXPPATH").attrval);

            reportid = repid;
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        public void TryRun(string sendby = "MAIL")
        {
            try
            {
                IList<TasksN2M> m_parms = null;
                m_parms = parms.Where(x => x.refname == "SQL").ToList();
                header = m_parms.Where(x => x.attrname == "HEADER").FirstOrDefault().attrval;
                inmail = parms.Where(x => x.refname == "MAIL" && x.attrname == "INMAIL").FirstOrDefault().attrval;
                DataSet ds = db.ExecuteDataset(sql);

                log.Debug(string.Format("[SqlExpJob] 导出文件{0},{1},{2},{3},{4}", title, filepath, filename + filedate, filetype, sql));

                if (filetype == "TXT")
                {
                    QMText txt = new QMText();
                    if (inmail == "X")
                    {
                        body = txt.Export(ds.Tables[0], header);
                    }
                    if (txt.Export(ds.Tables[0], header, filepath, out error) == false)
                    {
                        log.Fatal(string.Format("[SqlExpJob] 导出文件异常{0}", error));
                        //filepath = "";
                    }
                    else
                    {
                        log.Debug("[SqlExpJob] 导出成功");
                    }
                }
                else if (filetype == "CSV")
                {
                    QMText txt = new QMText();
                    body = txt.Export(ds.Tables[0], header);
                    if (txt.ExportCSV(ds.Tables[0], header, filepath, out error) == false)
                    {
                        log.Fatal(string.Format("[SqlExpJob] 导出文件异常{0}", error));
                        //filepath = "";
                    }
                    else
                    {
                        log.Debug("[SqlExpJob] 导出成功");
                    }
                }
                else
                {
                    IExcel ex = new QMExcel(title, filepath);
                    if (ex.Export(ds.Tables[0], title, filepath, out error) == false)
                    {
                        log.Fatal(string.Format("[SqlExpJob] 导出文件异常{0}", error));
                        //filepath = "";
                    }
                    else
                    {
                        log.Debug("[SqlExpJob] 导出成功");
                    }
                }

                foreach (var item in sendby.Split('+'))
                {
                    switch (item)
                    {
                        case "MAIL":
                            try
                            {
                                IMail mail = new QMMail();
                                mail.AddAttachment(filepath);
                                m_parms = parms.Where(x => x.refname == "MAIL").ToList();
                                mail.Subject = m_parms.Where(x => x.attrname == "SUBJECT").FirstOrDefault().attrval +
                                      DateTime.Now.ToString(m_parms.Where(x => x.attrname == "SUBDATE").FirstOrDefault().attrval);
                                mail.AddBody(m_parms.Where(x => x.attrname == "BODY").FirstOrDefault().attrval + body, reportid);

                                foreach (var parm in m_parms.Where(x => x.attrname == "TO" || x.attrname == "CC" || x.attrname == "BCC"))
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
                                    }
                                }

                                if (mail.Send())
                                {
                                    log.Debug("[MAIL] 发送成功");
                                }
                                else
                                {
                                    log.Fatal("[MAIL] 发送失败");
                                    throw new QMException("[MAIL] 发送失败");
                                }
                            }
                            catch (QMException ex)
                            {
                                log.Fatal(string.Format("[MAIL] 发送失败，{0}", ex.Message));
                                throw ex;
                            }
                           
                            break;
                        case "FTP":
                            try
                            {
                                QMFtp ftp = new QMFtp();
                                m_parms = parms.Where(x => x.refname == "FTP").ToList();
                                ftp.server = m_parms.Where(x => x.attrname == "SERVER").FirstOrDefault().attrval;
                                ftp.user = m_parms.Where(x => x.attrname == "ACCOUNT").FirstOrDefault().attrval;
                                ftp.pass = m_parms.Where(x => x.attrname == "PASSWORD").FirstOrDefault().attrval;
                                string path = m_parms.Where(x => x.attrname == "RMOTEPATH").FirstOrDefault().attrval;
                                ftp.ChangeDir(path);
                                log.Debug("[FTP]目录切换成功");
                                if (ftp.OpenUpload(filepath, path + '/' + filename + filedate + '.' + filetype, false))
                                {
                                    log.Debug(string.Format("[FTP]开始上传{0},{1}", filepath, path + '/' + filename + '.' + filetype));
                                    long size = ftp.DoUpload();
                                    log.Debug(string.Format("[FTP]文件大小:{0}", size));
                                }
                                else
                                {
                                    log.Fatal("[FTP]上传失败");
                                }

                                ftp.Disconnect();
                                log.Debug("[FTP] 上传成功");
                            }
                            catch (QMException ex)
                            {
                                log.Fatal(string.Format("[FTP] 发送失败，{0}", ex.Message));
                                throw ex;
                            }
                            break;
                        case "SFTP":

                            break;
                    }

                }

                QMFile.Delete(filepath);
                log.Debug("[SqlExpJob] 附件删除成功");

            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("[SqlExpJob] 异常{0}", ex.Message));
                throw ex;
            }
            finally
            {
                db.Disponse();
            }
        }
    }
}
