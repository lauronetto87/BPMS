using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    public class FlowServiceRequestTest : BaseTest
    {
        private ProcessVersionInfo processVersionInfo;
        private Mock<IFrontendNotifyService> frontendNotifyServiceMock;

        private event EventHandler<EventArgsNotify> FrontendNotifyServiceEvent;
        MockServices mockServices;

        [SetUp]
        public async Task Setup()
        {
            mockServices = new MockServices();
            mockServices.AddCustomizeServices((services) =>
            {
                frontendNotifyServiceMock = new Mock<IFrontendNotifyService>();
                frontendNotifyServiceMock.Setup(f => f.Notify(It.IsAny<string>(), It.IsAny<object>())).Callback((string connectionId, object message) => FrontendNotifyServiceEvent.Invoke(this, new EventArgsNotify(connectionId, message)));
                services.AddScoped((p) => frontendNotifyServiceMock.Object);
            });

            await mockServices.BuildServiceProvider();
            await mockServices.ActivationTenant();
            var processVersionId = await mockServices.NewProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField();
            processVersionInfo = mockServices.GetService<IProcessVersionService>().Get(processVersionId).Result.Value;
        }

        [Test]
        public async Task RequestProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField()
        {
            EventArgsNotify eventArgsNotify = null;

            FrontendNotifyServiceEvent += (object sender, EventArgsNotify e) =>
            {
                eventArgsNotify = e;
            };

            var flowRequestDTO = new FlowRequestDTO()
            {
                ProcessId = processVersionInfo.ProcessId,
                ConnectionId = mockServices.ConnectionId,
            };
            var result = await mockServices.GetService<IFlowService>().Request(flowRequestDTO);

            Assert.IsTrue(result.Success);

            WaitUntil(() => eventArgsNotify != null);

            Assert.AreEqual(eventArgsNotify.ConnectionId, flowRequestDTO.ConnectionId);
            Assert.IsNotNull(eventArgsNotify.Message);

            dynamic message = eventArgsNotify.Message;
            Assert.IsTrue(message.canExecute);

            var taskUserId = message.taskIdToExecute as int?;
            Assert.IsNotNull(taskUserId);
            var taskUserResult = await mockServices.GetService<ITaskService>().Get(taskUserId ?? 0);
            Assert.IsTrue(taskUserResult.Success);
            var taskUser = taskUserResult.Value;

            var dbContext = mockServices.GetService<DbContext>();
            // Limpa o rastreamento porque a thread do motor atualiza os dados e ao efetuar a busca era pego do cache
            // Uma outra opção é colocar o .AsNoTracking(), mas ocorre o seguinte erro em alguns casos:
            // The Include path 'SourceTasks->TargetTask' results in a cycle. Cycles are not allowed in no-tracking queries; either use a tracking query or remove the cycle.
            dbContext.ChangeTracker.Clear();
            var dbSetFlow = dbContext.Set<FlowInfo>();

            var formFieldName = "textField";


            var flow = dbSetFlow
                .Include(f => f.FieldValues).ThenInclude(t => t.Field)
                .Include(f => f.FlowPaths)
                .Include(f => f.Tasks).ThenInclude(t => t.FieldsValues)
                .Include(f => f.Tasks).ThenInclude(t => t.SourceTasks)
                .Include(f => f.Tasks).ThenInclude(t => t.TargetTasks)
                .Include(f => f.Tasks).ThenInclude(t => t.TasksHistories)
                .Include(f => f.Tasks).ThenInclude(t => t.Activity)
                .Include(f => f.Tasks)
                .FirstOrDefault(x => x.TenantId == mockServices.ContextData.Tenant.Id && x.Id == taskUser.FlowId);

            Assert.IsNotNull(flow);

            var taskStart = flow.Tasks.FirstOrDefault(t => t.Activity.Type == WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            Assert.IsNotNull(taskStart);

            taskUser = flow.Tasks.FirstOrDefault(t => t.Activity.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.IsNotNull(taskUser);

            AssertDateEqualNowWithDelay(flow.CreatedDate);
            Assert.IsNotNull(flow.FieldValues);
            Assert.AreEqual(flow.FieldValues.Count, 2);
            var fieldValue = flow.FieldValues[0];
            Assert.AreEqual(fieldValue.FieldValue, null);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskStart.Id);
            Assert.Greater(fieldValue.FieldId, 0);
            fieldValue = flow.FieldValues[1];
            Assert.AreEqual(fieldValue.FieldValue, null);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskUser.Id);
            Assert.Greater(fieldValue.FieldId, 0);

            Assert.IsNull(flow.FinishedDate);
            Assert.AreEqual(flow.ProcessVersionId, processVersionInfo.Id);
            Assert.AreEqual(flow.RequesterId, mockServices.ContextData.User.Id);
            Assert.AreEqual(flow.Status, FlowStatusEnum.INPROGRESS);
            Assert.AreEqual(flow.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.IsNotNull(flow.Tasks);
            Assert.AreEqual(flow.Tasks.Count, 2);

            AssertDateEqualNowWithDelay(taskStart.CreatedDate);
            Assert.IsNull(taskStart.ExecutorId);
            Assert.IsTrue(taskStart.FieldsValues != null && taskStart.FieldsValues.Count == 1);
             fieldValue = taskStart.FieldsValues[0];
            Assert.AreEqual(fieldValue.FieldValue, null);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskStart.Id);
            Assert.Greater(fieldValue.FieldId, 0);
            Assert.IsNotNull(taskStart.FinishedDate);
            AssertDateEqualNowWithDelay(taskStart.FinishedDate);
            Assert.IsTrue(taskStart.SourceTasks == null || taskStart.SourceTasks.Count == 0);
            Assert.IsNotNull(taskStart.TargetTasks);
            Assert.AreEqual(taskStart.TargetTasks.Count, 1);
            Assert.AreEqual(taskStart.TargetTasks[0].TargetTask.Activity.Type, WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.AreEqual(taskStart.TargetTasks[0].TargetTaskId, taskUser.Id);
            Assert.AreEqual(taskStart.TargetTasks[0].TargetTask.Id, taskUser.Id);
            Assert.True(taskStart.TasksHistories == null || taskStart.TasksHistories.Count == 0);
            Assert.AreEqual(taskStart.TenantId, mockServices.ContextData.Tenant.Id);

            AssertDateEqualNowWithDelay(taskUser.CreatedDate);
            Assert.AreEqual(taskUser.ExecutorId, mockServices.ContextData.User.Id);
            Assert.IsTrue(taskUser.FieldsValues != null && taskUser.FieldsValues.Count == 1);
            fieldValue = taskUser.FieldsValues[0];
            Assert.AreEqual(fieldValue.FieldValue, null);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskUser.Id);
            Assert.Greater(fieldValue.FieldId, 0);
            Assert.IsNull(taskUser.FinishedDate);
            Assert.IsNotNull(taskUser.SourceTasks);
            Assert.AreEqual(taskUser.SourceTasks.Count, 1);
            Assert.AreEqual(taskUser.SourceTasks[0].SourceTask.Activity.Type, WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            Assert.AreEqual(taskUser.SourceTasks[0].SourceTaskId, taskStart.Id);
            Assert.AreEqual(taskUser.SourceTasks[0].SourceTask.Id, taskStart.Id);
            Assert.True(taskUser.TargetTasks == null || taskUser.TargetTasks.Count == 0);
            Assert.IsTrue(taskUser.TasksHistories == null || taskUser.TasksHistories.Count == 0);
            Assert.AreEqual(taskUser.TenantId, mockServices.ContextData.Tenant.Id);
        }
    }
}