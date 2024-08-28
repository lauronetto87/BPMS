using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class EmailSendingDisabledException : BaseException
    {
        private EmailSendingDisabledException(string message, Exception innerException) : base(message, innerException) { }

        public static EmailSendingDisabledException Create(Exception innerException)
        {
            return new EmailSendingDisabledException("Email sending is disabled for your entire Amazon SES account.", innerException);
        }
    }
}
