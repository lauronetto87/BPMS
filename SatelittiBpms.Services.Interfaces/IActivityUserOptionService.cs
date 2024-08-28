using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IActivityUserOptionService : IServiceBase<ActivityUserOptionDTO, ActivityUserOptionInfo>
    {
        Task<List<ActivityUserOptionInfo>> ListByUserActivityId(List<int> userActivitiesID);
    }
}
