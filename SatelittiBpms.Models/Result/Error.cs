namespace SatelittiBpms.Models.Result
{
    public class Error
    {
        public string Message { get; private set; }
        public object Params { get; private set; }

        public Error(string message, object parameters = null)
        {
            Message = message;
            Params = parameters;
        }
    }
}
