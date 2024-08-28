using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SatelittiBpms.Models.HandleException;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SatelittiBpms.Authentication.Middleware
{
    public class AuthenticationHandleExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        internal readonly ILogger<AuthenticationHandleExceptionMiddleware> _logger;
        private const string SchemeDelimiter = "://";

        public AuthenticationHandleExceptionMiddleware(
            RequestDelegate next,
            ILogger<AuthenticationHandleExceptionMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context
        )
        {
            if (context.Request.Method == Satelitti.Authentication.Constants.REQUEST_METHOD_OPTIONS)
            {
                await _next(context);
                return;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(
            HttpContext context,
            Exception exception
        )
        {
            var url = $"({context.Request.Method}) {GetDisplayUrl(context).ToLower()}";

            if (exception is IHandleException handleException)
            {
                var problemDetail = handleException.GetDetails();

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = problemDetail.Status;

                _logger.LogError(exception, $"sessionId: {context.TraceIdentifier} - Url: {url} - Status: {context.Response.StatusCode} - response: {JsonConvert.SerializeObject(problemDetail)} #exception");

                return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetail));
            }
            else
            {
                _logger.LogError(exception, $"sessionId: {context.TraceIdentifier} - Url: {url} - Status: {context.Response.StatusCode} #exception");
                throw exception;
            }

            throw exception;
        }

        private string GetDisplayUrl(HttpContext context)
        {
            var scheme = context.Request.Scheme ?? string.Empty;
            var host = context.Request.Host.Value ?? string.Empty;
            var pathBase = context.Request.PathBase.Value ?? string.Empty;
            var path = context.Request.Path.Value ?? string.Empty;
            var queryString = context.Request.QueryString.Value ?? string.Empty;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new StringBuilder(length)
                .Append(scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }
    }
}
