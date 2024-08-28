using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Satelitti.Options;
using SatelittiBpms.Options.Models;

namespace SatelittiBpms.Options.Extensions
{
    public static class ConfigureOptionsExtension
    {
        public static void ConfigureBpmsOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SuiteOptions>(configuration.GetSection(SuiteOptions.SECTION_NAME));
            services.Configure<SuiteAuthenticationOptions>(configuration.GetSection($"{SuiteOptions.SECTION_NAME}:{SuiteAuthenticationOptions.SECTION_NAME}"));
            services.Configure<AwsOptions>(configuration.GetSection(AwsOptions.SECTION_NAME));
            services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.SECTION_NAME));
            services.Configure<SignerOptions>(configuration.GetSection(SignerOptions.SECTION_NAME));
        }
    }
}
