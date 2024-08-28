using Amazon.SimpleEmail.Model;
using SatelittiBpms.Mail.Mailer;
using SatelittiBpms.Mail.Models;
using SatelittiBpms.Mail.Models.Config;

namespace SatelittiBpms.Mail.Tests.ProtectedWrapper
{
    public class AwsSimpleEmailService_MailerTestable : AwsMailer
    {
        public AwsSimpleEmailService_MailerTestable() : base()
        {
        }

        public virtual SendEmailRequest CreateRequestTestMethod(MailMessage mail, SESMailMessageConfig config)
        {
            return base.CreateRequest(mail, config);
        }
    }
}
