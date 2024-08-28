using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test;
using SatelittiBpms.Test.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Tests.Tests.Notification
{
    [TestFixture]
    public class NotificationListTest : BaseTest
    {
        MockServices mockServices;

        [SetUp]
        public void Setup()
        {
            mockServices = new MockServices();
        }

        [Test(Description = "Efetua os testes para listagem de notificações.")]
        public async Task TestYouNeedToRunTask()
        {
            var executeResult = await mockServices
                .BeginCreateProcess()
                    .ActivityUser()
                        .ExecutorRequester()
                .EndCreateProcess()
                .NewFlow()
                    .ExecuteTask()
                .NewFlow()
                    .ExecuteAllTasks()
                .EndCreateFlows()
                .ExecuteInDataBase();

            var notificationService = mockServices.GetService<INotificationService>();


            var notifications = await notificationService.List();
            Assert.AreEqual(2, notifications.Count);

            var firstNotificationToDisplay = notifications[0];
            var secondNotificationToDisplay = notifications[1];
            var firstFlowInfoExecuted = executeResult.FlowsExecuted[0].FlowInfo;
            var secondFlowInfoExecuted = executeResult.FlowsExecuted[1].FlowInfo;
            TestNotification(firstFlowInfoExecuted, secondNotificationToDisplay);
            TestNotification(secondFlowInfoExecuted, firstNotificationToDisplay);

            var result = await notificationService.SetToRead(notifications[0].Id);
            Assert.IsTrue(result.Success);

            notifications = await notificationService.List();
            Assert.AreEqual(2, notifications.Count);
            Assert.IsTrue(notifications[0].Read);
            Assert.IsFalse(notifications[1].Read);

            result = await notificationService.SetToDeleted(notifications[0].Id);
            Assert.IsTrue(result.Success);

            notifications = await notificationService.List();
            Assert.AreEqual(1, notifications.Count);
            Assert.IsFalse(notifications[0].Read);
        }

        private static void TestNotification(Models.Infos.FlowInfo flowInfo, Models.ViewModel.NotificationViewModel notification)
        {
            Assert.AreEqual(flowInfo.ProcessVersion.Name, notification.ProcessName);
            var taskUser = flowInfo.Tasks.First(x => x.Activity.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.AreEqual(taskUser.Activity.Name, notification.TaskName);
            Assert.AreEqual(taskUser.Id, notification.TaskId);
            AssertDateEqualNowWithDelay(notification.Date);
            Assert.AreEqual(flowInfo.Id, notification.FlowId);
            Assert.AreEqual(false, notification.Read);
            Assert.IsNull(notification.RoleId);
            Assert.IsNull(notification.RoleName);
            Assert.AreEqual(NotificationTypeEnum.YouNeedToRunTask, notification.Type);
        }
    }
}
