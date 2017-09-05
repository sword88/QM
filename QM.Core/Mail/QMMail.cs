using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Mime;
using System.Net.Mail;
using QM.Core.Exception;
using QM.Core.Environments;
using QM.Core.Log;

namespace QM.Core.Mail
{
    /// <summary>
    /// 邮件发送类
    /// </summary>
    public class QMMail : IMail
    {
        private ILogger log = QMStarter.CreateQMLogger(typeof(QMMail));
        private string _subject;
        //private string _body;
        private string _from = "allancewh@aseglobal.COM";
        private string _fromName = "12ljx12";
        private string _recipientName;
        private string _mailDomain = "10.68.10.8";
        private int _mailserverport = 25;
        private string _username= "allancewh@aseglobal.COM";
        private string _password="123";
        private bool _html;
        private string _priority;
        private MailMessage myEmail = new MailMessage();

        public QMMail()
        { }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject
        {
            get
            {
                return this._subject;
            }
            set
            {
                this._subject = value;
            }
        }

        /// <summary>
        /// 邮件正文
        /// </summary>
        //public string Body
        //{
        //    get
        //    {
        //        return this._body;
        //    }
        //    set
        //    {
        //        this._body = value ;
        //    }
        //}


        /// <summary>
        /// 发件人地址
        /// </summary>
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                this._from = value;
            }
        }


        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string FromName
        {
            get
            {
                return this._fromName;
            }
            set
            {
                this._fromName = value;
            }
        }


        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string RecipientName
        {
            get
            {
                return this.myEmail.To.ToString();
            }
        }

        /// <summary>
        /// 邮箱域
        /// </summary>
        public string MailDomain
        {
            get
            {
                return this._mailDomain;
            }
            set
            {
                this._mailDomain = value;
            }
        }

        /// <summary>
        /// 邮件服务器端口号
        /// </summary>	
        public int MailDomainPort
        {
            set
            {
                this._mailserverport = value;
            }
            get
            {
                return this._mailserverport;
            }
        }


        /// <summary>
        /// SMTP认证时使用的用户名
        /// </summary>
        public string MailServerUserName
        {
            set
            {
                if (value.Trim() != "")
                {
                    this._username = value.Trim();
                }
                else
                {
                    this._username = "";
                }
            }
            get
            {
                return _username;
            }
        }

        /// <summary>
        /// SMTP认证时使用的密码
        /// </summary>
        public string MailServerPassWord
        {
            set
            {
                this._password = value;
            }
            get
            {
                return _password;
            }
        }

        /// <summary>
        ///  是否Html邮件
        /// </summary>
        public bool Html
        {
            get
            {
                return this._html;
            }
            set
            {
                this._html = value;
            }
        }

        /// <summary>
        /// 发送优先级
        /// </summary>
        public string Priority
        {
            get
            {
                return this._priority;
            }
            set
            {
                this._priority = value;
            }
        }



        //收件人的邮箱地址
        public bool AddRecipient(string username)
        {
            if (username == "") { return true; }
            foreach (var item in username.Split(';'))
            {
                this.myEmail.To.Add(item);
            }            

            return true;
        }


        //抄送人的邮箱地址
        public bool AddRecipientCC(string username)
        {
            if(username == "") { return true; }
            foreach (var item in username.Split(';'))
            {
                this.myEmail.CC.Add(item);
            }            

            return true;
        }

        //密送人的邮箱地址
        public bool AddRecipientBCC(string username)
        {
            if (username == "") { return true; }
            foreach (var item in username.Split(';'))
            {
                this.myEmail.Bcc.Add(item);
            }            

            return true;
        }
       

        /// <summary>
        /// 将字符串编码为Base64字符串
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        private string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="path"></param>
        public void AddAttachment(string path)
        {
            Attachment attach = new Attachment(path, MediaTypeNames.Application.Octet);
            myEmail.Attachments.Add(attach);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public bool Send()
        {
            bool result = false; 
            Encoding eEncod = Encoding.GetEncoding("utf-8");
            myEmail.From = new System.Net.Mail.MailAddress(this.From, this.Subject, eEncod);
            myEmail.Subject = this.Subject;
            myEmail.IsBodyHtml = true;
            myEmail.Priority = System.Net.Mail.MailPriority.Normal;
            myEmail.BodyEncoding = Encoding.GetEncoding("utf-8");
            //myEmail.BodyFormat = this.Html?MailFormat.Html:MailFormat.Text; //邮件形式，.Text、.Html 

            //优先级
            switch (this._priority)
            {
                case "High":
                    myEmail.Priority = MailPriority.High;
                    break;
                case "Low":
                    myEmail.Priority = MailPriority.Low;
                    break;
                default:
                    myEmail.Priority = MailPriority.Normal;
                    break;
            }

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = this.MailDomain;
            smtp.Port = this.MailDomainPort;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential(this.MailServerUserName, this.MailServerPassWord);            
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            //当不是25端口(gmail:587)
            if (this.MailDomainPort != 25)
            {
                smtp.EnableSsl = true;
            }
            else
            {
                smtp.EnableSsl = false;
            }
            System.Web.Mail.SmtpMail.SmtpServer = this.MailDomain;

            try
            {
                smtp.Send(myEmail);
                result = true;
            }
            catch (SmtpException ex)
            {
                log.Fatal(string.Format("QMMail=>Send发生严重错误，{0}", ex.Message));
                throw ex;                
            }
            catch (QMException ex)
            {
                log.Fatal(string.Format("QMMail=>Send发生严重错误，{0}", ex.Message));
                throw ex;
            }
            finally
            {
                //一定要释放该对象,否则无法删除附件
                foreach (Attachment attach in myEmail.Attachments)
                {
                    attach.Dispose();
                }
                myEmail.Dispose();
                smtp.Dispose();
            }

            return result;
        }


        /// <summary>
        /// 邮件整体样式
        /// </summary>
        /// <returns></returns>
        public void AddBody(string body,string reportId)
        {
            StringBuilder sb = new StringBuilder();
            //css
            sb.Append("<style type=\"text/css\">");
            sb.Append("html{word-wrap:break-word;}");
            sb.Append("body{font-size:14px;font-family:arial,verdana,sans-serif;line-height:1.666;padding:10px 8px;margin:0;overflow:auto}");
            sb.Append("pre {");
            sb.Append("white-space: pre-wrap;");
            sb.Append("white-space: -moz-pre-wrap;");
            sb.Append("white-space: -pre-wrap;");
            sb.Append("white-space: -o-pre-wrap;");
            sb.Append("word-wrap: break-word;");
            sb.Append("} ");
            sb.Append("html, body {margin: 0; padding: 0; font-size: 12px;font-family:微软雅黑,宋体,Arial;}");
            sb.Append(".gray{color:#C0C0C0;padding:0 !important;}");
            sb.Append("#main{padding:5px}");
            sb.Append("#main div{padding:0 0 30px 0;}");
            sb.Append("#main .content{text-indent:20px;}");
            sb.Append("#main table{font-size:12px;border-collapse:collapse;font-family:微软雅黑,宋体,Arial;}  .header{background-color:#F6F8F8;font-size:13px;}");
            sb.Append("#main #code{color:#FF0000;padding:3px;background-color:#C0C0C0;font-size:16px;}");
            sb.Append("</style>");
            //body
            sb.Append("<div id=\"main\">");
            sb.Append(body);
            sb.Append("</div>");
            //sign
            sb.Append("  <br>");
            sb.Append("<div class=\"content gray\">Report ID=" + reportId + "</ div>");
            sb.Append("<div class=\"content gray\">Please Note:  This message is a System Generated E-Mail.<br/>");
            sb.Append(" Please don't reply to this sender.<br/>");
            sb.Append(" If you have any question about this mail, please contact helpdesk.");
            sb.Append("</ div>");

            myEmail.Body = sb.ToString();
        }

    }
}
