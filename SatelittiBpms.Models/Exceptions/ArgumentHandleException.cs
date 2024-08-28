using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Result;
using System;
using System.Collections.Generic;
using System.Net;

namespace SatelittiBpms.Models.Exceptions
{
    public class ArgumentHandleException : ArgumentException, IHandleException
    {
        public string Title { get; set; }
        public IList<Error> Errors { get; set; } = new List<Error>();

        public ArgumentHandleException(string title, IList<Error> errors)
        {
            Title = title;
            Errors = errors;
        }

        public ProblemDetails GetDetails()
        {
            return new ProblemDetails
            {
                Detail = Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = Title ?? nameof(ArgumentHandleException),
                Errors = Errors
            };
        }
    }
}
