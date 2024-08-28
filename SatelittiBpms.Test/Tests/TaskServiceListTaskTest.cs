using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    public class TaskServiceListTaskTest : BaseTest
    {        
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

            await mockServices.NewProcessVersionWithTwoUserActivityBothWithRequesterAndWithThreeFields();
            await mockServices.NewProcessVersionWithUserActivityBothWithPaperAndWithThreeFields();
            
            await ListRequestProcessVersion();
            await ListNextRequestProcessVersion();            
        }


        [Test(Description = "Lista de tarefas 'Para você fazer', sem agrupamento e sem filtros")]
        public async Task ListTasksWithFiltersTaskGroupTypeEqualUngroupedAndTaskQueryTypeEqualMytasks()
        {
            EventArgsNotify eventArgsNotify = null;

            FrontendNotifyServiceEvent += (object sender, EventArgsNotify e) =>
            {
                eventArgsNotify = e;
            };

            var taskFilterDTO = new TaskFilterDTO
            {
                GroupId = null,
                SortOrder = 0,
                TaskGroupType = TaskGroupType.UNGROUPED,
                TaskQueryType = TaskQueryType.MYTASKS,
                TextSearch = "",
                TotalByQuery = 25                
            };

            var result = await mockServices.GetService<ITaskService>().ListTasks(taskFilterDTO);
            Assert.IsTrue(result.Success);


            var resultValue = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.AreEqual(10, resultValue.Quantity);
            Assert.AreEqual(10, resultValue.List.Count);

            Assert.AreEqual("Tarefa 1", resultValue.List[0].ActivityName);
            Assert.AreEqual(false, resultValue.List[0].Finished);
            Assert.AreEqual("PROCESS 2", resultValue.List[0].Name);
            Assert.AreEqual(UserTaskExecutorTypeEnum.ROLE, resultValue.List[0].ExecutorType);
        }

        [Test(Description = "Lista de tarefas 'Todos', sem agrupamento e sem filtros")]
        public async Task ListTasksWithFiltersTaskGroupTypeEqualUngroupedAndTaskQueryTypeEqualAll()
        {
            EventArgsNotify eventArgsNotify = null;

            FrontendNotifyServiceEvent += (object sender, EventArgsNotify e) =>
            {
                eventArgsNotify = e;
            };

            var taskFilterDTO = new TaskFilterDTO
            {
                GroupId = null,
                SortOrder = 0,
                TaskGroupType = TaskGroupType.UNGROUPED,
                TaskQueryType = TaskQueryType.ALL,
                TextSearch = "tarefa",
                TotalByQuery = 25                
            };

            var result = await mockServices.GetService<ITaskService>().ListTasks(taskFilterDTO);
            Assert.IsTrue(result.Success);


            var resultValue = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.AreEqual(10, resultValue.Quantity);
            Assert.AreEqual(10, resultValue.List.Count);

            Assert.AreEqual("Tarefa 3", resultValue.List[0].ActivityName);
            Assert.AreEqual(false, resultValue.List[0].Finished);
            Assert.AreEqual("PROCESS 1", resultValue.List[0].Name);
            Assert.AreEqual(UserTaskExecutorTypeEnum.REQUESTER, resultValue.List[0].ExecutorType);
        }

        [Test]
        public async Task ListTaskGroupWithFiltersTaskGroupTypeEqualProcessAndTaskQueryTypeEqualMytasks()
        {
            var taskFilterDTO = new TaskFilterDTO
            {
                GroupId = null,
                SortOrder = 0,
                TaskGroupType = TaskGroupType.PROCESS,
                TaskQueryType = TaskQueryType.MYTASKS,
                TextSearch = "tarefa",
                TotalByQuery = 25
            };

            var result = await mockServices.GetService<ITaskService>().ListTasksGroup(taskFilterDTO);
            Assert.IsTrue(result.Success);

            var resultValue = ResultContent<List<FlowGroupViewModel>>.GetValue(result);
            Assert.IsNotNull(resultValue);

            Assert.AreEqual(2, resultValue.Count);
            Assert.AreEqual("PROCESS 2", resultValue[0].Name);
            Assert.AreEqual("PROCESS 1", resultValue[1].Name);
        }

        [Test]
        public async Task ListTaskGroupWithFiltersTaskGroupTypeEqualTaskAndTaskQueryTypeEqualMytasks()
        {
            var taskFilterDTO = new TaskFilterDTO
            {
                GroupId = null,
                SortOrder = 0,
                TaskGroupType = TaskGroupType.TASK,
                TaskQueryType = TaskQueryType.MYTASKS,
                TextSearch = "tarefa",
                TotalByQuery = 25
            };

            var result = await mockServices.GetService<ITaskService>().ListTasksGroup(taskFilterDTO);
            Assert.IsTrue(result.Success);

            var resultValue = ResultContent<List<FlowGroupViewModel>>.GetValue(result);
            Assert.IsNotNull(resultValue);
        }

        private async Task ListRequestProcessVersion()
        {            
            var dbContext = mockServices.GetService<DbContext>();
            dbContext.ChangeTracker.Clear();
            var dbSetProcessVersion = dbContext.Set<ProcessVersionInfo>();
            var lstProcessVersion = dbSetProcessVersion.
                Where(x => x.Status == ProcessStatusEnum.PUBLISHED && x.TenantId == mockServices.ContextData.Tenant.Id).ToList();

            foreach (var processVersion in lstProcessVersion)
            {
                await RequestProcessVersion(processVersion.Id);
                await RequestProcessVersion(processVersion.Id);
                await RequestProcessVersion(processVersion.Id);
                await RequestProcessVersion(processVersion.Id);
                await RequestProcessVersion(processVersion.Id);                
            }
        }

        private async Task RequestProcessVersion(int ProcessVersionId)
        {
            EventArgsNotify eventArgsNotify = null;

            FrontendNotifyServiceEvent += (object sender, EventArgsNotify e) =>
            {
                eventArgsNotify = e;
            };

            var flowRequestDTO = new FlowRequestDTO()
            {
                ProcessId = ProcessVersionId,                
                ConnectionId = mockServices.ConnectionId,
            };
            var result = await mockServices.GetService<IFlowService>().Request(flowRequestDTO);

            Assert.IsTrue(result.Success);

            WaitUntil(() => eventArgsNotify != null);

            Assert.AreEqual(eventArgsNotify.ConnectionId, flowRequestDTO.ConnectionId);
            Assert.IsNotNull(eventArgsNotify.Message);
        }

        private async Task ListNextRequestProcessVersion()
        {
            var dbContext = mockServices.GetService<DbContext>();
            dbContext.ChangeTracker.Clear();
            var dbSetTask = dbContext.Set<TaskInfo>();

            var lstTask = dbSetTask
                .Include(t => t.Flow)
                .Where(x => x.TenantId == mockServices.ContextData.Tenant.Id && x.FinishedDate == null && x.Flow.ProcessVersionId == 1).ToList();

            foreach (var task in lstTask)
            {
                await NextRequestProcessVersion(task.Id);                
            }
        }

        private async Task NextRequestProcessVersion(int TaskId)
        {
            EventArgsNotify eventArgsNotify = null;

            FrontendNotifyServiceEvent += (object sender, EventArgsNotify e) =>
            {
                eventArgsNotify = e;
            };

            var nextStepDTO = new NextStepDTO()
            {
                OptionId = 1,
                TaskId = TaskId,
                FormData = new JObject
                {
                    { "textField", new JValue("Valor Campo 1") },
                    { "email", new JValue("selbetti@email.com") },
                    { "textArea", new JValue("Observação teste") }
                },
            };
            var taskService = mockServices.GetService<ITaskService>();
            var resultTaskResult = await taskService.NextTask(nextStepDTO);
            Assert.IsTrue(resultTaskResult.Success);

            WaitUntil(() => eventArgsNotify != null);

            var taskUserResult = await taskService.Get(TaskId);
            Assert.IsTrue(taskUserResult.Success);
        }
    }
}
