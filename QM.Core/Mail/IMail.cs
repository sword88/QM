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
        string Body { get; set; }
        string From { get; set; }
        string FromName { get; set; }
        string RecipientName { get; set; }
        string MailDomain { get; set; }
        int MailDomainPort { get; set; }
        bool AddRecipient(params string[] username);
        bool AddRecipientCC(params string[] username);
        bool AddRecipientBCC(params string[] username);
        void AddAttachment(string path);
        bool Send();
    }
}
