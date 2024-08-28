using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.VersionNormalization.Interfaces;
using SatelittiBpms.VersionNormalization.Repository;
using SatelittiBpms.VersionNormalization.Services;

namespace SatelittiBpms.Services.Extensions
{
    public static class VersionNormalizationDependencyInjectionExtension
    {
        public static void AddVersionNormalizationDependencyInjection(
          this IServiceCollection services)
        {
            services.AddScoped<IExecuteNormalizations, ExecuteNormalizations>();
            services.AddScoped<IVersionNormalizationService, VersionNormalizationService>();
            services.AddScoped<IVersionNormalizationRepository, VersionNormalizationRepository>();
        }
    }
}
