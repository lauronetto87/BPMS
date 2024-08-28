using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SatelittiBpms.Authentication.Middleware;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.Authentication.Tests
{
    public class AuthenticationHandleExceptionMiddlewareTest
    {
        Mock<HttpContext> _mockHttpContext;
        Mock<HttpRequest> _mockHttpRequest;
        Mock<HttpResponse> _mockHttpResponse;
        Mock<ILogger<AuthenticationHandleExceptionMiddleware>> _mockLogger;

        [SetUp]
        public void init()
        {
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();
            _mockLogger = new Mock<ILogger<AuthenticationHandleExceptionMiddleware>>();
        }

        [Test]
        public async Task ensureThatContinueWhenOptionsMethod()
        {
            int nextCount = 0;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                return Task.CompletedTask;
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("OPTIONS");
            await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object);
            Assert.AreEqual(1, nextCount);
        }

        [Test]
        public async Task ensureThatContinueWhenNotOptionsMethodAndNotThrows()
        {
            int nextCount = 0;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                return Task.CompletedTask;
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object);
            Assert.AreEqual(1, nextCount);
        }

        [Test]
        public void ensureThatThrowsWhenNotOptionsMethodAndGenericExceptionOccurs()
        {
            int nextCount = 0;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                throw new Exception("Some Error");
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpRequest.SetupGet(x => x.Path).Returns(new PathString("/teste/bpms"));
            _mockHttpRequest.SetupGet(x => x.PathBase).Returns(new PathString());
            _mockHttpRequest.SetupGet(x => x.Host).Returns(new HostString("myHost", 399));
            _mockHttpRequest.SetupGet(x => x.Scheme).Returns("http");
            _mockHttpRequest.SetupGet(x => x.QueryString).Returns(new QueryString());
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpContext.SetupGet(x => x.TraceIdentifier).Returns("80000132-0002-f600-b63f-84710c7967bb");
            _mockHttpResponse.Setup(x => x.StatusCode).Returns(400);

            Assert.ThrowsAsync(Is.TypeOf<Exception>().And.Message.EqualTo("Some Error"),
            async () => await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object));
            Assert.AreEqual(1, nextCount);
        }

        [Test]
        public async Task ensureThatThrowsWhenNotOptionsMethodAndArgumentHandleExceptionWithErrorListOccurs()
        {
            int nextCount = 0;
            string returnedData = String.Empty;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                throw new ArgumentHandleException("Some Errors", new List<Error>() { new Error("Error1"), new Error("Error2"), new Error("Error3") });
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpRequest.SetupGet(x => x.Path).Returns(new PathString("/teste/bpms"));
            _mockHttpRequest.SetupGet(x => x.PathBase).Returns(new PathString());
            _mockHttpRequest.SetupGet(x => x.Host).Returns(new HostString("myHost", 399));
            _mockHttpRequest.SetupGet(x => x.Scheme).Returns("http");
            _mockHttpRequest.SetupGet(x => x.QueryString).Returns(new QueryString());
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpContext.SetupGet(x => x.TraceIdentifier).Returns("80000132-0002-f600-b63f-84710c7967bb");
            _mockHttpResponse.Setup(x => x.StatusCode).Returns(400);            
            _mockHttpResponse.Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] data, int offset, int length, CancellationToken token) =>
                {
                    if (length > 0)
                        returnedData = Encoding.UTF8.GetString(data);
                })
                .Returns(Task.CompletedTask);

            await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object);
            Assert.AreEqual(1, nextCount);
            JObject jObject = JObject.Parse(returnedData);
            Assert.IsTrue(jObject.ContainsKey("Title"));
            Assert.IsTrue(jObject.ContainsKey("Status"));
            Assert.IsTrue(jObject.ContainsKey("Detail"));
            Assert.IsTrue(jObject.ContainsKey("Errors"));
            Assert.AreEqual("Some Errors", jObject["Title"].ToString());
            Assert.AreEqual(400, Convert.ToInt32(jObject["Status"]));
            Assert.AreEqual("Value does not fall within the expected range.", jObject["Detail"].ToString());
            Assert.AreEqual(3, jObject.GetValue("Errors").Count());
            Assert.AreEqual("Error1", jObject.GetValue("Errors")[0]["Message"].ToString());
            Assert.AreEqual("Error2", jObject.GetValue("Errors")[1]["Message"].ToString());
            Assert.AreEqual("Error3", jObject.GetValue("Errors")[2]["Message"].ToString());
        }

        [Test]
        public async Task ensureThatThrowsWhenNotOptionsMethodAndArgumentHandleWithoutErrorListExceptionOccurs()
        {
            int nextCount = 0;
            string returnedData = String.Empty;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                throw new ArgumentHandleException("Some Errors", new List<Error>());
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] data, int offset, int length, CancellationToken token) =>
                {
                    if (length > 0)
                        returnedData = Encoding.UTF8.GetString(data);
                })
                .Returns(Task.CompletedTask);

            await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object);
            Assert.AreEqual(1, nextCount);
            JObject jObject = JObject.Parse(returnedData);
            Assert.IsTrue(jObject.ContainsKey("Title"));
            Assert.IsTrue(jObject.ContainsKey("Status"));
            Assert.IsTrue(jObject.ContainsKey("Detail"));
            Assert.IsTrue(jObject.ContainsKey("Errors"));
            Assert.AreEqual("Some Errors", jObject["Title"].ToString());
            Assert.AreEqual(400, Convert.ToInt32(jObject["Status"]));
            Assert.AreEqual("Value does not fall within the expected range.", jObject["Detail"].ToString());
            Assert.AreEqual(0, jObject.GetValue("Errors").Count());
        }

        [Test]
        public async Task ensureThatThrowsWhenNotOptionsMethodAndArgumentNullHandleExceptionOccurs()
        {
            int nextCount = 0;
            string returnedData = String.Empty;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                throw new ArgumentNullHandleException("Other Errors");
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] data, int offset, int length, CancellationToken token) =>
                {
                    if (length > 0)
                        returnedData = Encoding.UTF8.GetString(data);
                })
                .Returns(Task.CompletedTask);

            await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object);
            Assert.AreEqual(1, nextCount);
            JObject jObject = JObject.Parse(returnedData);
            Assert.IsTrue(jObject.ContainsKey("Title"));
            Assert.IsTrue(jObject.ContainsKey("Status"));
            Assert.IsTrue(jObject.ContainsKey("Detail"));
            Assert.AreEqual("Other Errors", jObject["Title"].ToString());
            Assert.AreEqual(400, Convert.ToInt32(jObject["Status"]));
            Assert.AreEqual("Value cannot be null.", jObject["Detail"].ToString());
        }

        [Test]
        public async Task ensureThatThrowsWhenNotOptionsMethodAndBusinessExceptionOccurs()
        {
            int nextCount = 0;
            string returnedData = String.Empty;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                throw new BusinessException(new List<Error>() { new Error("More Errors") });
            };

            AuthenticationHandleExceptionMiddleware authenticationHandleExceptionMiddleware = new AuthenticationHandleExceptionMiddleware(_next, _mockLogger.Object);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] data, int offset, int length, CancellationToken token) =>
                {
                    if (length > 0)
                        returnedData = Encoding.UTF8.GetString(data);
                })
                .Returns(Task.CompletedTask);

            await authenticationHandleExceptionMiddleware.InvokeAsync(_mockHttpContext.Object);
            Assert.AreEqual(1, nextCount);
            JObject jObject = JObject.Parse(returnedData);
            Assert.IsTrue(jObject.ContainsKey("Title"));
            Assert.IsTrue(jObject.ContainsKey("Status"));
            Assert.IsTrue(jObject.ContainsKey("Detail"));
            Assert.AreEqual(1, jObject.GetValue("Errors").Count());
            Assert.AreEqual("More Errors", jObject.GetValue("Errors")[0]["Message"].ToString());
            Assert.AreEqual(400, Convert.ToInt32(jObject["Status"]));
            Assert.AreEqual("Exception of type 'SatelittiBpms.Models.Exceptions.BusinessException' was thrown.", jObject["Detail"].ToString());
        }
    }
}
