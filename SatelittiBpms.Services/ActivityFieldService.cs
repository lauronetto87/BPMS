using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;

namespace SatelittiBpms.Services
{
    public class ActivityFieldService : AbstractServiceBase<ActivityFieldDTO, ActivityFieldInfo, IActivityFieldRepository>, IActivityFieldService
    {
        public ActivityFieldService(IActivityFieldRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
