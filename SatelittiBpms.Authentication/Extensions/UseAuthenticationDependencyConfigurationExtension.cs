using Microsoft.AspNetCore.Builder;
using SatelittiBpms.Authentication.Middleware;

namespace SatelittiBpms.Authentication.Extensions
{
    public static class UseAuthenticationDependencyConfigurationExtension
    {
        public static void UseAuthenticationDependencyConfiguration(
           this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AuthenticationHandleExceptionMiddleware>();
            builder.UseMiddleware<AuthenticationMiddleware>();            
        }
    }
}
