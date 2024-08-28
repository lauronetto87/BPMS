using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.VersionNormalization.Interfaces;
using System;
using System.Threading.Tasks;

namespace SatelittiBpms.VersionNormalization.Extensions
{
    public static class VersionNormalizationConfigureExtension
    {
        public static async Task UseVersionNormalization(this IServiceProvider applicationServices)
        {
            using (var serviceScope = applicationServices.CreateScope())
            {
                var executeNormalizations = serviceScope.ServiceProvider.GetRequiredService<IExecuteNormalizations>();
                await executeNormalizations.Execute();
            }
        }
    }
}
