using I18Next.Net.AspNetCore;
using I18Next.Net.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Translate.Integrantions;
using SatelittiBpms.Translate.Interfaces;
using SatelittiBpms.Translate.Services;

namespace SatelittiBpms.Translate.Extensions
{
    public static class TranslateDependencyInjectionExtension
    {
        public static void AddTranslateDependencyInjection(
          this IServiceCollection services)
        {
            services.AddI18NextLocalization(i18n =>
            {
                i18n
                .IntegrateToAspNetCore()
                .AddBackend(new CustomJsonFileBackend())
                .UseDefaultLanguage("pt-BR")
                .UseDefaultNamespace("translation");
            });

            services.AddScoped<ITranslateService, TranslateService>();
        }
    }
}
