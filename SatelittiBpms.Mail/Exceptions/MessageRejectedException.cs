using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class MessageRejectedException : BaseException
    {
        private MessageRejectedException(string message, Exception innerException) : base(message, innerException) { }

        public static MessageRejectedException Create(Exception innerException)
        {
            return new MessageRejectedException("The action failed, and the message could not be sent.Check the error stack for more information about what caused the error.", innerException);
        }
    }
}
