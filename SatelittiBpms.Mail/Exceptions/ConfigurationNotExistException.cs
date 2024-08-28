using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class ConfigurationNotExistException : BaseException
    {
        private ConfigurationNotExistException(string message, Exception innerException) : base(message, innerException) { }

        public static ConfigurationNotExistException Create(Exception innerException)
        {
            return new ConfigurationNotExistException("The configuration set does not exist.", innerException);
        }
    }
}
