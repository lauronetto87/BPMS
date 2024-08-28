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
    public class ActivityUserService : AbstractServiceBase<ActivityUserDTO, ActivityUserInfo, IActivityUserRepository>, IActivityUserService
    {        
        private readonly IActivityUserOptionService _activityUserOptionService;
        private readonly IXmlDiagramService _xmlDiagramService;

        public ActivityUserService(
            IActivityUserOptionService activityUserOptionService,
            IXmlDiagramService xmlDiagramService,
            IActivityUserRepository repository,
            IMapper mapper) : base(repository, mapper)
        {
            _activityUserOptionService = activityUserOptionService;
            _xmlDiagramService = xmlDiagramService;
        }

        public async Task InsertByDiagram(XmlNode nodeUserTask, int activityId, int processVersionId, int tenantId)
        {

            var executorType = _xmlDiagramService.GetUserTaskExecutorType(nodeUserTask);

            int activityUserId = await _repository.Insert(new ActivityUserInfo
            {
                Id = activityId,
                ExecutorType = executorType,
                TenantId = tenantId,
                RoleId = executorType == UserTaskExecutorTypeEnum.ROLE ? _xmlDiagramService.GetExecutorIdAttributeValue(nodeUserTask) : null,
                PersonId = executorType == UserTaskExecutorTypeEnum.PERSON ? _xmlDiagramService.GetPersonIdAttributeValue(nodeUserTask) : null
            });


            foreach (XmlNode taskOption in _xmlDiagramService.ListOptionNodes(nodeUserTask))
            {
                var activityUserOption = new ActivityUserOptionDTO
                {
                    Description = _xmlDiagramService.GetAttributeValue(taskOption, "description"),
                    ActivityUserId = activityUserId
                };

                activityUserOption.SetTenantId(tenantId);
                await _activityUserOptionService.Insert(activityUserOption);
            }
        }
    }
}
