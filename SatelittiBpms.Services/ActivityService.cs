using AutoMapper;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services
{
    public class ActivityService : AbstractServiceBase<ActivityDTO, ActivityInfo, IActivityRepository>, IActivityService
    {
        private readonly IFieldService _fieldService;
        private readonly IActivityFieldService _activityFieldService;
        private readonly IActivityUserService _activityUserService;
        private readonly IXmlDiagramService _xmlDiagramService;
        private readonly IActivityNotificationService _activityNotificationService;

        public ActivityService(
            IActivityRepository repository,
            IMapper mapper,
            IFieldService fieldService,
            IActivityFieldService activityFieldService,
            IActivityUserService activityUserService,
            IXmlDiagramService xmlDiagramService,
            IActivityNotificationService activityNotificationService) : base(repository, mapper)
        {
            _fieldService = fieldService;
            _activityFieldService = activityFieldService;
            _activityUserService = activityUserService;
            _xmlDiagramService = xmlDiagramService;
            _activityNotificationService = activityNotificationService;
        }

        public int? GetId(string componentInternalId, int processVersionId, int tenantId)
        {
            return _repository.GetByTenant(tenantId).FirstOrDefault(x => x.ComponentInternalId == componentInternalId && x.ProcessVersionId == processVersionId)?.Id;
        }

        public async Task<ResultContent<ActivityInfo>> GetByTenant(int activityId, int tenantId)
        {
            var activity = await _repository.GetByIdAndTenantId(activityId, tenantId);
            return Result.Success(activity);
        }

        public async Task InsertByDiagram(XmlNode processNode, int processVersionId, int tenantId)
        {

            foreach (XmlNode activityNode in processNode.ChildNodes.OfType<XmlElement>())
            {
                var activityId = GetId(_xmlDiagramService.GetAttributeValue(activityNode, "id"), processVersionId, tenantId);
                var activityType = ActivityType(activityNode);

                if (activityId == null && activityType != WorkflowActivityTypeEnum.SEQUENCE_FLOW && activityType != WorkflowActivityTypeEnum.LANESET)
                {
                    activityId = await InsertActivity(new ActivityInfo
                    {
                        ComponentInternalId = _xmlDiagramService.GetAttributeValue(activityNode, "id"),
                        Name = _xmlDiagramService.GetAttributeValue(activityNode, "name"),
                        ProcessVersionId = processVersionId,
                        TenantId = tenantId,
                        Type = activityType
                    });
                }

                if (activityType == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY)
                    await _activityUserService.InsertByDiagram(activityNode, activityId.Value, processVersionId, tenantId);

                if (activityType == WorkflowActivityTypeEnum.SEND_TASK_ACTIVITY)
                    await _activityNotificationService.InsertByDiagram(activityNode, activityId.Value, tenantId);
                
            }
        }

        private WorkflowActivityTypeEnum ActivityType(XmlNode activityNode)
        {
            var activityNodeTypeName = activityNode.LocalName;
            var activityNodeName = _xmlDiagramService.GetAttributeValue(activityNode, "name");
            if (string.IsNullOrEmpty(activityNodeName))
                activityNodeName = _xmlDiagramService.GetAttributeValue(activityNode, "id");

            switch (activityNodeTypeName)
            {
                case XmlDiagramConstants.USER_TASK_ACTIVITY:
                    return WorkflowActivityTypeEnum.USER_TASK_ACTIVITY;
                case XmlDiagramConstants.START_EVENT_ACTIVITY:
                    return WorkflowActivityTypeEnum.START_EVENT_ACTIVITY;
                case XmlDiagramConstants.SEND_TASK_ACTIVITY:
                    return WorkflowActivityTypeEnum.SEND_TASK_ACTIVITY;
                case XmlDiagramConstants.END_EVENT_ACTIVITY:
                    return WorkflowActivityTypeEnum.END_EVENT_ACTIVITY;
                case XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY:
                    return WorkflowActivityTypeEnum.EXCLUSIVE_GATEWAY_ACTIVITY;
                case XmlDiagramConstants.LANESET:
                    return WorkflowActivityTypeEnum.LANESET;
                case XmlDiagramConstants.SEQUENCE_FLOW_NODE_NAME:
                    return WorkflowActivityTypeEnum.SEQUENCE_FLOW;
                case XmlDiagramConstants.SIGNER_TASK:
                    return WorkflowActivityTypeEnum.SIGNER_TASK;
                default:
                    throw new ActivityTypeNotExpectedException(activityNodeName);
            };
        }

        private async Task<int> InsertActivity(ActivityInfo activityInfo)
        {
            return await _repository.Insert(activityInfo);
        }

        public async Task InsertMany(List<ActivityDTO> activities, int processVersionId, int tenantId)
        {
            if (activities == null)
                return;

            Dictionary<string, int> fieldsIds = new Dictionary<string, int>();
            foreach (var activity in activities)
            {
                activity.SetTenantId(tenantId);
                activity.ProcessVersionId = processVersionId;
                activity.ActivityType = Models.Enums.WorkflowActivityTypeEnum.USER_TASK_ACTIVITY;

                var activityId = await _repository.Insert(_mapper.Map<ActivityInfo>(activity));

                foreach (var activityField in activity.Fields)
                {
                    if (!fieldsIds.ContainsKey(activityField.FieldId))
                    {
                        FieldDTO fieldDTO = _mapper.Map<FieldDTO>(activityField);
                        fieldDTO.SetTenantId(tenantId);
                        fieldDTO.ProcessVersionId = processVersionId;
                        var fieldInsertResult = await _fieldService.Insert(fieldDTO);
                        fieldsIds.Add(activityField.FieldId, fieldInsertResult.Value);
                    }

                    activityField.SetTenantId(tenantId);
                    activityField.TaskId = activityId;
                    activityField.ProcessVersionId = processVersionId;
                    activityField.SystemFieldId = fieldsIds[activityField.FieldId];
                    await _activityFieldService.Insert(activityField);
                }
            }
        }
    }
}
