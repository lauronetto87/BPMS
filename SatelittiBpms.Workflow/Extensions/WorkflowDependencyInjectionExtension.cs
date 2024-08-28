using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Workflow.ActivityTypes;
using SatelittiBpms.Workflow.Interfaces;
using SatelittiBpms.Workflow.Services;
using System;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Extensions
{
    public static class WorkflowDependencyInjectionExtension
    {
        public static void AddWorkflowDependencyInjection(
         this IServiceCollection services,
         IConfiguration configuration)
        {
            services.AddWorkflow(config =>
            {
                var connectionString = configuration.GetConnectionString(ProjectVariableConstants.WorkflowConnectionString);
                if (configuration["Provider"] == "SQLite")
                {
                    if (connectionString == "Filename=:memory:")
                    {
                        // Não aceita SQLite na memória tem de deixar o padrão que é salvo na memória
                    }
                    else
                    {
                        config.UseSqlite(connectionString, true);
                    }
                }
                else
                {
                    config.UseMySQL(connectionString, true, true);
                }
            });

            services.AddWorkflowDSL();
            services.AddTransient<StartEventActivity>();
            services.AddTransient<EndEventActivity>();
            services.AddTransient<SendTaskActivity>();
            services.AddTransient<UserTaskActivity>();
            services.AddTransient<ExclusiveGatewayActivity>();
            services.AddTransient<SignerIntegrationActivity>();
            services.AddTransient<IXmlDiagramParseService, XmlDiagramParseService>();
            services.AddTransient<IWorkflowHostService, WorkflowHostService>();
            services.AddTransient<IExclusiveGatewayService, ExclusiveGatewayParseService>();
        }

        public static void UseWorkflowDependencyConfiguration(this IServiceProvider applicationServices)
        {
            var workflowHost = applicationServices.GetService<IWorkflowHost>();
            workflowHost.Start();

            using (var serviceScope = applicationServices.CreateScope())
            {
                var myDependency = serviceScope.ServiceProvider.GetRequiredService<IWorkflowHostService>();
                myDependency.LoadPublishedWorkflows();

            }
        }
    }
}
