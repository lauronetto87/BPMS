using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Service;
using Satelitti.Authentication.Service.Interface;
using Satelitti.Authentication.Service.Mock;
using SatelittiBpms.Authentication.Context;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Authentication.Services;
using SatelittiBpms.Authentication.Services.Interfaces;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;

namespace SatelittiBpms.Authentication.Extensions
{
    public static class AuthenticationDependencyInjectionExtension
    {
        public static void AddAuthenticationDependencyInjection(
          this IServiceCollection services,
           IHostEnvironment currentEnvironment)
        {
            if (currentEnvironment.IsEnvironment("Local") || currentEnvironment.IsEnvironment("Test") || currentEnvironment.IsEnvironment("DockerLocal"))
            {
                services.AddScoped<IContextDataService<UserInfo>, MockBpmsContextDataService>();
                services.AddScoped<ISuiteService, MockSuiteService>();
            }
            else
            {
                services.AddScoped<IContextDataService<UserInfo>, BpmsContextDataService>();
                services.AddScoped<ISuiteService, SuiteService>();
            }

            services.AddScoped<ITokenService<GenerateTokenParameters>, JwtTokenService>();
            services.AddScoped<IAuthUserSuiteTokenService, AuthUserSuiteTokenService<UserInfo>>();
            services.AddScoped<ISuiteTenantAuthService, SuiteTenantAuthService>();
            services.AddScoped<ISuiteTokenValidationService, SuiteTokenValidationService>();
            services.AddScoped<IAuthUserTokenService<UserViewModel>, AuthUserTokenService>();
            services.AddScoped<IResolverUser<UserViewModel>, AuthUserService>();
            services.AddScoped<IAuthUserService, AuthUserService>();
        }
    }
}
