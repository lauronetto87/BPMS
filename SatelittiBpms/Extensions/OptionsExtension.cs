using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Options.Extensions;

namespace SatelittiBpms.Extensions
{
    public static class OptionsExtension
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureBpmsOptions(configuration);
        }
    }
}
