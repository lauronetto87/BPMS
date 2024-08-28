using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Options;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Integration;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Utilities.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class TenantAuthServiceTest
    {
        Mock<IHttpClientCustom> _mockHttpClient;
        Mock<IOptions<SuiteOptions>> _mockSuiteOptions;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ILogger<SuiteUserService>> _mockLogger;
        Mock<ITenantAuthService> _tenantAuthService;

        [SetUp]
        public void init()
        {
            _mockHttpClient = new();
            _mockSuiteOptions = new();
            _mockContextDataService = new();
            _mockLogger = new();
            _tenantAuthService = new();

            _mockLogger.Setup(s => s.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
            _mockSuiteOptions.SetupGet(x => x.Value).Returns(new SuiteOptions()
            {
                UrlBase = "",
                UrlUserList = "",
                Authentication = new SuiteAuthenticationOptions()
            });
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeTenantAccessKeyIsNull()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            TenantAuthService tenantAuthService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object, _mockLogger.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("AccessKey cannot be null or empty"),
            async() => await tenantAuthService.GetTenantAuth( new TenantAuthFilter() { TenantAccessKey = null }));

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Never()); 
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeTenantAccessKeyIsEmpty()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            TenantAuthService tenantAuthService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object, _mockLogger.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("AccessKey cannot be null or empty"),           
            async () => await tenantAuthService.GetTenantAuth(new TenantAuthFilter() { TenantAccessKey = string.Empty }));

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Never());
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationSubDomainIsEmpty()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

            TenantAuthService tenantAuthService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object, _mockLogger.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("SubDomain cannot be null or empty"),
            async () => await tenantAuthService.GetTenantAuth(new TenantAuthFilter() { TenantAccessKey = "dfsdfuiasd-fdsafdsfasd-reuriweo1544", TenantSubDomain = string.Empty }));

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Never());
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeTokenAndBadRequest()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new StringContent("some error occurs")
            });

            TenantAuthService tenantAuthService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object, _mockLogger.Object);

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>().And.Message.EqualTo("some error occurs"),           
            async () => await tenantAuthService.GetTenantAuth(new TenantAuthFilter() { TenantAccessKey = "dfsdfuiasd-fdsafdsfasd-reuriweo1544", TenantSubDomain= "SubDomain" }));
            
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once());
        }

        [Test]
        public async Task ensureThatReturnsGetTenantAuthWhenAuthenticationAccessKeyIsEntered()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"id\": 1, \"subDomain\": \"subDomain\" , \"name\": \"Selbetti\", \"zone\": \"America/Sao_Paulo\",\"timezone\": -3, \"customizable\": false, \"systemName\": false, \"print_id\": false, \"accessKey\": false, \"authenticatedByAccessKey\": false  }")
            });

            TenantAuthService tenantAuthService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object, _mockLogger.Object);

            var result = await tenantAuthService.GetTenantAuth(new TenantAuthFilter() { TenantAccessKey = "dfsdfuiasd-fdsafdsfasd-reuriweo1544", TenantSubDomain = "SubDomain" });                        
            
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once());
        }

    }
}
