

using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IActivityNotificationService : IServiceBase<ActivityNotificationDTO, ActivityNotificationInfo>
    {
        Task InsertByDiagram(XmlNode nodeNotificationTask, int activityId, int tenantId);
    }
}
