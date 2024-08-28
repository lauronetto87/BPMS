using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ITaskService : IServiceBase<TaskDTO, TaskInfo>
    {
        Task<ResultContent> Assign(int taskId);
        Task<ResultContent> Unassign(int taskId);
        Task<ResultContent<int>> Insert(TaskInfo info);
        Task<ResultContent> GetTaskToExecute(int taskId);
        Task<ResultContent> NextTask(NextStepDTO nextStepDTO);
        Task<ResultContent> ListTasks(TaskFilterDTO filters);
        Task<ResultContent> ListTasksGroup(TaskFilterDTO filters);
        Task<ResultContent> GetDetailsById(int taskId);
        Task<ResultContent<TaskCounterViewModel>> GetCounterTask(TaskFilterDTO filters);
        Task<ResultContent> ExecuteTaskSignerIntegration(int taskId);        
    }
}
