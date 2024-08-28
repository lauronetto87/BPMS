using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using SatelittiBpms.Workflow.Interfaces;
using SatelittiBpms.Workflow.Services;
using SatelittiBpms.Workflow.WorkflowCoreElements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Workflow.Tests.Services
{
    public class XmlDiagramParseServiceTest
    {
        const int START_EVENT_INPUTS_COUNT = 4;
        const int END_EVENT_INPUTS_COUNT = 5;
        const int USER_TASK_INPUTS_COUNT = 7;
        const int SEND_TASK_INPUTS_COUNT = 6;

        int processId = 3,
            version = 5,
            processVersionId = 99,
            tenantId = 55;

        Mock<IActivityService> _mockActivityService;
        Mock<IXmlDiagramService> _mockXmlDiagramService;
        Mock<IExclusiveGatewayService> _mockExclusiveGatewayService;
        Mock<IActivityUserOptionService> _mockActivityUserOptionService;

        IXmlDiagramService _xmlDiagramService;

        private XmlNamespaceManager nsmgr;

        [SetUp]
        public void Setup()
        {
            _mockActivityService = new Mock<IActivityService>();
            _mockXmlDiagramService = new Mock<IXmlDiagramService>();
            _mockExclusiveGatewayService = new Mock<IExclusiveGatewayService>();
            _mockActivityUserOptionService = new Mock<IActivityUserOptionService>();
            _xmlDiagramService = new XmlDiagramService();

            NameTable nameTable = new NameTable();
            nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace(XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX, XmlDiagramConstants.BPMN2_NAMESPACE_URI);
        }

        [Test]
        public async Task ensureThatParseConvertStartEvent()
        {
            int activityId = 7;
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_StartEvent));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object); ;
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = activityId,
                    ComponentInternalId = "StartEvent_1"
                }
            });
            _mockXmlDiagramService.Setup(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(() => null);

            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);


            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(1, workflowDefinition.Steps.Count);
            var activity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", activity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, activity.StepType);
            Assert.IsNull(activity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, activity.Inputs.Count);
            Assert.IsTrue(activity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", activity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", activity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{activityId}\"", activity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", activity.Inputs["RequesterId"]);
            Assert.AreEqual(2, activity.Outputs.Count);
            Assert.IsTrue(activity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(activity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", activity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", activity.Outputs["TaskId"]);
        }

        [Test]
        public async Task ensureThatParseConvertEndEvent()
        {
            int activityId = 8, startActivityId = 8;
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_EndEvent));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1"
                },
                new ActivityInfo(){
                    Id = activityId,
                    ComponentInternalId = "Event_1gka9ds"
                }
            });
            var testNode = bpmnXml.CreateElement("test");
            testNode.SetAttribute("id", "Event_1gka9ds");
            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(testNode).Returns(() => null);

            _mockXmlDiagramService.Setup(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(() => null);

            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);

            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(2, workflowDefinition.Steps.Count);

            var startActivity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.IsNull(startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            var activity = (StepElement)workflowDefinition.Steps[1];
            Assert.AreEqual("Event_1gka9ds", activity.Id);
            Assert.AreEqual(EndEventActivity.TypeDescription, activity.StepType);
            Assert.IsNull(activity.NextStepId);
            Assert.AreEqual(END_EVENT_INPUTS_COUNT, activity.Inputs.Count);
            Assert.IsTrue(activity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", activity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{activityId}\"", activity.Inputs["ActivityId"]);
            Assert.AreEqual("data.FlowId", activity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", activity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", activity.Inputs["ConnectionId"]);
        }

        [Test]
        public async Task ensureThatParseConvertStartAndEndEvent()
        {
            int startActivityId = 8,
                endActivityId = 79;
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_StartAndEndEvents));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = endActivityId,
                    ComponentInternalId = "Event_1gka9ds"
                },
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1"
                }
            });
            var testNode = bpmnXml.CreateElement("test");
            testNode.SetAttribute("id", "Activity_3h6dets");
            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(testNode).Returns(() => null);

            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);

            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(2, workflowDefinition.Steps.Count);

            var startActivity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.AreEqual("Activity_3h6dets", startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            var endActivity = (StepElement)workflowDefinition.Steps[1];
            Assert.AreEqual("Event_1gka9ds", endActivity.Id);
            Assert.AreEqual(EndEventActivity.TypeDescription, endActivity.StepType);
            Assert.IsNull(endActivity.NextStepId);
            Assert.AreEqual(END_EVENT_INPUTS_COUNT, endActivity.Inputs.Count);
            Assert.IsTrue(endActivity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", endActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{endActivityId}\"", endActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.FlowId", endActivity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", endActivity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", endActivity.Inputs["ConnectionId"]);
        }

        [Test]
        public async Task ensureThatParseConvertSendTaskEvent()
        {
            int activityId = 7, startActivityId = 8;

            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_SendTaskEvents));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1"
                },
                new ActivityInfo(){
                    Id = activityId,
                    ComponentInternalId = "Activity_1wu7de8"
                }
            });
            _mockXmlDiagramService.Setup(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(() => null);

            var startNextTestNode = bpmnXml.CreateElement("test");
            startNextTestNode.SetAttribute("id", "Activity_3h6dets");
            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(startNextTestNode).Returns(() => null);

            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);

            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(2, workflowDefinition.Steps.Count);

            var startActivity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.AreEqual("Activity_3h6dets", startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            var activity = (StepElement)workflowDefinition.Steps[1];
            Assert.AreEqual("Activity_1wu7de8", activity.Id);
            Assert.AreEqual(SendTaskActivity.TypeDescription, activity.StepType);
            Assert.IsNull(activity.NextStepId);
            Assert.AreEqual(SEND_TASK_INPUTS_COUNT, activity.Inputs.Count);
            Assert.IsTrue(activity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", activity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{activityId}\"", activity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", activity.Inputs["RequesterId"]);
            Assert.AreEqual("data.FlowId", activity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", activity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", activity.Inputs["ConnectionId"]);
            Assert.AreEqual(1, activity.Outputs.Count);
            Assert.IsTrue(activity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.TaskId", activity.Outputs["TaskId"]);
        }

        [Test]
        public async Task ensureThatParseConvertUserTaskEvent()
        {
            int activityId = 7, startActivityId = 8;
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_UserTaskEvents));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = activityId,
                    ComponentInternalId = "Activity_0dzz8m7",
                    ActivityUser = new ActivityUserInfo(){
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                },
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1"
                },
            });

            var startNextTestNode = bpmnXml.CreateElement("test");
            startNextTestNode.SetAttribute("id", "Activity_3h6dets");
            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(startNextTestNode).Returns(() => null);
            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);

            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(2, workflowDefinition.Steps.Count);

            var startActivity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.AreEqual("Activity_3h6dets", startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            var activity = (StepElement)workflowDefinition.Steps[1];
            Assert.AreEqual("Activity_0dzz8m7", activity.Id);
            Assert.AreEqual(UserTaskActivity.TypeDescription, activity.StepType);
            Assert.IsNull(activity.NextStepId);
            Assert.AreEqual(USER_TASK_INPUTS_COUNT, activity.Inputs.Count);
            Assert.IsTrue(activity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(activity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual("data.ConnectionId", activity.Inputs["ConnectionId"]);
            Assert.AreEqual($"\"{tenantId}\"", activity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{activityId}\"", activity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", activity.Inputs["RequesterId"]);
            Assert.AreEqual("data.FlowId", activity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", activity.Inputs["TaskId"]);
            Assert.AreEqual($"\"{UserTaskExecutorTypeEnum.REQUESTER}\"", activity.Inputs["ActivityUserExecutorType"]);
            Assert.AreEqual(2, activity.Outputs.Count);
            Assert.IsTrue(activity.Outputs.ContainsKey("TaskId"));
            Assert.IsTrue(activity.Outputs.ContainsKey("Option"));
            Assert.AreEqual("step.TaskId", activity.Outputs["TaskId"]);
            Assert.AreEqual("step.Option", activity.Outputs["Option"]);
        }

        [Test]
        public async Task ensureThatParseConvertExclusiveGatewayEvent()
        {
            int startActivityId = 8,
                activityId = 7;

            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_ExclusiveGatewayEvents));
            var nodeToProcess = bpmnXml.GetElementsByTagName("bpmn2:process")[0];

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = activityId,
                    ComponentInternalId = "Gateway_08ia5yb"
                },
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1"
                }
            });

            var startNextTestNode = bpmnXml.CreateElement("test");
            startNextTestNode.SetAttribute("id", "Activity_3h6dets");

            var dicBranch = new Dictionary<string, object>();
            dicBranch.Add("Activity_0cq0rxb", $"data.Option == 'Opcao1'");
            dicBranch.Add("Activity_1n1t4l5", $"data.Option == 'Opcao2'");

            XmlNodeList outgoingNode = nodeToProcess.SelectNodes($".//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:{XmlDiagramConstants.OUTGOING_NODE_NAME}", nsmgr);
            XmlNode singleNode = nodeToProcess.ChildNodes[0].SelectSingleNode($"//{XmlDiagramConstants.BPMN2_NAMESPACE_PREFIX}:sequenceFlow[@id='Flow_07ygqtw']", nsmgr);

            _mockXmlDiagramService.Setup(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(() => null);
            _mockXmlDiagramService.Setup(x => x.SelectSequenceFlow(It.IsAny<XmlNode>(), It.IsAny<string>())).Returns(singleNode);
            _mockExclusiveGatewayService.Setup(x => x.AddBranchExclusiveGatewayActivity(It.IsAny<XmlNode>(), It.IsAny<XmlNode>(), It.IsAny<List<ActivityUserOptionInfo>>())).Returns(dicBranch);

            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(startNextTestNode).Returns(() => null);
            string result = await xmlDiagramParseService.Parse(nodeToProcess, processId, version, processVersionId, tenantId);

            WorkflowElement stepElementExclusive = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            var startActivity = (StepElement)stepElementExclusive.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.AreEqual("Activity_3h6dets", startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            Assert.AreEqual(processId.ToString(), stepElementExclusive.Id);
            Assert.AreEqual(version, stepElementExclusive.Version);
            var exclusive = (StepElementExclusive)stepElementExclusive.Steps[1];


            Assert.AreEqual("Gateway_08ia5yb", exclusive.Id);
            Assert.AreEqual(ExclusiveGatewayActivity.TypeDescription, exclusive.StepType);
            Assert.AreEqual(2, exclusive.SelectNextStep.Count());
            Assert.AreEqual("data.Option == 'Opcao1'", exclusive.SelectNextStep["Activity_0cq0rxb"]);
            Assert.AreEqual("data.Option == 'Opcao2'", exclusive.SelectNextStep["Activity_1n1t4l5"]);
        }

        [Test]
        public async Task ensureThatParseConvertStartAndEndEventAndSendTask()
        {
            int startActivityId = 8,
                sendTaskActivityId = 11,
                endActivityId = 79;
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_StartAndEndEventsAndSendTask));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = endActivityId,
                    ComponentInternalId = "Event_1gka9ds"
                },
                new ActivityInfo(){
                    Id = sendTaskActivityId,
                    ComponentInternalId = "Activity_1wu7de8"
                },
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1"
                }
            });
            var startNextTestNode = bpmnXml.CreateElement("test");
            startNextTestNode.SetAttribute("id", "Activity_3h6dets");
            var sendNextTestNode = bpmnXml.CreateElement("test");
            sendNextTestNode.SetAttribute("id", "Activity_3kkkkjs");
            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(startNextTestNode).Returns(sendNextTestNode).Returns(() => null);
            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);

            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(3, workflowDefinition.Steps.Count);

            var startActivity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.AreEqual("Activity_3h6dets", startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            var sendTaskActivity = (StepElement)workflowDefinition.Steps[1];
            Assert.AreEqual("Activity_1wu7de8", sendTaskActivity.Id);
            Assert.AreEqual(SendTaskActivity.TypeDescription, sendTaskActivity.StepType);
            Assert.AreEqual("Activity_3kkkkjs", sendTaskActivity.NextStepId);
            Assert.AreEqual(SEND_TASK_INPUTS_COUNT, sendTaskActivity.Inputs.Count);
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", sendTaskActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{sendTaskActivityId}\"", sendTaskActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", sendTaskActivity.Inputs["RequesterId"]);
            Assert.AreEqual("data.FlowId", sendTaskActivity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", sendTaskActivity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", sendTaskActivity.Inputs["ConnectionId"]);
            Assert.AreEqual(1, sendTaskActivity.Outputs.Count);
            Assert.IsTrue(sendTaskActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.TaskId", sendTaskActivity.Outputs["TaskId"]);

            var endActivity = (StepElement)workflowDefinition.Steps[2];
            Assert.AreEqual("Event_1gka9ds", endActivity.Id);
            Assert.AreEqual(EndEventActivity.TypeDescription, endActivity.StepType);
            Assert.IsNull(endActivity.NextStepId);
            Assert.AreEqual(END_EVENT_INPUTS_COUNT, endActivity.Inputs.Count);
            Assert.IsTrue(endActivity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", endActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{endActivityId}\"", endActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.FlowId", endActivity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", endActivity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", endActivity.Inputs["ConnectionId"]);

        }

        [Test]
        public async Task ensureThatParseConvertStartEndSendUser()
        {
            int startActivityId = 8,
                sendTaskActivityId = 11,
                userTaskActivityId = 13,
                endActivityId = 79;
            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_StartEndSendUser));

            XmlDiagramParseService xmlDiagramParseService = new XmlDiagramParseService(_mockActivityService.Object, _mockXmlDiagramService.Object, _mockExclusiveGatewayService.Object, _mockActivityUserOptionService.Object);
            _mockActivityService.Setup(x => x.ListAsync(It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new List<ActivityInfo>()
            {
                new ActivityInfo(){
                    Id = endActivityId,
                    ComponentInternalId = "Event_1gka9ds",
                    ActivityUser = new ActivityUserInfo(){
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                },
                new ActivityInfo(){
                    Id = userTaskActivityId,
                    ComponentInternalId = "Activity_0dzz8m7",
                    ActivityUser = new ActivityUserInfo(){
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                },
                new ActivityInfo(){
                    Id = sendTaskActivityId,
                    ComponentInternalId = "Activity_1wu7de8",
                    ActivityUser = new ActivityUserInfo(){
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                },
                new ActivityInfo(){
                    Id = startActivityId,
                    ComponentInternalId = "StartEvent_1",
                    ActivityUser = new ActivityUserInfo(){
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                }
            });
            var startNextTestNode = bpmnXml.CreateElement("test");
            startNextTestNode.SetAttribute("id", "Activity_3h6dets");
            var sendNextTestNode = bpmnXml.CreateElement("test");
            sendNextTestNode.SetAttribute("id", "Activity_3kkkkjs");
            var userNextTestNode = bpmnXml.CreateElement("test");
            userNextTestNode.SetAttribute("id", "Activity_1wu7de8");
            _mockXmlDiagramService.SetupSequence(x => x.GetNextStepNode(It.IsAny<XmlNode>(), It.IsAny<XmlNode>())).Returns(startNextTestNode).Returns(sendNextTestNode).Returns(userNextTestNode).Returns(() => null);
            string result = await xmlDiagramParseService.Parse(bpmnXml.GetElementsByTagName("bpmn2:process")[0], processId, version, processVersionId, tenantId);

            WorkflowElement workflowDefinition = JsonConvert.DeserializeObject<WorkflowElement>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Assert.AreEqual(processId.ToString(), workflowDefinition.Id);
            Assert.AreEqual(version, workflowDefinition.Version);
            Assert.AreEqual(4, workflowDefinition.Steps.Count);

            var startActivity = (StepElement)workflowDefinition.Steps[0];
            Assert.AreEqual("StartEvent_1", startActivity.Id);
            Assert.AreEqual(StartEventActivity.TypeDescription, startActivity.StepType);
            Assert.AreEqual("Activity_3h6dets", startActivity.NextStepId);
            Assert.AreEqual(START_EVENT_INPUTS_COUNT, startActivity.Inputs.Count);
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ProcessVersionId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(startActivity.Inputs.ContainsKey("TenantId"));
            Assert.AreEqual($"\"{processVersionId}\"", startActivity.Inputs["ProcessVersionId"]);
            Assert.AreEqual($"\"{tenantId}\"", startActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{startActivityId}\"", startActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", startActivity.Inputs["RequesterId"]);
            Assert.AreEqual(2, startActivity.Outputs.Count);
            Assert.IsTrue(startActivity.Outputs.ContainsKey("FlowId"));
            Assert.IsTrue(startActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.FlowId", startActivity.Outputs["FlowId"]);
            Assert.AreEqual("step.TaskId", startActivity.Outputs["TaskId"]);

            var sendTaskActivity = (StepElement)workflowDefinition.Steps[1];
            Assert.AreEqual("Activity_1wu7de8", sendTaskActivity.Id);
            Assert.AreEqual(SendTaskActivity.TypeDescription, sendTaskActivity.StepType);
            Assert.AreEqual("Activity_3kkkkjs", sendTaskActivity.NextStepId);
            Assert.AreEqual(SEND_TASK_INPUTS_COUNT, sendTaskActivity.Inputs.Count);
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(sendTaskActivity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", sendTaskActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{sendTaskActivityId}\"", sendTaskActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", sendTaskActivity.Inputs["RequesterId"]);
            Assert.AreEqual("data.FlowId", sendTaskActivity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", sendTaskActivity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", sendTaskActivity.Inputs["ConnectionId"]);
            Assert.AreEqual(1, sendTaskActivity.Outputs.Count);
            Assert.IsTrue(sendTaskActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.TaskId", sendTaskActivity.Outputs["TaskId"]);

            var userTaskActivity = (StepElement)workflowDefinition.Steps[2];
            Assert.AreEqual("Activity_0dzz8m7", userTaskActivity.Id);
            Assert.AreEqual(UserTaskActivity.TypeDescription, userTaskActivity.StepType);
            Assert.AreEqual("Activity_1wu7de8", userTaskActivity.NextStepId);
            Assert.AreEqual(USER_TASK_INPUTS_COUNT, userTaskActivity.Inputs.Count);
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("RequesterId"));
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("TaskId"));
            Assert.IsTrue(userTaskActivity.Inputs.ContainsKey("ActivityUserExecutorType"));
            Assert.AreEqual("data.ConnectionId", userTaskActivity.Inputs["ConnectionId"]);
            Assert.AreEqual($"\"{tenantId}\"", userTaskActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{userTaskActivityId}\"", userTaskActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.RequesterId", userTaskActivity.Inputs["RequesterId"]);
            Assert.AreEqual("data.FlowId", userTaskActivity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", userTaskActivity.Inputs["TaskId"]);
            Assert.AreEqual($"\"{UserTaskExecutorTypeEnum.REQUESTER}\"", userTaskActivity.Inputs["ActivityUserExecutorType"]);
            Assert.AreEqual(2, userTaskActivity.Outputs.Count);
            Assert.IsTrue(userTaskActivity.Outputs.ContainsKey("Option"));
            Assert.IsTrue(userTaskActivity.Outputs.ContainsKey("TaskId"));
            Assert.AreEqual("step.Option", userTaskActivity.Outputs["Option"]);
            Assert.AreEqual("step.TaskId", userTaskActivity.Outputs["TaskId"]);

            var endActivity = (StepElement)workflowDefinition.Steps[3];
            Assert.AreEqual("Event_1gka9ds", endActivity.Id);
            Assert.AreEqual(EndEventActivity.TypeDescription, endActivity.StepType);
            Assert.IsNull(endActivity.NextStepId);
            Assert.AreEqual(END_EVENT_INPUTS_COUNT, endActivity.Inputs.Count);
            Assert.IsTrue(endActivity.Inputs.ContainsKey("ConnectionId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("ActivityId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("TenantId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("FlowId"));
            Assert.IsTrue(endActivity.Inputs.ContainsKey("TaskId"));
            Assert.AreEqual($"\"{tenantId}\"", endActivity.Inputs["TenantId"]);
            Assert.AreEqual($"\"{endActivityId}\"", endActivity.Inputs["ActivityId"]);
            Assert.AreEqual("data.FlowId", endActivity.Inputs["FlowId"]);
            Assert.AreEqual("data.TaskId", endActivity.Inputs["TaskId"]);
            Assert.AreEqual("data.ConnectionId", endActivity.Inputs["ConnectionId"]);

        }

        [Test]
        public void ensureThatSelectIncomingValueByProcessNode()
        {
            var incomingValue = string.Empty;

            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_ProcessWithSimpleDecision));
            var processNode = bpmnXml.GetElementsByTagName("bpmn2:process")[0];

            foreach (XmlNode node in processNode.ChildNodes.OfType<XmlElement>())
            {
                if (node.Name == "bpmn2:exclusiveGateway")
                {
                    incomingValue = _xmlDiagramService.SelectIncomingValue(node);
                    break;
                }
            }

            Assert.AreEqual("Flow_1pea3ty", incomingValue);
        }

        [Test]
        public void ensureThatSelectNodeWithOutgoingByProcessNode()
        {
            var incomingValue = string.Empty;
            var nodeWithOutoing = new object();

            XmlDocument bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(System.Text.Encoding.UTF8.GetString(Properties.Resources.diagramParseTest_ProcessWithSimpleDecision));
            var processNode = bpmnXml.GetElementsByTagName("bpmn2:process")[0];
            var userTaskCondition = bpmnXml.GetElementsByTagName("bpmn2:userTask")[0];

            foreach (XmlNode node in processNode.ChildNodes.OfType<XmlElement>())
            {
                if (node.Name == "bpmn2:exclusiveGateway")
                {
                    incomingValue = _xmlDiagramService.SelectIncomingValue(node);
                    nodeWithOutoing = _xmlDiagramService.SelectNodeWithOutgoing(processNode, incomingValue);
                    break;
                }
            }

            Assert.AreEqual(userTaskCondition, nodeWithOutoing);
            Assert.AreEqual("Flow_1pea3ty", incomingValue);
        }
    }
}
