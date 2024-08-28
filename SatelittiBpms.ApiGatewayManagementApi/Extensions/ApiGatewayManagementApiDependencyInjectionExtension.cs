using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.ApiGatewayManagementApi.Services;

namespace SatelittiBpms.ApiGatewayManagementApi.Extensions
{
    public static class ApiGatewayManagementApiDependencyInjectionExtension
    {
        public static void ApiGatewayManagementApiDependencyInjection(
          this IServiceCollection services,
           IHostEnvironment currentEnvironment)
        {
            if (currentEnvironment.IsEnvironment("Local") || currentEnvironment.IsEnvironment("Test") || currentEnvironment.IsEnvironment("DockerLocal"))
                services.AddScoped<IFrontendNotifyService, LocalFrontendNotifyService>();
            else
                services.AddScoped<IFrontendNotifyService, CloudFrontendNotifyService>();
        }
    }
}
