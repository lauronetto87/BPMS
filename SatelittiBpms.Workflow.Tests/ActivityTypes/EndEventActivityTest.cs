using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Tests.ActivityTypes
{
    public class EndEventActivityTest
    {
        int tenantId = 44,
            flowId = 26,
            activityId = 4,
            taskId = 3;

        Mock<IFieldValueService> _mockFieldValueService;
        Mock<IFlowService> _mockFlowService;
        Mock<ITaskService> _mockTaskService;
        Mock<IFlowPathService> _mockFlowPathService;
        Mock<IStepExecutionContext> _mockStepExecutionContext;

        [SetUp]
        public void Setup()
        {
            _mockFieldValueService = new Mock<IFieldValueService>();
            _mockFlowService = new Mock<IFlowService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockFlowPathService = new Mock<IFlowPathService>();
            _mockStepExecutionContext = new Mock<IStepExecutionContext>();
        }

        [Test]
        public async Task ensureThatEndEventActivityInsertTaskAndFlowPathAndUpdateFlow()
        {
            int insertedTaskId = 3;
            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(insertedTaskId, true, null));
            _mockFlowPathService.Setup(x => x.Insert(It.IsAny<FlowPathInfo>()));
            _mockFlowService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<FlowInfo>(new FlowInfo(), true, null));
            _mockFlowService.Setup(x => x.Update(It.IsAny<FlowInfo>()));
            _mockFieldValueService.Setup(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));

            EndEventActivity endEventActivity = new EndEventActivity(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockFlowService.Object)
            {
                TenantId = tenantId,
                FlowId = flowId,
                ActivityId = activityId,
                TaskId = taskId
            };
            var result = await endEventActivity.RunAsync(_mockStepExecutionContext.Object);

            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId && x.FlowId == flowId && x.ActivityId == activityId)), Times.Once());
            _mockFlowPathService.Verify(x => x.Insert(It.Is<FlowPathInfo>(x => x.TenantId == tenantId && x.FlowId == flowId && x.SourceTaskId == taskId && x.TargetTaskId == insertedTaskId)), Times.Once());
            _mockFlowService.Verify(x => x.Get(It.Is<int>(x => x == flowId)), Times.Once());
            _mockFlowService.Verify(x => x.Update(It.Is<FlowInfo>(x => x.Status == FlowStatusEnum.FINISHED)), Times.Once());
        }
    }
}
