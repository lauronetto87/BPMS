using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Mail.Interfaces;
using SatelittiBpms.Mail.Mailer;
using SatelittiBpms.Mail.Services;

namespace SatelittiBpms.Mail.Extensions
{
    public static class MailDependencyInjectionExtension
    {
        public static void AddMailDependencyInjection(
         this IServiceCollection services)
        {
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IMailerService, AwsMailer>();
        }
    }
}
