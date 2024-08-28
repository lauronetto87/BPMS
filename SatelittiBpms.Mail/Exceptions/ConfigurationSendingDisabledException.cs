using System;

namespace SatelittiBpms.Mail.Exceptions
{
    public class ConfigurationSendingDisabledException : BaseException
    {
        private ConfigurationSendingDisabledException(string message, Exception innerException) : base(message, innerException) { }

        public static ConfigurationSendingDisabledException Create(Exception innerException)
        {
            return new ConfigurationSendingDisabledException("Email sending is disabled for the configuration set.", innerException);
        }
    }
}
