using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SatelittiBpms.Services.Integration;
using SatelittiBpms.Services.Integration.Mock;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;

namespace SatelittiBpms.Services.Extensions
{
    public static class ServicesDependencyInjectionExtension
    {
        public static void AddServicesDependencyInjection(
          this IServiceCollection services,
           IHostEnvironment currentEnvironment)
        {
            services.AddScoped<IUserService, UserService>();
            if (currentEnvironment.IsEnvironment("Local") || currentEnvironment.IsEnvironment("Test") || currentEnvironment.IsEnvironment("DockerLocal"))
            {
                services.AddScoped<ISuiteUserService, MockSuiteUserService>();
                services.AddScoped<ITenantAuthService, MockTenantAuthService>();
                services.AddScoped<ISignerSegmentService, MockSignerSegmentService>();
                services.AddScoped<ISignerReminderService, MockSignerReminderService>();
                services.AddScoped<ISignerSignatureTypeService, MockSignerSignatureTypeService>();
                services.AddScoped<ISignerSubscriberTypeService, MockSignerSubscriberTypeService>();
                services.AddScoped<ISignerIntegrationRestService, MockSignerIntegrationRestService>();
            }
            else
            {
                services.AddScoped<ISuiteUserService, SuiteUserService>();
                services.AddScoped<ITenantAuthService, TenantAuthService>();
                services.AddScoped<ISignerSegmentService, SignerSegmentService>();
                services.AddScoped<ISignerReminderService, SignerReminderService>();
                services.AddScoped<ISignerSignatureTypeService, SignerSignatureTypeService>();
                services.AddScoped<ISignerSubscriberTypeService, SignerSubscriberTypeService>();
                services.AddScoped<ISignerIntegrationRestService, SignerIntegrationRestService>();
            }

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleUserService, RoleUserService>();
            services.AddScoped<IProcessRoleService, ProcessRoleService>();
            services.AddScoped<IFieldValueService, FieldValueService>();
            services.AddScoped<IProcessVersionService, ProcessVersionService>();
            services.AddScoped<IProcessService, ProcessService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IActivityFieldService, ActivityFieldService>();
            services.AddScoped<IWorkflowValidationService, WorkflowValidationService>();
            services.AddScoped<IFieldService, FieldService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskHistoryService, TaskHistoryService>();
            services.AddScoped<IFlowService, FlowService>();
            services.AddScoped<IActivityNotificationService, ActivityNotificationService>();
            services.AddScoped<IActivityUserService, ActivityUserService>();
            services.AddScoped<IActivityUserOptionService, ActivityUserOptionService>();
            services.AddScoped<IXmlDiagramService, XmlDiagramService>();
            services.AddScoped<IFlowPathService, FlowPathService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ITenantActivateService, TenantActivateService>();
            services.AddScoped<IWildcardService, WildcardService>();
            services.AddScoped<IFieldValueFileService, FieldValueFileService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IFlowHistoryService, FlowHistoryService>();
            services.AddScoped<ISignerIntegrationService, SignerIntegrationService>();
            services.AddScoped<ISignerIntegrationActivityService, SignerIntegrationActivityService>();
            services.AddScoped<ITaskSignerService, TaskSignerService>();
            services.AddScoped<INotificationService, NotificationService>();
        }
    }
}
