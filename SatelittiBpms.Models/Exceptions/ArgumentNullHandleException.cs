using SatelittiBpms.Models.HandleException;
using System;
using System.Net;

namespace SatelittiBpms.Models.Exceptions
{
    public class ArgumentNullHandleException : ArgumentNullException, IHandleException
    {
        public string Title { get; set; }
        public ArgumentNullHandleException(string title)
        {
            Title = title;
        }

        public ProblemDetails GetDetails()
        {
            return new ProblemDetails
            {
                Detail = Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = Title ?? nameof(ArgumentNullHandleException)
            };
        }
    }
}
