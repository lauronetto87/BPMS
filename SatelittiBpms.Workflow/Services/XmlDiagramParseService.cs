using Newtonsoft.Json;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using SatelittiBpms.Workflow.Interfaces;
using SatelittiBpms.Workflow.WorkflowCoreElements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Workflow.Services
{
    public class XmlDiagramParseService : IXmlDiagramParseService
    {
        private readonly XmlNamespaceManager nsmgr;

        private readonly IActivityService _activityService;
        private readonly IXmlDiagramService _xmlDiagramService;
        private readonly IExclusiveGatewayService _exclusiveGatewayService;
        private readonly IActivityUserOptionService _activityUserOptionsService;

        public XmlDiagramParseService(
            IActivityService activityService,
            IXmlDiagramService xmlDiagramService,
            IExclusiveGatewayService exclusiveGatewayService,
            IActivityUserOptionService activityUserOptionsService)
        {
            var nameTable = new NameTable();
            nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace(XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX, XmlDiagramConstants.BPMN2_NAMESPACE_URI);

            _activityService = activityService;
            _xmlDiagramService = xmlDiagramService;
            _exclusiveGatewayService = exclusiveGatewayService;
            _activityUserOptionsService = activityUserOptionsService;
        }

        public async Task<string> Parse(XmlNode processNode, int processId, int version, int processVersionId, int tenantId)
        {
            WorkflowElement workflow = new()
            {
                DataType = "SatelittiBpms.Workflow.Models.FlowDataInfo, SatelittiBpms.Workflow",
                Id = processId.ToString(),
                Version = version
            };

            var filter = new Dictionary<string, string>
            {
                { "ProcessVersionId", processVersionId.ToString() }
            };

            List<ActivityInfo> lstActivities = await _activityService.ListAsync(filter);
            List<ActivityUserOptionInfo> lstOptions = await _activityUserOptionsService.ListByUserActivityId(lstActivities.Select(x => x.Id).ToList());

            var nodeStartEvent = processNode.ChildNodes.OfType<XmlElement>().FirstOrDefault(x => x.Name == $"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.START_EVENT_ACTIVITY}");
            workflow.Steps.Add(new StepElement()
            {
                Id = nodeStartEvent.Attributes["id"].Value,
                StepType = StartEventActivity.TypeDescription,
                NextStepId = _xmlDiagramService.GetNextStepNode(processNode, nodeStartEvent)?.Attributes["id"].Value,
                Inputs = StartEventActivity.GetInputs(tenantId, lstActivities.Find(x => x.ComponentInternalId == nodeStartEvent.Attributes["id"].Value).Id, processVersionId),
                Outputs = StartEventActivity.GetOutputs()
            });

            foreach (XmlNode nodeToProcess in processNode.ChildNodes.OfType<XmlElement>())
            {
                if (nodeToProcess.Name == $"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.END_EVENT_ACTIVITY}")
                {
                    workflow.Steps.Add(new StepElement()
                    {
                        Id = nodeToProcess.Attributes["id"].Value,
                        StepType = EndEventActivity.TypeDescription,
                        Inputs = EndEventActivity.GetInputs(tenantId, lstActivities.Find(x => x.ComponentInternalId == nodeToProcess.Attributes["id"].Value).Id)
                    });
                }
                if (nodeToProcess.Name == $"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.SEND_TASK_ACTIVITY}")
                {
                    workflow.Steps.Add(new StepElement()
                    {
                        Id = nodeToProcess.Attributes["id"].Value,
                        StepType = SendTaskActivity.TypeDescription,
                        NextStepId = _xmlDiagramService.GetNextStepNode(processNode, nodeToProcess)?.Attributes["id"].Value,
                        Inputs = SendTaskActivity.GetInputs(tenantId, lstActivities.Find(x => x.ComponentInternalId == nodeToProcess.Attributes["id"].Value).Id),
                        Outputs = SendTaskActivity.GetOutputs()
                    });
                }
                if (nodeToProcess.Name == $"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.USER_TASK_ACTIVITY}")
                {
                    var activity = lstActivities.Find(x => x.ComponentInternalId == nodeToProcess.Attributes["id"].Value);
                    workflow.Steps.Add(new StepElement()
                    {
                        Id = nodeToProcess.Attributes["id"].Value,
                        StepType = UserTaskActivity.TypeDescription,
                        NextStepId = _xmlDiagramService.GetNextStepNode(processNode, nodeToProcess)?.Attributes["id"].Value,
                        Inputs = UserTaskActivity.GetInputs(tenantId, activity.Id, activity.ActivityUser.ExecutorType),
                        Outputs = UserTaskActivity.GetOutputs()
                    });
                }
                if (nodeToProcess.Name == $"{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.EXCLUSIVE_GATEWAY_ACTIVITY}")
                {
                    workflow.Steps.Add(new StepElementExclusive()
                    {
                        Id = nodeToProcess.Attributes["id"].Value,
                        StepType = ExclusiveGatewayActivity.TypeDescription,
                        Inputs = ExclusiveGatewayActivity.GetInputs(tenantId, lstActivities.Find(x => x.ComponentInternalId == nodeToProcess.Attributes["id"].Value).Id),
                        Outputs = ExclusiveGatewayActivity.GetOutputs(),
                        SelectNextStep = _exclusiveGatewayService.AddBranchExclusiveGatewayActivity(processNode, nodeToProcess, lstOptions),
                    });
                }
                if (nodeToProcess.Name == $"{XmlDiagramConstants.SATELITTI_NAMESPACE_PREFIX}:{XmlDiagramConstants.SIGNER_INTEGRATION_ACTIVITY}")
                {
                    workflow.Steps.Add(new StepElement()
                    {
                        Id = nodeToProcess.Attributes["id"].Value,
                        StepType = SignerIntegrationActivity.TypeDescription,
                        Inputs = SignerIntegrationActivity.GetInputs(tenantId, lstActivities.Find(x => x.ComponentInternalId == nodeToProcess.Attributes["id"].Value).Id),
                        Outputs = SignerIntegrationActivity.GetOutputs(),
                        NextStepId = _xmlDiagramService.GetNextStepNode(processNode, nodeToProcess)?.Attributes["id"].Value,
                    });
                }
            }
            return JsonConvert.SerializeObject(workflow, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}
