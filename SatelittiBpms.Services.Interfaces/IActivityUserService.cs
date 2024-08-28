using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IActivityUserService : IServiceBase<ActivityUserDTO, ActivityUserInfo>
    {
        Task InsertByDiagram(XmlNode nodeUserTask, int activityId, int processVersionId, int tenantId);
    }
}
