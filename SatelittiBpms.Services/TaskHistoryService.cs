using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class TaskHistoryService : AbstractServiceBase<TaskHistoryDTO, TaskHistoryInfo, ITaskHistoryRepository>, ITaskHistoryService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;

        public TaskHistoryService(
            ITaskHistoryRepository repository,
            IMapper mapper,
            IContextDataService<UserInfo> contextDataService) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
        }

        public async Task<int> Insert(int taskId, int executorId)
        {
            var contextData = _contextDataService.GetContextData();

            TaskHistoryInfo info = new TaskHistoryInfo();
            info.TenantId = contextData.Tenant.Id;
            info.ExecutorId = executorId;
            info.TaskId = taskId;
            info.StartDate = DateTime.UtcNow;
            info.EndDate = null;

            return await _repository.Insert(info);
        }
        public async Task UnassignHistory(int taskId, int executorId)
        {
            var contextData = _contextDataService.GetContextData();

            TaskHistoryInfo info = await _repository.GetLastByTask(contextData.Tenant.Id, taskId, executorId);

            info.EndDate = DateTime.UtcNow;

            await _repository.Update(info);
        }
        public async Task UpdateEndDateFromTaskHistories(int taskId, int tenantId)
        {
            TaskHistoryInfo taskHistoryInfoInfo = await _repository.GetLastByTask(tenantId, taskId);

            if (taskHistoryInfoInfo != null)
            {
                taskHistoryInfoInfo.EndDate = DateTime.UtcNow;
                await _repository.Update(taskHistoryInfoInfo);
            }
        }
    }
}