using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Core.Mail
{
    public interface IMail
    {
        string Subject { get; set; }
        string From { get; set; }
        string FromName { get; set; }
        string MailDomain { get; set; }
        int MailDomainPort { get; set; }
        void AddBody(string content, string reportid);
        bool AddRecipient(string username);
        bool AddRecipientCC(string username);
        bool AddRecipientBCC(string username);
        void AddAttachment(string path);
        bool Send();
    }
}
