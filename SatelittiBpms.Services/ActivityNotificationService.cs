using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services
{

    public class ActivityNotificationService : AbstractServiceBase<ActivityNotificationDTO, ActivityNotificationInfo, IActivityNotificationRepository>, IActivityNotificationService
    {        
        private readonly IXmlDiagramService _xmlDiagramService;

        public ActivityNotificationService(
            IXmlDiagramService xmlDiagramService,
            IActivityNotificationRepository repository,
            IMapper mapper) : base(repository, mapper)
        {
            _xmlDiagramService = xmlDiagramService;
        }

        public async Task InsertByDiagram(XmlNode nodeNotificationTask, int activityId, int tenantId)
        {
            var destinataryType = _xmlDiagramService.GetSendTaskDestinataryType(nodeNotificationTask);

            await _repository.Insert(new ActivityNotificationInfo
            {
                Id = activityId,
                TenantId = tenantId,
                DestinataryType = destinataryType,
                RoleId = destinataryType == SendTaskDestinataryTypeEnum.ROLE ? _xmlDiagramService.GetDestinataryIdAttributeValue(nodeNotificationTask) : null,
                PersonId = destinataryType == SendTaskDestinataryTypeEnum.PERSON ? _xmlDiagramService.GetDestinataryIdAttributeValue(nodeNotificationTask) : null,
                CustomEmail = destinataryType == SendTaskDestinataryTypeEnum.CUSTOM ? _xmlDiagramService.GetCustomEmailAttributeValue(nodeNotificationTask) : null,
                TitleMessage = _xmlDiagramService.GetTitleMessageNotification(nodeNotificationTask),
                Message = _xmlDiagramService.GetMessageNotification(nodeNotificationTask),
            });
        }
    }
}
