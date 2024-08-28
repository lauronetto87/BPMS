using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository.Extensions
{
    public static class RepositoryDependencyInjectionExtension
    {
        public static void AddRepositoryDependencyInjection(
         this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleUserRepository, RoleUserRepository>();
            services.AddScoped<IProcessRoleRepository, ProcessRoleRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<IProcessVersionRepository, ProcessVersionRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IActivityFieldRepository, ActivityFieldRepository>();
            services.AddScoped<IFieldRepository, FieldRepository>();
            services.AddScoped<IFieldValueRepository, FieldValueRepository>();
            services.AddScoped<IFlowRepository, FlowRepository>();
            services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IActivityNotificationRepository, ActivityNotificationRepository>();
            services.AddScoped<IActivityUserRepository, ActivityUserRepository>();
            services.AddScoped<IActivityUserOptionRepository, ActivityUserOptionRepository>();
            services.AddScoped<IFlowPathRepository, FlowPathRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IFieldValueFileRepository, FieldValueFileRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ISignerIntegrationActivityRepository, SignerIntegrationActivityRepository>();
            services.AddScoped<ITaskSignerRepository, TaskSignerRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
        }
    }
}
