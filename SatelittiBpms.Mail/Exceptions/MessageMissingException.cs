namespace SatelittiBpms.Mail.Exceptions
{
    public class MessageMissingException : BaseException
    {
        private MessageMissingException(string message) : base(message)
        { }

        public static MessageMissingException Create()
        {
            return new MessageMissingException("The mail message argument is missing.");
        }
    }
}
