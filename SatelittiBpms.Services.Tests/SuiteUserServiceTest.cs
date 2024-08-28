using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Options;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Integration;
using SatelittiBpms.Utilities.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class SuiteUserServiceTest
    {
        Mock<IHttpClientCustom> _mockHttpClient;
        Mock<IOptions<SuiteOptions>> _mockSuiteOptions;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;

        [SetUp]
        public void init()
        {
            _mockHttpClient = new();
            _mockSuiteOptions = new();
            _mockContextDataService = new();

            _mockSuiteOptions.SetupGet(x => x.Value).Returns(new SuiteOptions()
            {
                UrlBase = "",
                UrlUserList = ""
            });
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeTokenAndSuiteTokenIsNull()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });

            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Value cannot be null or empty"),
           async () => await suiteUserService.ListWithContext(new SuiteUserListFilter() { SuiteToken = null }));

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Never());
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeTokenAndSuiteTokenIsEmpty()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            });

            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Value cannot be null or empty"),
           async () => await suiteUserService.ListWithContext(new SuiteUserListFilter() { SuiteToken = string.Empty }));

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

            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>().And.Message.EqualTo("some error occurs"),
           async () => await suiteUserService.ListWithContext(new SuiteUserListFilter() { SuiteToken = "someTokem" }));

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once());
        }

        [Test]
        public async Task ensureThatReturnsUserListWhenAuthenticationTypeTokenAndHasToken()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("[ { \"id\": 1, \"name\": \"Teste1\" }, { \"id\": 2, \"name\": \"Teste2\" }, { \"id\": 3, \"name\": \"Teste3\" } ]")
            });

            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            var result = await suiteUserService.ListWithContext(new SuiteUserListFilter() { SuiteToken = "someTokem" });

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual("Teste1", result[0].Name);
            Assert.AreEqual("Teste2", result[1].Name);
            Assert.AreEqual("Teste3", result[2].Name);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once());
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeServiceAndTenantAccessKeyIsNull()
        {
            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Value cannot be null or empty"),
          async () => await suiteUserService.ListWithoutContext(new SuiteUserListFilter() { TenantAccessKey = null }));
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeServiceAndTenantAccessKeyIsEmpty()
        {
            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Value cannot be null or empty"),
          async () => await suiteUserService.ListWithoutContext(new SuiteUserListFilter() { TenantAccessKey = string.Empty }));
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeServiceAndTenantSubDomainIsNull()
        {
            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Value cannot be null or empty"),
          async () => await suiteUserService.ListWithoutContext(new SuiteUserListFilter() { TenantSubDomain = null }));
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeServiceAndTenantSubDomainIsEmpty()
        {
            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Value cannot be null or empty"),
          async () => await suiteUserService.ListWithoutContext(new SuiteUserListFilter() { TenantSubDomain = string.Empty }));
        }

        [Test]
        public void ensureThatThrowsWhenAuthenticationTypeServiceAndBadRequest()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { SubDomain = "SubDomain" });
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new StringContent("some error occurs2")
            });

            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>().And.Message.EqualTo("some error occurs2"),
           async () => await suiteUserService.ListWithoutContext(new SuiteUserListFilter() { TenantAccessKey = "someAccessKey", TenantSubDomain = "someSubdomain" }));
        }

        [Test]
        public async Task ensureThatReturnsUserListWhenAuthenticationTypeServiceAndHasTenantAccessKeyAndTenantSubDomain()
        {
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("[ { \"id\": 1, \"name\": \"Teste1\" }, { \"id\": 2, \"name\": \"Teste2\" }, { \"id\": 3, \"name\": \"Teste3\" } ]")
            });

            SuiteUserService suiteUserService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockContextDataService.Object);

            var result = await suiteUserService.ListWithoutContext(new SuiteUserListFilter() { TenantAccessKey = "someAccessKey", TenantSubDomain = "someSubdomain" });

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual("Teste1", result[0].Name);
            Assert.AreEqual("Teste2", result[1].Name);
            Assert.AreEqual("Teste3", result[2].Name);

            _mockHttpClient.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Once());
        }
    }
}
