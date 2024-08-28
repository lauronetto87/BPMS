using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Utilities.Http;

namespace SatelittiBpms.Utilities.Extensions
{
    public static class UtilitiesDependencyInjectionExtension
    {
        public static void AddUtilitiesDependencyInjection(
          this IServiceCollection services)
        {
            services.AddTransient<IHttpClientCustom, HttpClientCustom>();
        }
    }
}
