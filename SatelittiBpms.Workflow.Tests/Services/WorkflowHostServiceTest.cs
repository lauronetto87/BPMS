using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Models;
using SatelittiBpms.Workflow.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Tests.Services
{
    public class WorkflowHostServiceTest
    {
        Mock<IWorkflowHost> _mockWorkflowHost;
        Mock<IDefinitionLoader> _mockDefinitionLoader;
        Mock<IProcessService> _mockProcessService;
        Mock<ILogger<WorkflowHostService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockWorkflowHost = new Mock<IWorkflowHost>();
            _mockDefinitionLoader = new Mock<IDefinitionLoader>();
            _mockProcessService = new Mock<IProcessService>();
            _mockLogger = new Mock<ILogger<WorkflowHostService>>();
        }

        [Test]
        public void ensureThatLoadWorkflowFromJsonCallLoad()
        {
            string json = "{ type: 'new' }";
            _mockDefinitionLoader.Setup(x => x.LoadDefinition(It.IsAny<string>(), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()));

            WorkflowHostService workflowHostService = new WorkflowHostService(_mockWorkflowHost.Object, _mockDefinitionLoader.Object, _mockProcessService.Object, _mockLogger.Object);
            workflowHostService.LoadWorkflowFromJson(json);

            _mockDefinitionLoader.Verify(x => x.LoadDefinition(It.Is<string>(x => x == json), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()), Times.Once());
        }

        [Test]
        public void ensureThatLoadPublishedWorkflowsNoLoadWorkflowsWhenNoPublishedProcess()
        {
            List<string> workFlowList = new List<string>()
            {
            };

            _mockProcessService.Setup(x => x.ListWorkFlows()).Returns(workFlowList);
            _mockDefinitionLoader.Setup(x => x.LoadDefinition(It.IsAny<string>(), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()));

            WorkflowHostService workflowHostService = new WorkflowHostService(_mockWorkflowHost.Object, _mockDefinitionLoader.Object, _mockProcessService.Object, _mockLogger.Object);
            workflowHostService.LoadPublishedWorkflows();


            _mockProcessService.Verify(x => x.ListWorkFlows(), Times.Once());
            _mockDefinitionLoader.Verify(x => x.LoadDefinition(It.IsAny<string>(), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()), Times.Never());
        }

        [Test]
        public void ensureThatLoadPublishedWorkflowsLoadOnlyPublishedProcessWhenHaveOneProcess()
        {
            string json = "this JSON should be load";

            List<string> workFlowList = new List<string>(new string[] { "", json });

            _mockProcessService.Setup(x => x.ListWorkFlows()).Returns(workFlowList);
            _mockDefinitionLoader.Setup(x => x.LoadDefinition(It.IsAny<string>(), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()));

            WorkflowHostService workflowHostService = new WorkflowHostService(_mockWorkflowHost.Object, _mockDefinitionLoader.Object, _mockProcessService.Object, _mockLogger.Object);
            workflowHostService.LoadPublishedWorkflows();

            _mockProcessService.Verify(x => x.ListWorkFlows(), Times.Once());
            _mockDefinitionLoader.Verify(x => x.LoadDefinition(It.Is<string>(x => x == json), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()), Times.Once());
        }

        [Test]
        public void ensureThatLoadPublishedWorkflowsLoadOnlyPublishedProcessWhenHaveManyProcesses()
        {
            string json = "this JSON should be load1";
            string json2 = "this JSON should be load2";

            List<string> workFlowList = new List<string>(new string[] { "", json, json2, "" });

            _mockProcessService.Setup(x => x.ListWorkFlows()).Returns(workFlowList);
            _mockDefinitionLoader.Setup(x => x.LoadDefinition(It.IsAny<string>(), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()));

            WorkflowHostService workflowHostService = new WorkflowHostService(_mockWorkflowHost.Object, _mockDefinitionLoader.Object, _mockProcessService.Object, _mockLogger.Object);
            workflowHostService.LoadPublishedWorkflows();

            _mockProcessService.Verify(x => x.ListWorkFlows(), Times.Once());
            _mockDefinitionLoader.Verify(x => x.LoadDefinition(It.Is<string>(x => x == json), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()), Times.Once());
            _mockDefinitionLoader.Verify(x => x.LoadDefinition(It.Is<string>(x => x == json2), It.IsAny<Func<string, WorkflowCore.Models.DefinitionStorage.v1.DefinitionSourceV1>>()), Times.Once());
        }

        [Test]
        public async Task ensureStartFlow()
        {
            int processId = 3, version = 9, requesterId = 44;
            string connectionId = "someConnectionId";

            _mockWorkflowHost.Setup(x => x.StartWorkflow(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<string>()));

            WorkflowHostService workflowHostService = new WorkflowHostService(_mockWorkflowHost.Object, _mockDefinitionLoader.Object, _mockProcessService.Object, _mockLogger.Object);
            await workflowHostService.StartFlow(processId, version, requesterId, connectionId);

            _mockWorkflowHost.Verify(x => x.StartWorkflow(It.Is<string>(x => x == processId.ToString()), It.Is<int>(x => x == version), It.Is<FlowDataInfo>(x => x.RequesterId == requesterId && x.ConnectionId == connectionId), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ensureNextTask()
        {

            var nextStepDTO = new NextStepDTO()
            {
                OptionId = 10,
                TaskId = 3
            };

            var enventUser = new EventUserInfo(nextStepDTO.TaskId);

            _mockWorkflowHost.Setup(x => x.PublishEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTime>()));

            WorkflowHostService workflowHostService = new WorkflowHostService(_mockWorkflowHost.Object, _mockDefinitionLoader.Object, _mockProcessService.Object, _mockLogger.Object);
            await workflowHostService.NextTask(nextStepDTO);

            _mockWorkflowHost.Verify(x => x.PublishEvent(enventUser.eventName, enventUser.eventKey, It.IsAny<FlowDataInfo>(), It.IsAny<DateTime>()), Times.Once());
            Assert.AreEqual("eventName", enventUser.eventName);
            Assert.AreEqual("eventKey3", enventUser.eventKey);
        }
    }
}
