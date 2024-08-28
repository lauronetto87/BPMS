using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class FlowPathService : AbstractServiceBase<FlowPathDTO, FlowPathInfo, IFlowPathRepository>, IFlowPathService
    {
        public FlowPathService(IFlowPathRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public int getPreviousTaskId(int taskId)
        {
            return _repository.getFlowPathInfoByTargetTaskId(taskId).Result.SourceTaskId;
        }

        public async Task<ResultContent<int>> Insert(FlowPathInfo info)
        {
            return Result.Success(await _repository.Insert(info));
        }
    }
}
