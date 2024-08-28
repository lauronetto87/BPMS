using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IActivityService : IServiceBase<ActivityDTO, ActivityInfo>
    {
        Task InsertMany(List<ActivityDTO> activities, int processVersionId, int tenantId);
        int? GetId(string Name, int processVersionId, int tenantId);
        Task<ResultContent<ActivityInfo>> GetByTenant(int processId, int tenantId);
        Task InsertByDiagram(XmlNode processNode, int processVersionId, int tenantId);
    }
}
