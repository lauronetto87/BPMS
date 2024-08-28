using SatelittiBpms.Models.DTO;
using System.Threading.Tasks;

namespace SatelittiBpms.Workflow.Interfaces
{
    public interface IWorkflowHostService
    {
        void LoadWorkflowFromJson(string workflowJsonAsString);
        Task<string> StartFlow(int processId, int version, int requesterId, string connectionId);
        Task NextTask(NextStepDTO nextStepDTO);
        Task ExecuteTaskSignerIntegration(int taskId);
        void LoadPublishedWorkflows();
    }
}
