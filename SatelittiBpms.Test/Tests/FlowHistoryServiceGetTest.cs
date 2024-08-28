using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Extensions;
using SatelittiBpms.Test.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    [TestFixture]
    public class FlowHistoryServiceGetTest : BaseTest
    {
        MockServices mockServices;

        [SetUp]
        public void Setup()
        {
            mockServices = new MockServices();
        }

        [Test(Description = "Efetua o teste com todas as regras levantadas na estória para um fluxo em andamento.")]
        public async Task TestWithFlowInProgress()
        {
            var fieldId = new DataId();

            var wildcardTestData = WildcardHelper.BuildWildcardTestData(fieldId.InternalId);

            var data = await mockServices
                .BeginCreateProcess()
                    .DescriptionFlow(wildcardTestData.BuildUserInput())
                    .Field()
                        .Id(fieldId)
                        .Type(FieldTypeEnum.TEXTFIELD)
                    .ActivityUser()
                        .Field()
                            .Id(fieldId)
                            .State(ProcessTaskFieldStateEnum.MANDATORY)
                        .ExclusiveGateway()
                            .Branch()
                                .ActivitySend()
                                .ActivityUser()
                            .Branch()
                                .ActivitySend()
                                .ActivityUser()
                            .Branch()
                                .ActivitySend()
                                .ActivityUser()
                .EndCreateProcess()
                .NewFlow()
                    .ExecuteTask()
                .EndCreateFlows()
                .ExecuteInDataBase();

            var flowExecuted = data.FlowsExecuted[0];
            var flowHistoryResult = await mockServices.GetService<IFlowHistoryService>().Get(flowExecuted.FlowId);


            Assert.IsTrue(flowHistoryResult.Success);

            var flowHistory = flowHistoryResult.Value;
            AssertFlowHeader(data, flowHistory, wildcardTestData);

            var taskOpen = flowExecuted.FlowInfo.Tasks.First(t => t.FinishedDate == null);
            await AssertLineOne(flowHistory.FlowHistoryTasks[0], taskOpen);

            var taskSend = flowExecuted.FlowInfo.Tasks.First(t => t.Activity.Type == WorkflowActivityTypeEnum.SEND_TASK_ACTIVITY);
            AssertLineTwo(flowHistory.FlowHistoryTasks[1], taskSend);

            var taskExclusiveGateway = flowExecuted.FlowInfo.Tasks.First(t => t.Activity.Type == WorkflowActivityTypeEnum.EXCLUSIVE_GATEWAY_ACTIVITY);
            AssertLineThree(flowHistory.FlowHistoryTasks[2], taskExclusiveGateway);

            var userTaskFinished = flowExecuted.FlowInfo.Tasks.First(t => t.Activity.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY && t.FinishedDate != null);
            await AssertLineFour(flowHistory.FlowHistoryTasks[3], userTaskFinished, flowExecuted.Tasks[0]);

            var taskStart = flowExecuted.FlowInfo.Tasks.First(t => t.Activity.Type == WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            await AssertLineFive(flowHistory.FlowHistoryTasks[4], taskStart);
        }

        private static void AssertFlowHeader(Data.FlowExecuteResult data, Models.ViewModel.FlowHistoryViewModel flowHistory, WildcardHelper.WildcardTestData wildcardTestData)
        {
            var flowExecuted = data.FlowsExecuted[0];

            var fieldValues = flowExecuted.Tasks.Last().TaskInput.FieldValues;

            Assert.AreEqual(flowHistory.DiagramContent, data.ProcessVersion.DiagramContent);
            Assert.AreEqual(flowHistory.FlowId, flowExecuted.FlowId);
            Assert.AreEqual(flowHistory.ProcessName, data.ProcessVersion.Name);
            Assert.AreEqual(flowHistory.FlowDescription, wildcardTestData.BuildResult(fieldValues, flowExecuted.FlowId));
            Assert.AreEqual(flowHistory.FlowHistoryTasks.Count, 5);
        }

        private async Task AssertLineOne(Models.ViewModel.FlowHistoryTaskViewModel history, TaskInfo taskOpen)
        {
            Assert.IsNull(history.ActionDescription);
            Assert.AreEqual(history.ActivityType, taskOpen.Activity.Type);
            Assert.AreEqual(history.CreatedDatetime, taskOpen.CreatedDate);
            Assert.AreEqual(history.ExecutorName, await UserHelper.GetNameOfUser(taskOpen.ExecutorId ?? 0, mockServices));
            Assert.AreEqual(history.FinishedDatetime, taskOpen.FinishedDate);
            Assert.AreEqual(history.TaskName, taskOpen.Activity.Name);
        }

        private void AssertLineTwo(Models.ViewModel.FlowHistoryTaskViewModel history, TaskInfo taskSend)
        {
            Assert.AreEqual(history.ActionDescription, "flows.flowHistory.table.labels.system");
            Assert.AreEqual(history.ActivityType, taskSend.Activity.Type);
            Assert.AreEqual(history.CreatedDatetime, taskSend.CreatedDate);
            Assert.AreEqual(history.ExecutorName, "flows.flowHistory.table.labels.executorSystem");
            Assert.AreEqual(history.FinishedDatetime, taskSend.FinishedDate);
            Assert.AreEqual(history.TaskName, taskSend.Activity.Name);
        }

        private void AssertLineThree(Models.ViewModel.FlowHistoryTaskViewModel history, TaskInfo taskExclusiveGateway)
        {
            Assert.AreEqual(history.ActionDescription, "flows.flowHistory.table.labels.system");
            Assert.AreEqual(history.ActivityType, taskExclusiveGateway.Activity.Type);
            Assert.AreEqual(history.CreatedDatetime, taskExclusiveGateway.CreatedDate);
            Assert.AreEqual(history.ExecutorName, "flows.flowHistory.table.labels.executorSystem");
            Assert.AreEqual(history.FinishedDatetime, taskExclusiveGateway.FinishedDate);
            Assert.AreEqual(history.TaskName, taskExclusiveGateway.Activity.Name);
        }

        private async Task AssertLineFour(Models.ViewModel.FlowHistoryTaskViewModel history, TaskInfo userTaskFinished, Data.TaskExecutedData taskExecutedData)
        {
            Assert.AreEqual(history.ActionDescription, taskExecutedData.OptionButton.Description);
            Assert.AreEqual(history.ActivityType, userTaskFinished.Activity.Type);
            Assert.AreEqual(history.CreatedDatetime, userTaskFinished.CreatedDate);
            Assert.AreEqual(history.ExecutorName, await UserHelper.GetNameOfUser(userTaskFinished.ExecutorId, mockServices));
            Assert.AreEqual(history.FinishedDatetime, userTaskFinished.FinishedDate);
            Assert.AreEqual(history.TaskName, userTaskFinished.Activity.Name);
        }

        private async Task AssertLineFive(Models.ViewModel.FlowHistoryTaskViewModel history, TaskInfo taskStart)
        {
            Assert.AreEqual(history.ActionDescription, "flows.flowHistory.table.labels.request");
            Assert.AreEqual(history.ActivityType, taskStart.Activity.Type);
            Assert.AreEqual(history.CreatedDatetime, taskStart.CreatedDate);
            Assert.AreEqual(history.ExecutorName, await UserHelper.GetNameOfUser(taskStart.Flow.RequesterId, mockServices));
            Assert.AreEqual(history.FinishedDatetime, taskStart.FinishedDate);
            Assert.AreEqual(history.TaskName, taskStart.Activity.Name);
        }


        [Test(Description = "Efetua o teste específico para um fluxo finalizado, testando apenas as variações comparando ao fluxo aberto.")]
        public async Task TestWithFinishedFlow()
        {
            var data = await mockServices
                .BeginCreateProcess()
                    .ActivityUser()
                .EndCreateProcess()
                .NewFlow()
                    .ExecuteAllTasks()
                .EndCreateFlows()
                .ExecuteInDataBase();

            var flowExecuted = data.FlowsExecuted[0];

            var flowHistoryResult = await mockServices.GetService<IFlowHistoryService>().Get(flowExecuted.FlowId);
            Assert.IsTrue(flowHistoryResult.Success);

            var flowHistory = flowHistoryResult.Value;

            Assert.AreEqual(flowHistory.FlowHistoryTasks.Count, 3);

            var history = flowHistory.FlowHistoryTasks[0];
            var taskEnd = flowExecuted.FlowInfo.Tasks.First(t => t.Activity.Type == WorkflowActivityTypeEnum.END_EVENT_ACTIVITY);

            Assert.AreEqual(history.ActionDescription, "flows.flowHistory.table.labels.finished");
            Assert.AreEqual(history.ActivityType, taskEnd.Activity.Type);
            Assert.AreEqual(history.CreatedDatetime, taskEnd.CreatedDate);
            Assert.AreEqual(history.ExecutorName, "flows.flowHistory.table.labels.executorSystem");
            Assert.AreEqual(history.FinishedDatetime, taskEnd.FinishedDate);
            Assert.AreEqual(history.TaskName, taskEnd.Activity.Name);
        }
    }
}
