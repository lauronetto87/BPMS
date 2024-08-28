using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayMock.Interfaces
{
    public interface IDefaultWebSocketService
    {
        Task Connect(HttpContext context, Func<Task> next);
        Task SendMessage(string connectionId, string message);
    }
}
