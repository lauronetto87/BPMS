using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class FlowHistoryService : IFlowHistoryService
    {
        private readonly IFlowRepository _repository;
        private readonly IUserService _userService;
        private readonly IWildcardService _wildcardService;

        public FlowHistoryService(
            IFlowRepository repository,
            IUserService userService,
            IWildcardService wildcardService)
        {
            _repository = repository;
            _userService = userService;
            _wildcardService = wildcardService;
        }

        public async Task<ResultContent<FlowHistoryViewModel>> Get(int flowId)
        {
            var users = await _userService.ListUsersSuite();

            var flowInfo = _repository.GetQuery(flowId)
                .Include(f => f.ProcessVersion)
                .Include(f => f.Tasks.Where(f =>
                        f.Activity.Type == WorkflowActivityTypeEnum.START_EVENT_ACTIVITY ||
                        f.Activity.Type == WorkflowActivityTypeEnum.END_EVENT_ACTIVITY ||
                        f.Activity.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY ||
                        f.Activity.Type == WorkflowActivityTypeEnum.SEND_TASK_ACTIVITY)
                    ).ThenInclude(t => t.Activity)
                .Include(f => f.Tasks).ThenInclude(t => t.Option)
                .Include(f => f.Tasks).ThenInclude(t => t.FieldsValues).ThenInclude(fv => fv.Field)
                .First();

            var flowViewModel = flowInfo.AsHistoryViewModel(users);
            flowViewModel.FlowDescription = _wildcardService.FormatDescriptionWildcard(flowInfo.ProcessVersion.DescriptionFlow, flowInfo, users);
            return new ResultContent<FlowHistoryViewModel>(flowViewModel, true, null);
        }
    }
}