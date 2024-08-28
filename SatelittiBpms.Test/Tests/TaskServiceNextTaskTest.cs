using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
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
using WorkflowCore.Interface;

namespace SatelittiBpms.Test.Tests
{
    public class TaskServiceNextTaskTest : BaseTest
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
        public async Task NextTaskWithOneUserActivityWithExecutorTypeRequesterAndOneField()
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
            var workFlowRequestResult = await mockServices.GetService<IFlowService>().Request(flowRequestDTO);
            Assert.IsTrue(workFlowRequestResult.Success);

            var workflowInstanceId = workFlowRequestResult.Value;
            WaitUntil(() => eventArgsNotify != null);

            dynamic message = eventArgsNotify.Message;

            var taskUserId = message.taskIdToExecute as int?;
            Assert.IsNotNull(taskUserId);

            var formFieldName = "textField";
            var formFieldValue = "Valor Campo 1";

            var nextStepDTO = new NextStepDTO()
            {
                OptionId = 1,
                TaskId = taskUserId.Value,
                FormData = new JObject
                {
                    { formFieldName, new JValue(formFieldValue) }
                },
            };
            var taskService = mockServices.GetService<ITaskService>();
            var resultTaskResult = await taskService.NextTask(nextStepDTO);
            Assert.IsTrue(resultTaskResult.Success);

            var workflowHost = mockServices.GetService<IWorkflowHost>();
            await WaitUntil(async () =>
            {
                var workflowInstance = await workflowHost.PersistenceStore.GetWorkflowInstance(workflowInstanceId);
                return workflowInstance.Status == WorkflowCore.Models.WorkflowStatus.Complete;
            });

            var taskUserResult = await taskService.Get(taskUserId.Value);
            Assert.IsTrue(taskUserResult.Success);

            var dbContext = mockServices.GetService<DbContext>();
            // Limpa o rastreamento porque a thread do motor atualiza os dados e ao efetuar a busca era pego do cache
            // Uma outra opção é colocar o .AsNoTracking(), mas ocorre o seguinte erro em alguns casos:
            // The Include path 'SourceTasks->TargetTask' results in a cycle. Cycles are not allowed in no-tracking queries; either use a tracking query or remove the cycle.
            dbContext.ChangeTracker.Clear();
            var dbSetFlow = dbContext.Set<FlowInfo>();
            var flow = dbSetFlow
                .Include(f => f.FieldValues).ThenInclude(t => t.Field)
                .Include(f => f.FlowPaths)
                .Include(f => f.Tasks).ThenInclude(t => t.FieldsValues).ThenInclude(t => t.Field)
                .Include(f => f.Tasks).ThenInclude(t => t.SourceTasks)
                .Include(f => f.Tasks).ThenInclude(t => t.TargetTasks)
                .Include(f => f.Tasks).ThenInclude(t => t.TasksHistories)
                .Include(f => f.Tasks).ThenInclude(t => t.Activity)
                .Include(f => f.Tasks)
                .FirstOrDefault(x => x.TenantId == mockServices.ContextData.Tenant.Id && x.Id == taskUserResult.Value.FlowId);

            Assert.IsNotNull(flow);

            AssertDateEqualNowWithDelay(flow.FinishedDate);
            Assert.AreEqual(flow.Status, FlowStatusEnum.FINISHED);
            Assert.IsNotNull(flow.Tasks);
            Assert.AreEqual(flow.Tasks.Count, 3);

            var taskStart = flow.Tasks.FirstOrDefault(t => t.Activity.Type == WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            Assert.IsNotNull(taskStart);

            var taskEnd = flow.Tasks.FirstOrDefault(t => t.Activity.Type == WorkflowActivityTypeEnum.END_EVENT_ACTIVITY);
            Assert.IsNotNull(taskEnd);

            var taskUser = flow.Tasks.FirstOrDefault(t => t.Activity.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.IsNotNull(taskUser);

            Assert.AreEqual(taskStart.FieldsValues.Count, 1);
            var fieldValue = taskStart.FieldsValues[0];
            Assert.IsNull(fieldValue.FieldValue);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.IsTrue(taskStart.SourceTasks == null || taskStart.SourceTasks.Count == 0);
            Assert.IsNotNull(taskStart.TargetTasks);
            Assert.AreEqual(taskStart.TargetTasks.Count, 1);
            Assert.AreEqual(taskStart.TargetTasks[0].TargetTask.Activity.Type, WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.AreEqual(taskStart.TargetTasks[0].TargetTaskId, taskUser.Id);
            Assert.AreEqual(taskStart.TargetTasks[0].TargetTask.Id, taskUser.Id);
            Assert.IsTrue(taskStart.TasksHistories == null || taskStart.TasksHistories.Count == 0);

            AssertDateEqualNowWithDelay(taskUser.FinishedDate);
            Assert.IsNotNull(taskUser.FieldsValues);
            Assert.AreEqual(taskUser.FieldsValues.Count, 1);
            fieldValue = taskUser.FieldsValues[0];
            Assert.AreEqual(fieldValue.FieldValue, formFieldValue);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);


            Assert.IsNotNull(taskUser.SourceTasks);
            Assert.AreEqual(taskUser.SourceTasks.Count, 1);
            Assert.AreEqual(taskUser.SourceTasks[0].SourceTask.Activity.Type, WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            Assert.AreEqual(taskUser.SourceTasks[0].SourceTaskId, taskStart.Id);
            Assert.AreEqual(taskUser.SourceTasks[0].SourceTask.Id, taskStart.Id);
            Assert.IsNotNull(taskUser.TargetTasks);
            Assert.AreEqual(taskUser.TargetTasks.Count, 1);
            Assert.AreEqual(taskUser.TargetTasks[0].TargetTask.Activity.Type, WorkflowActivityTypeEnum.END_EVENT_ACTIVITY);
            Assert.AreEqual(taskUser.TargetTasks[0].TargetTaskId, taskEnd.Id);
            Assert.AreEqual(taskUser.TargetTasks[0].TargetTask.Id, taskEnd.Id);

            // TaskHistory é usada somente para quando o usuário assina a tarefa para si mesmo TaskService.Assign
            Assert.IsTrue(taskUser.TasksHistories == null || taskUser.TasksHistories.Count == 0);

            AssertDateEqualNowWithDelay(taskEnd.CreatedDate);
            Assert.IsNull(taskEnd.ExecutorId);
            Assert.IsNotNull(taskEnd.FieldsValues);
            Assert.AreEqual(taskEnd.FieldsValues.Count, 1);
            fieldValue = taskEnd.FieldsValues[0];
            Assert.AreEqual(fieldValue.FieldValue, formFieldValue);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            AssertDateEqualNowWithDelay(taskEnd.FinishedDate);
            Assert.IsNotNull(taskEnd.SourceTasks);
            Assert.AreEqual(taskEnd.SourceTasks.Count, 1);
            Assert.AreEqual(taskEnd.SourceTasks[0].SourceTask.Activity.Type, WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.AreEqual(taskEnd.SourceTasks[0].SourceTaskId, taskUser.Id);
            Assert.AreEqual(taskEnd.SourceTasks[0].SourceTask.Id, taskUser.Id);
            Assert.True(taskEnd.TargetTasks == null || taskEnd.TargetTasks.Count == 0);
            Assert.IsTrue(taskEnd.TasksHistories == null || taskEnd.TasksHistories.Count == 0);
            Assert.AreEqual(taskEnd.TenantId, mockServices.ContextData.Tenant.Id);


            Assert.IsNotNull(flow.FieldValues);
            Assert.AreEqual(flow.FieldValues.Count, 3);
            fieldValue = flow.FieldValues[0];
            Assert.IsNull(fieldValue.FieldValue);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskStart.Id);
            Assert.Greater(fieldValue.FieldId, 0);
            fieldValue = flow.FieldValues[1];
            Assert.AreEqual(fieldValue.FieldValue, formFieldValue);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskUser.Id);
            Assert.Greater(fieldValue.FieldId, 0);
            fieldValue = flow.FieldValues[2];
            Assert.AreEqual(fieldValue.FieldValue, formFieldValue);
            Assert.AreEqual(fieldValue.Field.ComponentInternalId, formFieldName);
            Assert.AreEqual(fieldValue.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(fieldValue.TaskId, taskEnd.Id);
            Assert.Greater(fieldValue.FieldId, 0);
        }
    }
}