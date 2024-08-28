using Moq;
using NUnit.Framework;
using SatelittiBpms.ApiGatewayManagementApi.Services;
using SatelittiBpms.ApiGatewayMock.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayManagementApi.Tests
{
    public class LocalFrontendNotifyServiceTest
    {
        Mock<IDefaultWebSocketService> _mockDefaultWebSocketService;

        [SetUp]
        public void Setup()
        {
            _mockDefaultWebSocketService = new Mock<IDefaultWebSocketService>();
        }

        [Test]
        public async Task ensureNotify()
        {
            string connectionId = "someConnectionId",
                message = "mensagem aleatoria para ser enviada";

            _mockDefaultWebSocketService.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>()));

            LocalFrontendNotifyService localNotifyService = new LocalFrontendNotifyService(_mockDefaultWebSocketService.Object);
            await localNotifyService.Notify(connectionId, message);

            _mockDefaultWebSocketService.Verify(x => x.SendMessage(It.Is<string>(y => y == connectionId), It.Is<string>(y => y.Equals("\"mensagem aleatoria para ser enviada\""))), Times.Once());

        }
    }
}
