using Newtonsoft.Json;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.ApiGatewayMock.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayManagementApi.Services
{
    public class LocalFrontendNotifyService : IFrontendNotifyService
    {
        private readonly IDefaultWebSocketService _defaultWebSocketService;

        public LocalFrontendNotifyService(
            IDefaultWebSocketService defaultWebSocketService)
        {
            _defaultWebSocketService = defaultWebSocketService;
        }

        public async Task Notify(string connectionId, object message)
        {
            var notifyMessage = JsonConvert.SerializeObject(message, Formatting.None);
            await _defaultWebSocketService.SendMessage(connectionId, notifyMessage);
        }
    }
}
