using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Tests.ActivityTypes
{
    public class ExclusiveGatewayActivityTest
    {
        int tenantId = 55,
            activityId = 10;

        Mock<IFieldValueService> _mockFieldValueService;
        Mock<IFlowPathService> _mockFlowPathService;
        Mock<ITaskService> _mockTaskService;
        Mock<IStepExecutionContext> _mockStepExecutionContext;

        [SetUp]
        public void Setup()
        {
            _mockFieldValueService = new Mock<IFieldValueService>();
            _mockFlowPathService = new Mock<IFlowPathService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockStepExecutionContext = new Mock<IStepExecutionContext>();
        }

        [Test]
        public async Task ensureThatExclusiveGatewayActivityInsertFlowAndTask()
        {
            int flowId = 84, taskId = 6, flowPathId = 10;

            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));
            _mockFlowPathService.Setup(x => x.Insert(It.IsAny<FlowPathInfo>())).ReturnsAsync(new ResultContent<int>(flowPathId, true, null));
            _mockFieldValueService.Setup(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

            ExclusiveGatewayActivity exclusiveActivity = new ExclusiveGatewayActivity(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object)
            {
                FlowId = flowId,
                TaskId = taskId,
                ActivityId = activityId,
                TenantId = tenantId
            };

            var result = await exclusiveActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.AreEqual(flowId, exclusiveActivity.FlowId);
            Assert.AreEqual(taskId, exclusiveActivity.TaskId);

            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once);
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId)));
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }
    }
}
