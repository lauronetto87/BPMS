using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SatelittiBpms.ApiGatewayManagementApi.Services;
using SatelittiBpms.ApiGatewayManagementApi.Tests.ServicesHelper;
using SatelittiBpms.Options.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayManagementApi.Tests
{
    public class CloudFrontendNotifyServiceTest
    {
        Mock<IOptions<AwsOptions>> _mockAwsOptions;

        AwsOptions awsOptions = new AwsOptions
        {
            ApiGateway = new ApiGatewayOptions()
            {
                WebSocketPostToConnectionUrl = "SomeUrlToPost"
            }
        };

        [SetUp]
        public void Setup()
        {
            _mockAwsOptions = new Mock<IOptions<AwsOptions>>();

            _mockAwsOptions.SetupGet(x => x.Value).Returns(awsOptions);
        }

        [Test]
        public async Task ensureNotify()
        {
            string connectionId = "someConnectionId",
                message = "mensagem aleatoria para ser enviada";
            Mock<IAmazonApiGatewayManagementApi> mockAmazonApiClient = new Mock<IAmazonApiGatewayManagementApi>();
            mockAmazonApiClient.Setup(x => x.PostToConnectionAsync(It.IsAny<PostToConnectionRequest>(), It.IsAny<CancellationToken>()));

            Mock<CloudFrontendNotifyService> cloudFrontendNotifyService = new Mock<CloudFrontendNotifyService>(_mockAwsOptions.Object) { CallBase = true };
            cloudFrontendNotifyService.Protected().Setup<IAmazonApiGatewayManagementApi>("CreateApiClient").Returns(mockAmazonApiClient.Object);
            await cloudFrontendNotifyService.Object.Notify(connectionId, message);

            mockAmazonApiClient.Verify(x => x.PostToConnectionAsync(It.Is<PostToConnectionRequest>(y => y.ConnectionId == connectionId && CompareStream(y.Data)), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void ensureCreateApiClient()
        {
            CloudFrontendNotifyServiceHelper cloudFrontendNotifyService = new CloudFrontendNotifyServiceHelper(_mockAwsOptions.Object);
            var result = cloudFrontendNotifyService.CreateApiClient();

            Assert.AreEqual(awsOptions.ApiGateway.WebSocketPostToConnectionUrl, result.Config.ServiceURL);
        }

        private bool CompareStream(MemoryStream resultMs)
        {
            using (MemoryStream expectedMs = new MemoryStream(Encoding.UTF8.GetBytes("\"mensagem aleatoria para ser enviada\"")))
            {
                return expectedMs.ToArray().SequenceEqual(resultMs.ToArray());
            }
        }
    }
}
