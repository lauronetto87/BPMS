using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SatelittiBpms.Storage.Interfaces;
using SatelittiBpms.Storage.Storage;

namespace SatelittiBpms.Storage.Extensions
{
    public static class StorageDependencyInjectionExtension
    {
        public static void AddStorageDependencyInjection(this IServiceCollection services, IHostEnvironment currentEnvironment)
        {
            if (currentEnvironment.IsEnvironment("Local") || currentEnvironment.IsEnvironment("Test") || currentEnvironment.IsEnvironment("DockerLocal"))
                services.AddScoped<IStorageService, LocalTempStorageService>();
            else
                services.AddScoped<IStorageService, AwsStorageService>();
        }
    }
}
