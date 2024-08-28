using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;

namespace SatelittiBpms.Services
{
    public class FlowService : AbstractServiceBase<FlowDTO, FlowInfo, IFlowRepository>, IFlowService
    {
        public FlowService(
              IFlowRepository repository,
              IMapper mapper,
              IProcessRepository processRepository) : base(repository, mapper)
        {
        }

    }
}