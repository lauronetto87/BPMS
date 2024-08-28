using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.ApiGatewayMock.Interfaces;
using SatelittiBpms.ApiGatewayMock.Services;

namespace SatelittiBpms.ApiGatewayMock.Extensions
{
    public static class ApiGatewayMockDependencyInjectionExtension
    {
        public static void AddApiGatewayMockDependencyInjection(
         this IServiceCollection services)
        {
            services.AddSingleton<IDefaultWebSocketService, DefaultWebSocketService>();
        }
    }
}
