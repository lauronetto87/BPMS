using Moq;
using NUnit.Framework;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using SatelittiBpms.Workflow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Tests.ActivityTypes
{
    public class UserTaskActivityTest
    {
        int tenantId = 23,
            activityId = 3,
            requesterId = 65;

        Mock<IFieldValueService> _mockFieldValueService;
        Mock<IFlowPathService> _mockFlowPathService;
        Mock<ITaskService> _mockTaskService;
        Mock<ITaskHistoryService> _mockTaskHistoryService;
        Mock<IActivityUserService> _mockActivityUserService;
        Mock<IStepExecutionContext> _mockStepExecutionContext;
        Mock<IFrontendNotifyService> _mockFrontendNotifyService;
        Mock<IFlowService> _mockFlowService;
        Mock<INotificationService> _mockNotificationService;

        [SetUp]
        public void Setup()
        {
            _mockFieldValueService = new Mock<IFieldValueService>();
            _mockFlowPathService = new Mock<IFlowPathService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockTaskHistoryService = new Mock<ITaskHistoryService>();
            _mockActivityUserService = new Mock<IActivityUserService>();
            _mockStepExecutionContext = new Mock<IStepExecutionContext>();
            _mockFrontendNotifyService = new Mock<IFrontendNotifyService>();
            _mockFlowService = new Mock<IFlowService>();
            _mockNotificationService = new Mock<INotificationService>();
        }

        [Test]
        public async Task ensureThatUserEventActivityInsertFlowPathAndTask()
        {
            int flowPathId = 84, taskId = 6;

            _mockStepExecutionContext.SetupGet(x => x.ExecutionPointer).Returns(new WorkflowCore.Models.ExecutionPointer { EventPublished = false });
            _mockFlowPathService.Setup(x => x.Insert(It.IsAny<FlowPathInfo>())).ReturnsAsync(new ResultContent<int>(flowPathId, true, null));
            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));
            _mockFieldValueService.Setup(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockFrontendNotifyService.Setup(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()));

            var flowInfo = new FlowInfo
            {
                RequesterId = 1,
            };
            _mockFlowService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<FlowInfo>(flowInfo, true, null));

            var taskInfo = new TaskInfo
            {
                Id = taskId,
                Activity = new ActivityInfo
                {
                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                }
            };
            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<TaskInfo>(taskInfo, true, null));


            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };
            var result = await userEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.IsFalse(result.Proceed);
            Assert.NotNull(result.PersistenceData);
            Assert.AreEqual(taskId, result.PersistenceData);

            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once());
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId)), Times.Once());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Never());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockTaskHistoryService.Verify(x => x.UpdateEndDateFromTaskHistories(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatUserEventActivityWhenEventPublished()
        {
            int taskId = 6;
            var taskInfo = new TaskInfo
            {
                Id = taskId,
                Activity = new ActivityInfo
                {
                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                }
            };
            var flowDataInfo = new FlowDataInfo
            {
                FlowId = 2,
                RequesterId = 1,
                TaskId = taskId
            };

            _mockStepExecutionContext.SetupGet(x => x.ExecutionPointer).Returns(new WorkflowCore.Models.ExecutionPointer { EventPublished = true, EventData = flowDataInfo });
            _mockTaskService.Setup(x => x.Get(taskId)).ReturnsAsync(new ResultContent<TaskInfo>(taskInfo, true, null));
            _mockTaskService.Setup(x => x.Update(taskInfo));
            _mockFieldValueService.Setup(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockFrontendNotifyService.Setup(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()));

            var flowInfo = new FlowInfo
            {
                RequesterId = 1,
            };
            _mockFlowService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<FlowInfo>(flowInfo, true, null));

            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<TaskInfo>(taskInfo, true, null));


            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };

            var result = await userEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.IsTrue(result.Proceed);

            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Never());
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId)), Times.Never());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Once());
            _mockTaskHistoryService.Verify(x => x.UpdateEndDateFromTaskHistories(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatUserEventActivityWhenNeedWait()
        {
            int taskId = 6;
            var taskInfo = new TaskInfo
            {
                Id = taskId,
                Activity = new ActivityInfo
                {
                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                }
            };
            var flowDataInfo = new FlowDataInfo
            {
                FlowId = 2,
                RequesterId = 1,
                TaskId = taskId
            };

            _mockStepExecutionContext.SetupGet(x => x.ExecutionPointer).Returns(new WorkflowCore.Models.ExecutionPointer { EventPublished = false });
            _mockStepExecutionContext.SetupGet(x => x.PersistenceData).Returns(taskId);

            var flowInfo = new FlowInfo
            {
                RequesterId = 1,
            };
            _mockFlowService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<FlowInfo>(flowInfo, true, null));

            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };

            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<TaskInfo>(taskInfo, true, null));


            var result = await userEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.IsFalse(result.Proceed);
            Assert.AreEqual("eventName", result.EventName);
            Assert.AreEqual($"eventKey{taskId}", result.EventKey);

            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Never());
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId)), Times.Never());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Never());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockTaskHistoryService.Verify(x => x.UpdateEndDateFromTaskHistories(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatInsertTaskWhenRequesterExecutorType()
        {
            int flowPathId = 43, taskId = 2;

            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));

            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId,
                FlowId = flowPathId,
                ActivityUserExecutorType = UserTaskExecutorTypeEnum.REQUESTER
            };

            var result = await userEventActivity.InsertTask();
            Assert.AreEqual(taskId, result);

            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(y => y.ExecutorId == requesterId)), Times.Once());
            _mockActivityUserService.Verify(x => x.Get(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatInsertTaskWhenRoleExecutorType()
        {
            int flowPathId = 534, taskId = 67;

            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));

            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId,
                FlowId = flowPathId,
                ActivityUserExecutorType = UserTaskExecutorTypeEnum.ROLE
            };

            var result = await userEventActivity.InsertTask();
            Assert.AreEqual(taskId, result);

            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(y => y.ExecutorId == null)), Times.Once());
            _mockActivityUserService.Verify(x => x.Get(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatInsertTaskWhenPersonExecutorType()
        {
            int flowPathId = 862, taskId = 77;

            ActivityUserInfo activitiUserInfo = new()
            {
                PersonId = 582
            };

            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));
            _mockActivityUserService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<ActivityUserInfo>(activitiUserInfo, true, null));

            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId,
                FlowId = flowPathId,
                ActivityUserExecutorType = UserTaskExecutorTypeEnum.PERSON
            };

            var result = await userEventActivity.InsertTask();
            Assert.AreEqual(taskId, result);

            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(y => y.ExecutorId == activitiUserInfo.PersonId)), Times.Once());
            _mockActivityUserService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatNotifyEventWithUserResponsibilityTask()
        {
            int taskId = 6;
            var taskInfo = new TaskInfo 
            {
                Id = taskId,
                Activity = new ActivityInfo
                {
                    Type = WorkflowActivityTypeEnum.START_EVENT_ACTIVITY,
                }
            };
            var flowDataInfo = new FlowDataInfo
            {
                FlowId = 2,
                RequesterId = 1,
                TaskId = taskId
            };

            _mockStepExecutionContext.SetupGet(x => x.ExecutionPointer).Returns(new WorkflowCore.Models.ExecutionPointer { EventPublished = false });
            _mockStepExecutionContext.SetupGet(x => x.PersistenceData).Returns(null);

            var flowInfo = new FlowInfo
            {
                RequesterId = 1,
            };
            _mockFlowService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<FlowInfo>(flowInfo, true, null));

            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync((Result.Success(taskId)));

            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<TaskInfo>(taskInfo, true, null));

            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };

            userEventActivity.RequesterId = 1;
            userEventActivity.ActivityUserExecutorType = UserTaskExecutorTypeEnum.REQUESTER;

            var result = await userEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.IsFalse(result.Proceed);

            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.Is<IDictionary<string, object?>>((d) => ((bool)d["canExecute"]) && ((int)d["taskIdToExecute"]) == 6)));

            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once());
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId)), Times.Once());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockTaskHistoryService.Verify(x => x.UpdateEndDateFromTaskHistories(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatNotifyEventWithUserNonResponsibilityTask()
        {
            int taskId = 6;
            var taskInfo = new TaskInfo
            {
                Id = taskId,
                Activity = new ActivityInfo
                {
                    Type = WorkflowActivityTypeEnum.SEND_TASK_ACTIVITY,
                }
            };
            var flowDataInfo = new FlowDataInfo
            {
                FlowId = 2,
                RequesterId = 1,
                TaskId = taskId
            };

            _mockStepExecutionContext.SetupGet(x => x.ExecutionPointer).Returns(new WorkflowCore.Models.ExecutionPointer { EventPublished = false });
            _mockStepExecutionContext.SetupGet(x => x.PersistenceData).Returns(null);

            var flowInfo = new FlowInfo
            {
                RequesterId = 1,
            };
            _mockFlowService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<FlowInfo>(flowInfo, true, null));

            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync((Result.Success(taskId)));

            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ResultContent<TaskInfo>(taskInfo, true, null));

            UserTaskActivity userEventActivity = new(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockTaskHistoryService.Object, _mockActivityUserService.Object, _mockFrontendNotifyService.Object, _mockNotificationService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };

            userEventActivity.RequesterId = 2;
            userEventActivity.ActivityUserExecutorType = UserTaskExecutorTypeEnum.REQUESTER;

            var result = await userEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.IsFalse(result.Proceed);

            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.Is<IDictionary<string, object?>>((d) => !(bool)d["canExecute"] )));

            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once());
            _mockTaskService.Verify(x => x.Insert(It.Is<TaskInfo>(x => x.TenantId == tenantId)), Times.Once());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockTaskHistoryService.Verify(x => x.UpdateEndDateFromTaskHistories(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

    }
}
