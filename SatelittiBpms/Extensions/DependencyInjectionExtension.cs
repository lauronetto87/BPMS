using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SatelittiBpms.ApiGatewayManagementApi.Extensions;
using SatelittiBpms.ApiGatewayMock.Extensions;
using SatelittiBpms.Attributes;
using SatelittiBpms.Authentication.Extensions;
using SatelittiBpms.Data.Extensions;
using SatelittiBpms.Mail.Extensions;
using SatelittiBpms.Models.Extensions;
using SatelittiBpms.Repository.Extensions;
using SatelittiBpms.Services.Extensions;
using SatelittiBpms.Storage.Extensions;
using SatelittiBpms.Translate.Extensions;
using SatelittiBpms.Utilities.Extensions;
using SatelittiBpms.Workflow.Extensions;

namespace SatelittiBpms.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(
          this IServiceCollection services,
          IHostEnvironment currentEnvironment,
          IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddAuthenticationDependencyInjection(currentEnvironment);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddUtilitiesDependencyInjection();
            services.AddModelsDependencyInjection();
            services.AddDataDependencyInjection();
            services.AddTranslateDependencyInjection();
            services.AddRepositoryDependencyInjection();
            services.AddServicesDependencyInjection(currentEnvironment);
            services.AddVersionNormalizationDependencyInjection();
            services.AddScoped<ValidationFilterAttribute>();
            services.AddMailDependencyInjection();
            services.AddStorageDependencyInjection(currentEnvironment);
            services.AddWorkflowDependencyInjection(configuration);
            if (currentEnvironment.IsEnvironment("Local") || currentEnvironment.IsEnvironment("Test") || currentEnvironment.IsEnvironment("DockerLocal"))
                services.AddApiGatewayMockDependencyInjection();
            services.ApiGatewayManagementApiDependencyInjection(currentEnvironment);
        }
    }
}