using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Tests.ActivityTypes
{
    public class StartEventActivityTest
    {
        int tenantId = 23,
            activityId = 3,
            processVersionId = 45,
            requesterId = 65;

        Mock<IFlowService> _mockFlowService;
        Mock<ITaskService> _mockTaskService;
        Mock<IStepExecutionContext> _mockStepExecutionContext;

        [SetUp]
        public void Setup()
        {
            _mockFlowService = new Mock<IFlowService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockStepExecutionContext = new Mock<IStepExecutionContext>();
        }

        [Test]
        public async Task ensureThatStartEventActivityInsertFlowAndTask()
        {
            int flowId = 84, taskId = 6;

            _mockFlowService.Setup(x => x.Insert(It.IsAny<FlowInfo>())).ReturnsAsync(new ResultContent<int>(flowId, true, null));
            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));

            _mockFlowService.Setup(x => x.GetFields(It.Is<int>(p => p == flowId))).Returns(new List<FieldInfo>());

            StartEventActivity startEventActivity = new StartEventActivity(_mockTaskService.Object, _mockFlowService.Object)
            {
                TenantId = tenantId,
                ProcessVersionId = processVersionId,
                RequesterId = requesterId,
                ActivityId = activityId
            };
            var result = await startEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.AreEqual(flowId, startEventActivity.FlowId);
            Assert.AreEqual(taskId, startEventActivity.TaskId);

            _mockFlowService.Verify(x => x.Insert(It.Is<FlowInfo>(x => x.TenantId == tenantId && x.ProcessVersionId == processVersionId && x.RequesterId == requesterId && x.Status == FlowStatusEnum.INPROGRESS)));
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId && x.FlowId == flowId && x.ActivityId == activityId)));
        }
    }
}
