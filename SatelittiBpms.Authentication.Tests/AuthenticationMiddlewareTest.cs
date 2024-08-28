using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Satelitti.Authentication.Authorization;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using Satelitti.Authentication.Result;
using Satelitti.Authentication.Service.Interface;
using SatelittiBpms.Authentication.Middleware;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.Authentication.Tests
{
    public class AuthenticationMiddlewareTest
    {
        Mock<HttpContext> _mockHttpContext;
        Mock<ISuiteService> _mockSuiteService;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<IAuthUserTokenService<UserViewModel>> _mockAuthUserTokenService;
        Mock<HttpRequest> _mockHttpRequest;
        Mock<HttpResponse> _mockHttpResponse;

        [SetUp]
        public void init()
        {
            _mockHttpContext = new Mock<HttpContext>();
            _mockSuiteService = new Mock<ISuiteService>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockAuthUserTokenService = new Mock<IAuthUserTokenService<UserViewModel>>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();
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

            AuthenticationMiddleware authenticationMiddleware = new AuthenticationMiddleware(_next);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("OPTIONS");
            await authenticationMiddleware.Invoke(_mockHttpContext.Object, _mockSuiteService.Object, _mockContextDataService.Object, _mockAuthUserTokenService.Object);
            Assert.AreEqual(1, nextCount);
        }

        [Test]
        public async Task ensureThatContinueWhenHealthyCheckPath()
        {
            int nextCount = 0;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                return Task.CompletedTask;
            };

            AuthenticationMiddleware authenticationMiddleware = new AuthenticationMiddleware(_next);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpRequest.SetupGet(x => x.Path).Returns($"{ProjectVariableConstants.BpmsUrlPath}/check");
            await authenticationMiddleware.Invoke(_mockHttpContext.Object, _mockSuiteService.Object, _mockContextDataService.Object, _mockAuthUserTokenService.Object);
            Assert.AreEqual(1, nextCount);
        }

        [Test]
        public async Task ensureThatContinueWhenTokenIsValid()
        {
            int nextCount = 0;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                return Task.CompletedTask;
            };
            ContextData<UserInfo> contextData = new ContextData<UserInfo>()
            {
                SubDomain = "subDomainTest",
                SuiteToken = "tokenSuite"
            };
            AuthTokenWithUser<UserViewModel> authTokenWithUser = new AuthTokenWithUser<UserViewModel>()
            {
                Token = "someTokem",
                User = new UserViewModel() { Timezone = -4 }
            };

            Mock<IHeaderDictionary> mockHeaders = new Mock<IHeaderDictionary>();
            Dictionary<string, StringValues> dicHeader = new Dictionary<string, StringValues>();

            AuthenticationMiddleware authenticationMiddleware = new AuthenticationMiddleware(_next);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpRequest.SetupGet(x => x.Path).Returns(ProjectVariableConstants.BpmsUrlPath);
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(mockHeaders.Object);
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(contextData);
            _mockContextDataService.Setup(x => x.SetSuiteTenant(It.IsAny<SuiteTenantAuth>())).Returns(contextData);
            _mockSuiteService.Setup(x => x.GetTenantBySubDomain(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SuiteTenantAuth()
            {
            });
            mockHeaders.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<StringValues>()))
                .Callback<string, StringValues>((x, y) =>
                {
                    dicHeader.Add(x, y);
                });
            _mockAuthUserTokenService.Setup(x => x.Validate(It.IsAny<string>())).ReturnsAsync(Result.Success(authTokenWithUser));

            await authenticationMiddleware.Invoke(_mockHttpContext.Object, _mockSuiteService.Object, _mockContextDataService.Object, _mockAuthUserTokenService.Object);
            Assert.AreEqual(1, nextCount);

            Assert.IsTrue(dicHeader.ContainsKey(Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS));
            Assert.IsTrue(dicHeader.ContainsKey(Constants.Constants.RESPONSE_SET_AUTHORIZATION));
            Assert.IsTrue(dicHeader.ContainsKey(Constants.Constants.RESPONSE_TIMEZONE));
            Assert.AreEqual(Constants.Constants.RESPONSE_ACCESS_CONTROL_ALL, dicHeader[Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS]);
            Assert.AreEqual(authTokenWithUser.Token, dicHeader[Constants.Constants.RESPONSE_SET_AUTHORIZATION]);
            Assert.AreEqual(authTokenWithUser.User.Timezone.ToString(), dicHeader[Constants.Constants.RESPONSE_TIMEZONE]);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockContextDataService.Verify(x => x.SetSuiteTenant(It.IsAny<SuiteTenantAuth>()), Times.Once());
            _mockSuiteService.Verify(x => x.GetTenantBySubDomain(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockAuthUserTokenService.Verify(x => x.Validate(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ensureThatContinueWhenSuiteTokenIsEmpty()
        {
            int nextCount = 0;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                return Task.CompletedTask;
            };
            ContextData<UserInfo> contextData = new ContextData<UserInfo>()
            {
                SubDomain = "subDomainTest",
                SuiteToken = null
            };
            AuthTokenWithUser<UserViewModel> authTokenWithUser = new AuthTokenWithUser<UserViewModel>()
            {
                Token = "someTokem",
                User = new UserViewModel() { Timezone = -4 }
            };

            Mock<IHeaderDictionary> mockHeaders = new Mock<IHeaderDictionary>();
            Dictionary<string, StringValues> dicHeader = new Dictionary<string, StringValues>();

            AuthenticationMiddleware authenticationMiddleware = new AuthenticationMiddleware(_next);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpRequest.SetupGet(x => x.Path).Returns(ProjectVariableConstants.BpmsUrlPath);
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(mockHeaders.Object);
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(contextData);
            _mockContextDataService.Setup(x => x.SetSuiteTenant(It.IsAny<SuiteTenantAuth>())).Returns(contextData);
            _mockSuiteService.Setup(x => x.GetTenantBySubDomain(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SuiteTenantAuth()
            {
            });
            mockHeaders.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<StringValues>()))
                .Callback<string, StringValues>((x, y) =>
                {
                    dicHeader.Add(x, y);
                });
            _mockAuthUserTokenService.Setup(x => x.Validate(It.IsAny<string>())).ReturnsAsync(Result.Success(authTokenWithUser));

            await authenticationMiddleware.Invoke(_mockHttpContext.Object, _mockSuiteService.Object, _mockContextDataService.Object, _mockAuthUserTokenService.Object);
            Assert.AreEqual(1, nextCount);

            Assert.IsTrue(dicHeader.ContainsKey(Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS));
            Assert.IsFalse(dicHeader.ContainsKey(Constants.Constants.RESPONSE_SET_AUTHORIZATION));
            Assert.IsFalse(dicHeader.ContainsKey(Constants.Constants.RESPONSE_TIMEZONE));
            Assert.AreEqual(Constants.Constants.RESPONSE_ACCESS_CONTROL_ALL, dicHeader[Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS]);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockContextDataService.Verify(x => x.SetSuiteTenant(It.IsAny<SuiteTenantAuth>()), Times.Once());
            _mockSuiteService.Verify(x => x.GetTenantBySubDomain(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockAuthUserTokenService.Verify(x => x.Validate(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ensureThatReturnsErrorWhenTokenIsNotValid()
        {
            int nextCount = 0;
            string returnedData = string.Empty;
            RequestDelegate _next = (HttpContext context) =>
            {
                nextCount++;
                return Task.CompletedTask;
            };
            ContextData<UserInfo> contextData = new ContextData<UserInfo>()
            {
                SubDomain = "subDomainTest",
                SuiteToken = "tokenSuite"
            };
            AuthTokenWithUser<UserViewModel> authTokenWithUser = new AuthTokenWithUser<UserViewModel>()
            {
                Token = "someTokem",
                User = new UserViewModel() { Timezone = -4 }
            };

            Mock<IHeaderDictionary> mockHeaders = new Mock<IHeaderDictionary>();
            Dictionary<string, StringValues> dicHeader = new Dictionary<string, StringValues>();

            AuthenticationMiddleware authenticationMiddleware = new AuthenticationMiddleware(_next);
            _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            _mockHttpRequest.SetupGet(x => x.Method).Returns("GET");
            _mockHttpRequest.SetupGet(x => x.Path).Returns(ProjectVariableConstants.BpmsUrlPath);
            _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);
            _mockHttpResponse.SetupGet(x => x.Headers).Returns(mockHeaders.Object);
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(contextData);
            _mockContextDataService.Setup(x => x.SetSuiteTenant(It.IsAny<SuiteTenantAuth>())).Returns(contextData);
            _mockSuiteService.Setup(x => x.GetTenantBySubDomain(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new SuiteTenantAuth()
            {
            });
            mockHeaders.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<StringValues>()))
                .Callback<string, StringValues>((x, y) =>
                {
                    dicHeader.Add(x, y);
                });
            _mockAuthUserTokenService.Setup(x => x.Validate(It.IsAny<string>())).ReturnsAsync(Result.Error<AuthTokenWithUser<UserViewModel>>("Some error occurs - VALIDATE"));
            _mockHttpResponse.Setup(x => x.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] data, int offset, int length, CancellationToken token) =>
                {
                    if (length > 0)
                        returnedData = Encoding.UTF8.GetString(data);
                })
                .Returns(Task.CompletedTask);

            await authenticationMiddleware.Invoke(_mockHttpContext.Object, _mockSuiteService.Object, _mockContextDataService.Object, _mockAuthUserTokenService.Object);
            Assert.AreEqual(0, nextCount);

            Assert.IsTrue(dicHeader.ContainsKey(Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS));
            Assert.IsFalse(dicHeader.ContainsKey(Constants.Constants.RESPONSE_SET_AUTHORIZATION));
            Assert.IsFalse(dicHeader.ContainsKey(Constants.Constants.RESPONSE_TIMEZONE));
            Assert.AreEqual(Constants.Constants.RESPONSE_ACCESS_CONTROL_ALL, dicHeader[Constants.Constants.RESPONSE_ACCESS_CONTROL_EXPOSE_HEADERS]);
            JObject jObject = JObject.Parse(returnedData);
            Assert.IsTrue(jObject.ContainsKey("errorId"));
            Assert.AreEqual("Some error occurs - VALIDATE", jObject["errorId"].ToString());

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockContextDataService.Verify(x => x.SetSuiteTenant(It.IsAny<SuiteTenantAuth>()), Times.Once());
            _mockSuiteService.Verify(x => x.GetTenantBySubDomain(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockAuthUserTokenService.Verify(x => x.Validate(It.IsAny<string>()), Times.Once());
        }
    }
}
