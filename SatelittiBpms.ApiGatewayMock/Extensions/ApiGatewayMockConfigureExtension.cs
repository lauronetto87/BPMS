using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.ApiGatewayMock.Interfaces;
using System;

namespace SatelittiBpms.ApiGatewayMock.Extensions
{
    public static class ApiGatewayMockConfigureExtension
    {
        public static void UseApiGatewayMock(this IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            app.UseWebSockets(webSocketOptions);

            var defaultWebSocketService = app.ApplicationServices.GetService<IDefaultWebSocketService>();

            app.Use(defaultWebSocketService.Connect);
        }
    }
}
