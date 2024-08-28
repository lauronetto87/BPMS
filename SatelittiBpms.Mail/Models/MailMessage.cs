using System.Collections.Generic;

namespace SatelittiBpms.Mail.Models
{
    public class MailMessage
    {
        public string Sender { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public string Subject { get; set; }
        public MailBody Body { get; set; }
    }
}
