using Moq;
using NUnit.Framework;
using Satelitti.Options;
using SatelittiBpms.Services.Integration;
using SatelittiBpms.Services.Tests.Infos;
using SatelittiBpms.Utilities.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests.Integrations
{
    public class SignerServiceBaseTest
    {
        Mock<IHttpClientCustom> _mockHttpClient;
        SuiteOptions _suiteOptions;

        [SetUp]
        public void Setup()
        {
            _mockHttpClient = new Mock<IHttpClientCustom>();
            _suiteOptions = new SuiteOptions() { UrlBase = "http://{0}.dev.satelitti.com.br/rest" };
        }

        [Test]
        public async Task EnsureThatReturnsTypedObjectWhenSuccess()
        {
            string subdomain = "someSubdomain", signerToken = "someToken";

            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("[ { \"id\": 1, \"description\": \"Teste1\" }, { \"id\": 2, \"description\": \"Teste2\" }, { \"id\": 3, \"description\": \"Teste3\" } ]")
            });

            SignerServiceBase signerServiceBase = new(_mockHttpClient.Object, _suiteOptions, "/some/url/path");

            var result = await signerServiceBase.List<List<SignerServiceBaseTestResultObjectHelper>>(subdomain, signerToken);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Teste1", result[0].Description);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Teste2", result[1].Description);
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual("Teste3", result[2].Description);

            _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(x => x.Headers.Contains("Authorization") &&
                                                                                   x.Headers.Authorization.Scheme == "Custom" &&
                                                                                   x.Headers.Authorization.Parameter == signerToken &&
                                                                                   x.Method == HttpMethod.Get &&
                                                                                   x.RequestUri.Host == "someSubdomain.dev.satelitti.com.br".ToLower() &&
                                                                                   x.RequestUri.PathAndQuery == "/rest/some/url/path")), Times.Once());
        }

        [Test]
        public void EnsureThatThrowsWhenErrorOccursAtRequest()
        {
            string subdomain = "someSubdomain", signerToken = "someToken";
            _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new StringContent("some error occurs")
            });

            SignerServiceBase signerServiceBase = new(_mockHttpClient.Object, _suiteOptions, "/some/url/path");

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>().And.Message.EqualTo("some error occurs"),
           async () => await signerServiceBase.List<List<SignerServiceBaseTestResultObjectHelper>>(subdomain, signerToken));

            _mockHttpClient.Verify(x => x.SendAsync(It.Is<HttpRequestMessage>(x => x.Headers.Contains("Authorization") &&
                                                                                   x.Headers.Authorization.Scheme == "Custom" &&
                                                                                   x.Headers.Authorization.Parameter == signerToken &&
                                                                                   x.Method == HttpMethod.Get &&
                                                                                   x.RequestUri.Host == "someSubdomain.dev.satelitti.com.br".ToLower() &&
                                                                                   x.RequestUri.PathAndQuery == "/rest/some/url/path")), Times.Once());
        }
    }
}
