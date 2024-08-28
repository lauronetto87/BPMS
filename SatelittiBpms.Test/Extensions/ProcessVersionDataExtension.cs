using SatelittiBpms.FluentDataBuilder.FlowExecute.Builders;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;
using System.Linq;

namespace SatelittiBpms.Test.Extensions
{
    public static class ProcessVersionDataExtension
    {

        public static FlowBuilder NewFlow(this ProcessVersionData processVersionData)
        {
            return new FlowBuilder(processVersionData, null);
        }

        public static ProcessVersionDTO AsDto(this ProcessVersionData processVersionData)
        {
            return new ProcessVersionDTO
            {
                Activities = processVersionData.AllActivities.OfType<ActivityUserData>().Select(a => a.AsDto(processVersionData)).ToList(),
                Description = processVersionData.Description,
                DescriptionFlow = processVersionData.DescriptionFlow,
                DiagramContent = processVersionData.DiagramContent,
                FormContent = processVersionData.FormContent,
                Name = processVersionData.Name,
                NeedPublish = processVersionData.NeedPublish,
                ProcessId = 0,
                RolesIds = processVersionData.RolesIds,
                TaskSequance = processVersionData.TaskSequance,
                TenantId = processVersionData.TenantId,
                Version = processVersionData.Version,
                SignerTasks = processVersionData.AllActivities.OfType<ActivitySignerData>().Select(a => a.AsDto()).ToList(),
            };
        }
    }
}
