using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class ActivityUserOptionService : AbstractServiceBase<ActivityUserOptionDTO, ActivityUserOptionInfo, IActivityUserOptionRepository>, IActivityUserOptionService
    {
        public ActivityUserOptionService(
            IActivityUserOptionRepository repository,
            IMapper mapper) : base(repository, mapper)
        { }

        public async Task<List<ActivityUserOptionInfo>> ListByUserActivityId(List<int> userActivitiesID)
        {
            var result = await _repository.ListAsync();
            return result.Where(x => userActivitiesID.Contains(x.ActivityUserId)).ToList();
        }
    }
}
