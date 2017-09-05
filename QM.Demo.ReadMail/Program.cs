using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Imap4;
using ActiveUp.Net.Mail;
using log4net;
using QM.Core;
using Oracle.ManagedDataAccess.Client;
using QM.Core.Model;

namespace QM.Demo.ReadMail
{
    class Program : DllTask
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));
        private static string server = "mail.asewh.com";
        private static int port = 143;
        private static string uid = "junxiao_liang@aseglobal.com";
        private static string pwd = "123456789*";
        private static string folder = "B2B";

        static void Main(string[] args)
        {
            log.Debug("程序开始");
            Mail();
        }

        public static void Mail()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("TITLE");
                dt.Columns.Add("BODY");

                Imap4Client client = new Imap4Client();            
                client.Connect(server, port);
                log.Debug(string.Format("连接服务器成功{0},{1}", server, port));

                client.Login(uid, pwd);
                log.Debug(string.Format("登录成功{0},{1}", uid, pwd));

                Mailbox box = client.SelectMailbox(folder);
                log.Debug(string.Format("选择操作文件夹{0},邮件数{1}", folder,box.MessageCount));

                for (int i = box.MessageCount; i > 0; i--)
                {
                    Message msg = box.Fetch.MessageObject(i);

                    if (msg.From.ToString() == "csmgr@sin.infineon.com" && msg.Subject.StartsWith("VF104_"))
                    {
                        dt.Rows.Add(msg.Subject);
                        dt.Rows.Add(msg.BodyText.Text);
                        box.UidDeleteMessage(i, true);
                        box.DeleteMessage(i, true);
                        log.Debug(string.Format("删除邮件,发件人{0},标题{1},正文{2}", msg.From.ToString(), msg.Subject, msg.BodyText.Text));
                    }
                    else if (msg.From.ToString() == "AllianceWH@aseglobal.com" && msg.Subject.StartsWith("[60][Low]MISC-FT-资料缺失请尽快添加"))
                    {
                        box.UidDeleteMessage(i, true);
                        box.DeleteMessage(i, true);
                        log.Debug(string.Format("删除邮件,发件人{0},标题{1},正文{2}", msg.From.ToString(), msg.Subject, msg.BodyText.Text));
                    }
                    else if (msg.From.ToString() == "IRAD2@INFINEON.COM" && msg.Subject.StartsWith("Build Instruction ASE"))
                    {
                        box.UidDeleteMessage(i, true);
                        box.DeleteMessage(i, true);
                        log.Debug(string.Format("删除邮件,发件人{0},标题{1},正文{2}", msg.From.ToString(), msg.Subject, msg.BodyText.Text));
                    }
                }

                

                client.Disconnect();
                log.Debug("断开连接");
            }
            catch (Exception ex)
            {
                log.Fatal(string.Format("出错了,{0}", ex.Message));
            }
        }

        public override void Run()
        {
            log.Debug("程序开始");
            Mail();
        }
    }
}
