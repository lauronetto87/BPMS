using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Satelitti.Options;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Services.Integration;
using SatelittiBpms.Utilities.Http;

namespace SatelittiBpms.Services.Tests.Integrations
{
    public class SignerSubscriberTypeServiceTest
    {
        Mock<IHttpClientCustom> _mockHttpClient;
        Mock<IOptions<SuiteOptions>> _mockSuiteOptions;
        Mock<IOptions<SignerOptions>> _mockSigerOptions;

        [SetUp]
        public void Setup()
        {
            _mockHttpClient = new Mock<IHttpClientCustom>();
            _mockSuiteOptions = new Mock<IOptions<SuiteOptions>>();
            _mockSigerOptions = new Mock<IOptions<SignerOptions>>();

            _mockSuiteOptions.SetupGet(x => x.Value).Returns(new SuiteOptions() { UrlBase = "http://{0}.dev.satelitti.com.br/rest" });
            _mockSigerOptions.SetupGet(x => x.Value).Returns(new SignerOptions() { BasePath = "/signer/", ReminderIntegrationPath = "SubscriberTypeIntegration" });
        }

        [Test]
        public void Ensure()
        {
            SignerSubscriberTypeService signerSubscriberTypeService = new(_mockHttpClient.Object, _mockSuiteOptions.Object, _mockSigerOptions.Object);

            Assert.IsNotNull(signerSubscriberTypeService);
            Assert.IsInstanceOf(typeof(SignerServiceBase), signerSubscriberTypeService);
        }
    }
}
