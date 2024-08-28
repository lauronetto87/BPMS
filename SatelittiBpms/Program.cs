using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace SatelittiBpms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureLogging((hostingContext, logging) =>
             {
                 var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                 bool isDevelopment = currentEnvironment == "Local" || currentEnvironment == "DockerLocal" || currentEnvironment == "Test";

                 if (isDevelopment)
                     logging.AddNLog(hostingContext.Configuration.GetSection("Logging"));
                 else
                     logging.AddAWSProvider();

                 // When you need logging below set the minimum level. Otherwise the logging framework will default to Informational for external providers.
                 logging.SetMinimumLevel(LogLevel.Debug);
             })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel((context, options) =>
                {
                    const long max200Mb = 200 * 1024 * 1024;
                    options.Limits.MaxRequestBodySize = max200Mb;
                })
                .UseStartup<Startup>();
            });

    }
}
