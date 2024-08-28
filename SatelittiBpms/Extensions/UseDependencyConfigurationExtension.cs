using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SatelittiBpms.ApiGatewayMock.Extensions;
using SatelittiBpms.Authentication.Extensions;
using SatelittiBpms.VersionNormalization.Extensions;
using SatelittiBpms.Workflow.Extensions;
using System.Threading.Tasks;

namespace SatelittiBpms.Extensions
{
    public static class UseDependencyConfigurationExtension
    {
        public static void UseDependencyConfiguration(
            this IApplicationBuilder builder,
            IHostEnvironment currentEnvironment)
        {
            builder.UseAuthenticationDependencyConfiguration();
            Task.WaitAll(Task.Run(async () => await builder.ApplicationServices.UseVersionNormalization()));
            builder.ApplicationServices.UseWorkflowDependencyConfiguration();
            if (currentEnvironment.IsEnvironment("Local") || currentEnvironment.IsEnvironment("Test") || currentEnvironment.IsEnvironment("DockerLocal"))
                builder.UseApiGatewayMock();
        }
    }
}
