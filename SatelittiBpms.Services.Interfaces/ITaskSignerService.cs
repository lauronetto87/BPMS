using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ITaskSignerService : IServiceBase<TaskSignerDTO, TaskSignerInfo>
    {
        Task<ResultContent> ActionPerformedOnsigner(ActionPerformedOnSignerDTO actionPerformedOnSigner);
    }
}
