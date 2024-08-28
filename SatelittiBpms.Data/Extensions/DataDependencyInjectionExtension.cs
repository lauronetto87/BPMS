using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SatelittiBpms.Data.Extensions
{
    public static class DataDependencyInjectionExtension
    {
        public static void AddDataDependencyInjection(
         this IServiceCollection services)
        {
            services.AddScoped<DbContext, BpmsContext>();
        }
    }
}
