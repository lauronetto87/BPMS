using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string message) : base(message)
        { }

        public BaseException(string message, Exception innerException) : base(message, innerException)
        { }

        public string LogMessage
        {
            get
            {
                return $"ErroMessage: {Message} || StackTrace: {StackTrace} {(InnerException != null ? $"|| InnerExceptionMessage : {InnerException.Message} || InnerExceptionStackTrace: {InnerException.StackTrace}" : String.Empty)}";
            }
        }
    }
}
