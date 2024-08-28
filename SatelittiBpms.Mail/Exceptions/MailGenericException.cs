using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class MailGenericException : BaseException
    {
        private MailGenericException(string message, Exception innerException) : base(message, innerException) { }

        public static MailGenericException Create(Exception innerException)
        {
            return new MailGenericException("A generic error occurred while sending email. Check the Stack for more information.", innerException);
        }
    }
}
