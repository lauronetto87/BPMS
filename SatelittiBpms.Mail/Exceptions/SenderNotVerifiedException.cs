using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class SenderNotVerifiedException : BaseException
    {
        private SenderNotVerifiedException(string message, Exception innerException) : base(message, innerException)
        { }

        public static SenderNotVerifiedException Create(Exception innerException)
        {
            return new SenderNotVerifiedException("The message could not be sent because Amazon SES could not read the MX record required to use the specified MAIL FROM domain.", innerException);
        }
    }
}
